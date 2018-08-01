using System;
using System.Collections.Generic;
using System.Text;
using LiteDB;
using Mynt.Core.Enums;
using Mynt.Core.Interfaces;
using Mynt.Core.Models;

namespace Mynt.Data.LiteDB
{
    public class TradeSignalAdapter
    {
        [BsonId]
        public Guid Id { get; set; }

        public Guid ParentId { get; set; }

        public string MarketName { get; set; }
        public string QuoteCurrency { get; set; }
        public string BaseCurrency { get; set; }
        public decimal Price { get; set; }
        public TradeAdvice TradeAdvice { get; set; }
        public CandleAdapter SignalCandle { get; set; }

        public string StrategyName { get; internal set; }
        public decimal Profit { get; set; }
        public decimal PercentageProfit { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
