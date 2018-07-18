using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Mynt.Core.Enums;
using Mynt.Core.Indicators;
using Mynt.Core.Models;

namespace Mynt.Core.Strategies
{
    public class FreqTrade : BaseStrategy
    {
		private ILogger _logger;

        public override string Name => "FreqTrade";
        public override int MinimumAmountOfCandles => 40;
        public override Period IdealPeriod => Period.QuarterOfAnHour;

        public override List<TradeAdvice> Prepare(List<Candle> candles)
        {
            var result = new List<TradeAdvice>();

            var rsi = candles.Rsi(14);
            var adx = candles.Adx(14);
            var plusDi = candles.PlusDI(14);
            var minusDi = candles.MinusDI(14);
            var fast = candles.StochFast();

            for (int i = 0; i < candles.Count; i++)
            {
                if (
                    rsi[i] < 25
                    && fast.D[i] < 30
                    && adx[i] > 30
                    && plusDi[i] > 5
                    )
                    result.Add(TradeAdvice.Buy);

                else if (
                    adx[i] > 0
                    && minusDi[i] > 0
                    && fast.D[i] > 65
                    )
                    result.Add(TradeAdvice.Sell);

                else
                    result.Add(TradeAdvice.Hold);
            }

            if (_logger != null)
            {
                try
                {
                    _logger.LogInformation("{Name} " +
                                           "rsi:{rsi} ," +
                                           "fast.D:{f} ," +
                                           "adx:{a} ," +
                                           "plusDi:{p} ",
                                           Name,
                                           rsi[candles.Count - 1],
                                           fast.D[candles.Count - 1],
                                           adx[candles.Count - 1],
                                           plusDi[candles.Count - 1]);

                }
                catch (Exception ex)
                {
                    
                }
            }

            return result;
        }

        public override Candle GetSignalCandle(List<Candle> candles)
        {
            return candles.Last();
        }

        public override TradeAdvice Forecast(List<Candle> candles, ILogger logger)
        {
            _logger = logger;
            return Prepare(candles).LastOrDefault();
        }
    }
}
