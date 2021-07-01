using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GrpcWebBlazorWasm.Server.Data;
using GrpcWebBlazorWasm.Shared;

namespace GrpcWebBlazorWasm.Server.Services

{
    public class TouristRouteSevice :
        TouristRouteProtoSevice.TouristRouteProtoSeviceBase
    {
        private readonly ApplicationDbContext _dbContext;

        public TouristRouteSevice(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<TouristRoute> GetTouristRoute(TouristRouteId request,
            ServerCallContext context)
        {
            var touristRoute =
                await _dbContext.TouristRoutes.SingleOrDefaultAsync(route => route.Id == request.Id);

            //touristRoute.Price = touristRoute.OriginalPrice *(decimal) (touristRoute.DiscountPresent??1);  
            //
            touristRoute.TouristRouteAppend();

            return touristRoute;
        }

        public override async Task<TouristRoutes> GetTouristRoutes(Empty request, ServerCallContext context)
        {
            TouristRoutes touristRoutes = new();
            var touristRoutesFrom = await _dbContext.TouristRoutes.ToListAsync();

            //touristRoutesFrom.ForEach(touristRoute => touristRoute.Price = touristRoute.OriginalPrice * (decimal)(touristRoute.DiscountPresent ?? 1));   
            //
            touristRoutesFrom.ForEach(touristRoute => touristRoute.TouristRouteAppend());

            touristRoutes.TouristRouteList.AddRange(touristRoutesFrom);

            return touristRoutes;
        }

        public override async Task<AffNumbers> RemoveTouristRoute(TouristRoute request, ServerCallContext context)
        {
            _dbContext.Remove(request);
            int i = _dbContext.SaveChanges();
            Console.WriteLine("删除了：" + request.Title);
            Console.WriteLine("ID  ：" + request.Id);
            return new AffNumbers
            {
                Numbers = i
            };
        }

        public override async Task<AffNumbers> ModTouristRoute(TouristRoute request, ServerCallContext context)
        {
            Console.WriteLine("接收到：" + request.Title);
            _dbContext.Update(request);
            int i= _dbContext.SaveChanges();
            Console.WriteLine("修改了：" + request.Title);
            Console.WriteLine("ID  ：" + request.Id);
            return new AffNumbers
            {
                Numbers = i
            };
        }

        public override async Task<AffNumbers> AddTouristRoute(TouristRoute request, ServerCallContext context)
        {
            Console.WriteLine(request.Title);
            _dbContext.Add(request);
            
            int i = _dbContext.SaveChanges();
            Console.WriteLine("增加了：" + request.Title);
            Console.WriteLine("ID  ：" + request.Id);
            Console.WriteLine("城市  ：" + request.DepartureCity);
            return new AffNumbers
            {
                Numbers = i
            };
        }

    }
}
