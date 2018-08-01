using ExchangeSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mynt.Core.Exchanges
{
    public class ExchangeBacktestGdaxAPI : ExchangeAPI
    {
        public override string BaseUrl { get => "local"; set => throw new NotImplementedException(); }

        public override string Name => "ExchangeBacktestGdaxAPI";

        public async Task<IEnumerable<KeyValuePair<string, ExchangeTicker>>> GetTickersAsync()
        {
            var tickers = new List<KeyValuePair<string, ExchangeTicker>>();
            var listOfMakert = new List<string>();
            foreach (var market in listOfMakert)
            {
                string symbol = "".ToStringInvariant();
                var ticker = new ExchangeTicker()
                {

                };

                tickers.Add(new KeyValuePair<string, ExchangeTicker>(symbol, ticker));
            }
            return tickers;
        }

        public async Task<IEnumerable<ExchangeOrderResult>> GetOpenOrderDetailsAsync(string symbol = null)
        {
            var orders = new List<ExchangeOrderResult>();

            var o = new ExchangeOrderResult()
            {

            };

            return orders;
        }

        public async Task<ExchangeOrderResult> GetOrderDetailsAsync(string orderId, string symbol = null)
        {
            var result = new ExchangeOrderResult()
            {
                OrderId = "",
                Result = ExchangeAPIOrderResult.Filled,
                Message = "",
                Amount = 0m,
                AmountFilled = 0m,
                Price = 0m,
                AveragePrice = 0m,
                OrderDate = DateTime.Now,
                Symbol = "XXXBTC",
                IsBuy = true,
                Fees = 0m,
                FeesCurrency = ""
            };

            return result;
        }

        public async Task<ExchangeOrderBook> GetOrderBookAsync(string symbol, int maxCount = 100)
        {
            var result = new ExchangeOrderBook()
            {

            };

            return result;
        }

        public async Task<ExchangeTicker> GetTickerAsync(string symbol)
        {
            var result = new ExchangeTicker()
            {
                Id = "",
                Bid = 0m,
                Ask = 0m,
                Last = 0m,
                Volume = new ExchangeVolume
                {
                    BaseVolume = 0m.ConvertInvariant<decimal>(),
                    BaseSymbol = symbol,
                    ConvertedVolume = 0m.ConvertInvariant<decimal>(),
                    ConvertedSymbol = symbol,
                    Timestamp = CryptoUtility.UnixTimeStampToDateTimeMilliseconds(0m.ConvertInvariant<long>())
                }
            };
            return result;
        }

        public async Task<IEnumerable<MarketCandle>> GetCandlesAsync(string symbol, int periodSeconds, DateTime? startDate = null, DateTime? endDate = null, int? limit = null)
        {
            var candles = new List<MarketCandle>();

            var obj = new List<string>();

            foreach (object array in obj)
            {
                candles.Add(new MarketCandle
                {
                    ClosePrice = 0m.ConvertInvariant<decimal>(),
                    ExchangeName = Name,
                    HighPrice = 0m.ConvertInvariant<decimal>(),
                    LowPrice = 0m.ConvertInvariant<decimal>(),
                    Name = symbol,
                    OpenPrice = 0m.ConvertInvariant<decimal>(),
                    PeriodSeconds = periodSeconds,
                    Timestamp = CryptoUtility.UnixTimeStampToDateTimeMilliseconds(DateTime.Now.ConvertInvariant<long>()),
                    BaseVolume = 0.ConvertInvariant<double>(),
                    ConvertedVolume = 0.ConvertInvariant<double>(),
                    WeightedAverage = 0m
                });
            }

            return candles;
        }

        public async Task<ExchangeOrderResult> PlaceOrderAsync(ExchangeOrderRequest order)
        {
            var result = new ExchangeOrderResult()
            {
                OrderId = "",
                Result = ExchangeAPIOrderResult.Filled,
                Message = "",
                Amount = 0m,
                AmountFilled = 0m,
                Price = 0m,
                AveragePrice = 0m,
                OrderDate = DateTime.Now,
                Symbol = "XXXBTC",
                IsBuy = true,
                Fees = 0m,
                FeesCurrency = "",
            };
            return result;
        }

        public async Task<IEnumerable<ExchangeMarket>> GetSymbolsMetadataAsync()
        {
            var markets = new List<ExchangeMarket>()
            {
              new ExchangeMarket()
              {
                   MarketName = "",
                IsActive = true,
                BaseCurrency = "",
                MarketCurrency = "",
                MinTradeSize = 0,
                MaxTradeSize = 0,
                MinPrice = 0,
                MaxPrice = 0,
                PriceStepSize = 0,
                QuantityStepSize = 0
              }
            };

            return markets;
        }

        public virtual string GlobalSymbolToExchangeSymbol(string symbol)
        {
            return string.Empty;
        }

        public virtual string ExchangeSymbolToGlobalSymbol(string symbol)
        {
            return string.Empty;
        }
    }
}
