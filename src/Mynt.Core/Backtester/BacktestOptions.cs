using System;
using System.Collections.Generic;
using System.IO;

namespace Mynt.Core.Backtester
{
    public class BacktestOptions
    {
		public Enums.Exchange Exchange { get; set; } = Enums.Exchange.Binance;
        public string DataFolder { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
        public decimal StakeAmount { get; set; } = 0.005m;
        public bool OnlyStartNewTradesWhenSold { get; set; } = true;
        public List<string> Coins { get; set; } = new List<string>();
        public string Coin { get; set; } = null;
        public int CandlePeriod { get; set; } = 15;
        public bool UpdateCandles { get; set; } = true;
        public DateTime StartDate { get; set; } = new DateTime(2018,01,01);
        public DateTime EndDate { get; set; } = DateTime.MinValue;
    }
}
