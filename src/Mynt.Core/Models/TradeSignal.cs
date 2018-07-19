using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mynt.Core.Enums;
using Mynt.Core.Interfaces;

namespace Mynt.Core.Models
{
    public class TradeSignal
    {
        public Guid Id { get; set; }

        public Guid ParentId { get; set; }

        public string MarketName { get; set; }
        public string QuoteCurrency { get; set; }
        public string BaseCurrency { get; set; }
        public decimal Price { get; set; }
        public TradeAdvice TradeAdvice { get; set; }
        public Candle SignalCandle { get; set; }
		public ITradingStrategy Strategy { get; internal set; }

        public string StrategyName { get; set; }

        public decimal Profit { get; set; }
        public decimal PercentageProfit { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
