using System.Collections.Generic;
using System.Linq;
using Mynt.Core.Enums;
using Mynt.Core.Extensions;
using Mynt.Core.Indicators;
using Mynt.Core.Interfaces;
using Mynt.Core.Models;

namespace Mynt.Core.Strategies
{
    public class SmaCrossoverEVO : BaseStrategy
    {
        public override string Name => "SMA Crossover EVO";
        public override int MinimumAmountOfCandles => 60;
        public override Period IdealPeriod => Period.QuarterOfAnHour;

        public override List<TradeAdvice> Prepare(List<Candle> candles)
        {
            var result = new List<TradeAdvice>();

            var shorts = candles.Ema(6);
            var longs = candles.Ema(3);

            for (int i = 0; i < candles.Count; i++)
            {
                var diff = 100 * (shorts[i] - longs[i]) / ((shorts[i] + longs[i]) / 2);

                if (diff > 1.5m)
                {
                    result.Add((TradeAdvice.Buy));
                }
                else if (diff <= -0.1m)
                {
                    result.Add((TradeAdvice.Sell));
                }
                else
                {
                    result.Add((TradeAdvice.Hold));
                }
            }
            return result;
        }

        public override Candle GetSignalCandle(List<Candle> candles)
        {
            return candles.Last();
        }

        public override TradeAdvice Forecast(List<Candle> candles)
        {
            return Prepare(candles).LastOrDefault();
        }
    }
}
