using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Mynt.Backtester.Models;
using Mynt.Core.Models;


namespace Mynt.Data.LiteDB
{
    public static class Mapping
    {
        private static readonly Lazy<IMapper> Lazy = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                // This line ensures that internal properties are also mapped over.
                cfg.ShouldMapProperty = p => p.GetMethod.IsPublic || p.GetMethod.IsAssembly;
                cfg.AddProfile<MappingProfile>();
            });

            var mapper = config.CreateMapper();
            return mapper;
        });

        public static IMapper Mapper => Lazy.Value;
    }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TradeAdapter, Trade>();
            CreateMap<Trade, TradeAdapter>();

            CreateMap<TraderAdapter, Trader>();
            CreateMap<Trader, TraderAdapter>();

            CreateMap<CandleAdapter, Candle>();
            CreateMap<Candle, CandleAdapter>();

            CreateMap<TradeSignal, TradeSignalAdapter>();
            CreateMap<TradeSignalAdapter, TradeSignal>();
        }
    }
}
