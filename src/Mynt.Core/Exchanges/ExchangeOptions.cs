using Mynt.Core.Enums;
using System;

namespace Mynt.Core.Exchanges
{
    public class ExchangeOptions
    {
        public Exchange Exchange { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public string PassPhrase { get; set; }

        public DateTime SimulationCurrentDate { get; set; }
        public bool IsSimulation { get; set; }
        public string SimulationCandleSize { get; set; } = "15";
    }
}
