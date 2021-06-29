using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using GrpcWebBlazorWasm.Shared;
using Microsoft.AspNetCore.Components;

namespace GrpcWebBlazorWasm.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddHttpClient("GrpcWebBlazorWasm.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            // Supply HttpClient instances that include access tokens when making requests to the server project
            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("GrpcWebBlazorWasm.ServerAPI"));

            //builder.Services.AddApiAuthorization();
            builder.Services.AddApiAuthorization()
                .AddAccountClaimsPrincipalFactory<RolesClaimsPrincipalFactory>();

            builder.Services.AddScoped(services =>
            {
                // Get the service address from appsettings.json
                var config = services.GetRequiredService<IConfiguration>();
                var backendUrl = config["Settings:BackEndUrl"];

                // If no address is set then fallback to the current webpage URL
                if (string.IsNullOrEmpty(backendUrl))
                {
                    var navigationManager = services.GetRequiredService<NavigationManager>();
                    backendUrl = navigationManager.BaseUri;
                }

                // Create a channel with a GrpcWebHandler that is addressed to the backend server.
                //
                // GrpcWebText is used because server streaming requires it. If server streaming is not used in your app
                // then GrpcWeb is recommended because it produces smaller messages.
                var grpcWebHandler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, new HttpClientHandler());
                var channel= GrpcChannel.ForAddress(backendUrl, new GrpcChannelOptions { HttpHandler = grpcWebHandler });
                return new TouristRouteProtoSevice.TouristRouteProtoSeviceClient(channel);
            });

			await builder.Build().RunAsync();
        }
    }
}
