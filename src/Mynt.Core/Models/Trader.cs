using System;

namespace Mynt.Core.Models
{
    public class Trader
    {
        // Used as primary key for the different data storage mechanisms.
        public int Id { get; set; }

        public string Identifier { get; set; }
        public decimal StakeAmount { get; set; }
        public decimal CurrentBalance { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsBusy { get; set; }
        public bool IsArchived { get; set; }
        public bool IsPaperTrade { get; set; }

        // Used for MyntUI output
        public Trade ActiveTrade { get; set; }
        public decimal ProfitLoss { get; set; }
    }
}
