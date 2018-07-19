using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Mynt.Core.Enums;
using Mynt.Core.Interfaces;
using Mynt.Core.Models;

namespace Mynt.Data.MongoDB
{
    public class TradeSignalAdapter
    {
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
