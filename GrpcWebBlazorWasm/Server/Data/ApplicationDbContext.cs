using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using Google.Protobuf.WellKnownTypes;
using GrpcWebBlazorWasm.Server.Models;
using GrpcWebBlazorWasm.Shared;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GrpcWebBlazorWasm.Server.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        public DbSet<TouristRoute> TouristRoutes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TouristRoute>().Ignore(route => route.Price);

            modelBuilder.Entity<TouristRoute>(builder =>
            {
                builder.Property(route => route.Id)
                    .HasConversion(s => new Guid(s),
                        guid => guid.ToString());
                builder.Property(route => route.Title)
                    .IsRequired().HasMaxLength(100);
                builder.Property(route => route.Description)
                    .HasMaxLength(1500);
                builder.Property(route => route.CreateTime)
                    .IsRequired()
                    .HasDefaultValue(default(DateTimeOffset).ToTimestamp())
                    .HasConversion(timestamp => timestamp.ToDateTime(),
                        time => time.ToUniversalTime().ToTimestamp());
                builder.Property(route => route.UpdateTime)  //
                    .HasConversion(timestamp => timestamp.ToDateTime(),
                        time => time.ToUniversalTime().ToTimestamp());
                builder.Property(route => route.DepartureTime) //
                    .HasConversion(timestamp => timestamp.ToDateTime(),
                        time => time.ToUniversalTime().ToTimestamp());
                builder.Property(route => route.TravelDays)
                    .HasColumnName("TravelDays2");
                builder.Property(route => route.TripType)
                    .HasColumnName("TripType2");
                builder.Property(route => route.DepartureCity)
                    .HasColumnName("DepartureCity2");
            });

            var touristRouteJsonData = File.ReadAllText(
                @".\Data\touristRoutesMockData.json");

            //修改JSON种子数据
            JArray jArray = JArray.Parse(touristRouteJsonData);
            var jObjects = jArray.ToObject<List<JObject>>();
            foreach (var obj in jObjects)
            {

                foreach (var prop in obj.Properties())
                {
                    if (prop.Name == "travelDays" || prop.Name == "tripType" || prop.Name == "departureCity")
                    {
                        if (obj[prop.Name] != null)
                        {
                            obj[prop.Name] = (int)obj[prop.Name] + 1;
                        }
                    }
                }
            }
            JArray outputArray = JArray.FromObject(jObjects);
            touristRouteJsonData = outputArray.ToString();
            //File.WriteAllText(@".\Data\touristRoutesMockData2.json",touristRouteJsonData);

            var touristRoutes
                = JsonConvert.DeserializeObject<IList<TouristRoute>>(touristRouteJsonData);
            modelBuilder.Entity<TouristRoute>().HasData(touristRoutes);

            base.OnModelCreating(modelBuilder);
        }
    }
}

