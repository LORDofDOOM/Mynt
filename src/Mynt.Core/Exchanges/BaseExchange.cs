using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeSharp;
using Microsoft.Extensions.Configuration;
using Mynt.Core.Enums;
using Mynt.Core.Extensions;
using Mynt.Core.Interfaces;
using Mynt.Core.Models;

namespace Mynt.Core.Exchanges
{
    public class BaseExchangeInstance
    {
        public BaseExchange BaseExchange(string exchange)
        {
            IExchangeAPI _api = ExchangeAPI.GetExchangeAPI(exchange.ToLower());
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true);
            IConfiguration Configuration = builder.Build();

            ExchangeOptions ExchangeOptions = new ExchangeOptions();
            ExchangeOptions.Exchange = (Exchange)Enum.Parse(typeof(Exchange), exchange, true);

            string apiKey;
            string apiSecret;

            //Check if there are multiple exchanges in config, else Fallback to single mode
            if (Configuration.GetSection("Exchanges").GetSection(exchange) != null)
            {
                apiKey = Configuration.GetSection("Exchanges").GetSection(exchange).GetValue<string>("ApiKey");
                apiSecret = Configuration.GetSection("Exchanges").GetSection(exchange).GetValue<string>("ApiSecret");

            }
            else
            {
                apiKey = Configuration.GetValue<string>("ApiKey");
                apiSecret = Configuration.GetValue<string>("ApiSecret");
            }

            ExchangeOptions.Exchange = (Exchange)Enum.Parse(typeof(Exchange), exchange, true);
            ExchangeOptions.ApiKey = apiKey;
            ExchangeOptions.ApiSecret = apiSecret;

            return new BaseExchange(ExchangeOptions);
        }
    }

    public class BaseExchange : IExchangeApi
    {
        private readonly ExchangeSharp.ExchangeAPI _api;
        private readonly Exchange _exchange;
        private List<ExchangeSharp.ExchangeMarket> _exchangeInfo;

        public BaseExchange(ExchangeOptions options)
        {
            _exchange = options.Exchange;

            switch (_exchange)
            {
                case Exchange.Binance:
                    _api = new ExchangeSharp.ExchangeBinanceAPI();
                    break;
                case Exchange.Bitfinex:
                    _api = new ExchangeSharp.ExchangeBitfinexAPI();
                    break;
                case Exchange.Bittrex:
                    _api = new ExchangeSharp.ExchangeBittrexAPI();
                    break;
                case Exchange.Poloniex:
                    _api = new ExchangeSharp.ExchangePoloniexAPI();
                    break;
                case Exchange.Huobi:
                    _api = new ExchangeSharp.ExchangeHuobiAPI();
                    break;
                case Exchange.HitBtc:
                    _api = new ExchangeSharp.ExchangeHitbtcAPI();
                    break;
                case Exchange.Gdax:
                    _api = new ExchangeSharp.ExchangeGdaxAPI();
                    break;
                case Exchange.Okex:
                    _api = new ExchangeSharp.ExchangeOkexAPI();
                    break;
                case Exchange.Cryptopia:
                    _api = new ExchangeSharp.ExchangeCryptopiaAPI();
                    break;
                case Exchange.Kucoin:
                    _api = new ExchangeSharp.ExchangeKucoinAPI();
                    break;               
            }

            _api.LoadAPIKeysUnsecure(options.ApiKey, options.ApiSecret, options.PassPhrase);
        }

        public BaseExchange(ExchangeOptions options, ExchangeAPI exchangeAPI)
        {
            _exchange = options.Exchange;
            _api = exchangeAPI;
        }

        #region default implementations

        public async Task<string> Buy(string market, decimal quantity, decimal rate)
        {
            var request = new ExchangeSharp.ExchangeOrderRequest()
            {
                Amount = quantity,
                IsBuy = true,
                OrderType = ExchangeSharp.OrderType.Limit,
                Price = rate,
                Symbol = market
            };

            var order = await _api.PlaceOrderAsync(request);

            return order.OrderId;
        }

        public async Task CancelOrder(string orderId, string market)
        {
            await _api.CancelOrderAsync(orderId, market);
        }

        public async Task<AccountBalance> GetBalance(string currency)
        {
            var balances = await _api.GetAmountsAvailableToTradeAsync();

            if (balances.ContainsKey(currency))
                return new AccountBalance(currency, balances[currency], 0);
            else
                return new AccountBalance(currency, 0, 0);
        }

        public async Task<List<Models.MarketSummary>> GetMarketSummaries(string quoteCurrency)
        {
            if (_exchange == Exchange.Huobi || _exchange == Exchange.Okex || _exchange == Exchange.Gdax)
                return await GetExtendedMarketSummaries(quoteCurrency);

            var summaries = _api.GetTickersAsync().Result;

            if (summaries.Any())
            {
                var result = new List<Models.MarketSummary>();

                foreach (var summary in summaries)
                {
                    var info = await GetSymbolInfo(summary.Key);
                    result.Add(new Models.MarketSummary()
                    {
                        CurrencyPair = new CurrencyPair() { BaseCurrency = info.MarketCurrency, QuoteCurrency = info.BaseCurrency },
                        MarketName = summary.Key,
                        Ask = summary.Value.Ask,
                        Bid = summary.Value.Bid,
                        Last = summary.Value.Last,
                        Volume = summary.Value.Volume.ConvertedVolume,
                    });
                }

                return result;
            }

            return new List<Models.MarketSummary>();
        }

        public async Task<List<OpenOrder>> GetOpenOrders(string market)
        {
            var orders = await _api.GetOpenOrderDetailsAsync(market);

            if (orders.Count() > 0)
            {
                return orders.Select(x => new OpenOrder
                {
                    Exchange = _exchange.ToString(),
                    OriginalQuantity = x.Amount,
                    ExecutedQuantity = x.AmountFilled,
                    OrderId = x.OrderId,
                    Side = x.IsBuy ? OrderSide.Buy : OrderSide.Sell,
                    Market = x.Symbol,
                    Price = x.Price,
                    OrderDate = x.OrderDate,
                    Status = x.Result.ToOrderStatus()
                }).ToList();
            }

            return new List<OpenOrder>();
        }

        public async Task<Order> GetOrder(string orderId, string market)
        {
            ExchangeOrderResult order = new ExchangeOrderResult();

            try
            {
                order = await _api.GetOrderDetailsAsync(orderId, market);
            }
            catch (Exception ex)
            {
                Console.WriteLine(orderId.ToString());
                Console.WriteLine(ex.ToString());
            }

            if (order != null)
            {
                return new Order
                {
                    Exchange = _exchange.ToString(),
                    OriginalQuantity = order.Amount,
                    ExecutedQuantity = order.AmountFilled,
                    OrderId = order.OrderId,
                    Price = order.Price,
                    Market = order.Symbol,
                    Side = order.IsBuy ? OrderSide.Buy : OrderSide.Sell,
                    OrderDate = order.OrderDate,
                    Status = order.Result.ToOrderStatus()
                };
            }

            return null;
        }

        public async Task<OrderBook> GetOrderBook(string market)
        {
            var orderbook = await _api.GetOrderBookAsync(market);
            var orderbookFixed = new OrderBook
            {
                Asks = orderbook.Asks.Select(x => new OrderBookEntry { Price = x.Value.Price, Quantity = x.Value.Amount }).ToList(),
                Bids = orderbook.Bids.Select(x => new OrderBookEntry { Price = x.Value.Price, Quantity = x.Value.Amount }).ToList()
            };

            return orderbookFixed;
        }

        public async Task<Ticker> GetTicker(string market)
        {
            var ticker = await _api.GetTickerAsync(market);

            if (ticker != null)
                return new Ticker
                {
                    Ask = ticker.Ask,
                    Bid = ticker.Bid,
                    Last = ticker.Last,
                    Volume = ticker.Volume.ConvertedVolume
                };

            return null;
        }

        public async Task<List<Candle>> GetTickerHistory(string market, Period period, DateTime startDate, DateTime? endDate = null)
        {
            IEnumerable<MarketCandle> ticker = new List<MarketCandle>();

            int k = 1;

            while (ticker.Count() <= 0 && k < 20)
            {
                try
                {
                    ticker = await _api.GetCandlesAsync(market, period.ToMinutesEquivalent() * 60, startDate, endDate);
                }
                catch (Exception)
                {
                    Thread.Sleep(5000);
                }
            }

            if (ticker.Any())
                return ticker.Select(x => new Candle
                {
                    Close = x.ClosePrice,
                    High = x.HighPrice,
                    Low = x.LowPrice,
                    Open = x.OpenPrice,
                    Timestamp = x.Timestamp,
                    Volume = (decimal)x.ConvertedVolume
                }).ToList();

            return new List<Candle>();
        }

        public async Task<List<Candle>> GetTickerHistory(string market, Period period, int length)
        {
            var ticker = await _api.GetCandlesAsync(market, period.ToMinutesEquivalent() * 60, null, null, length);

            if (ticker.Any())
                return ticker.Select(x => new Candle
                {
                    Close = x.ClosePrice,
                    High = x.HighPrice,
                    Low = x.LowPrice,
                    Open = x.OpenPrice,
                    Timestamp = x.Timestamp,
                    Volume = (decimal)x.ConvertedVolume
                }).ToList();

            return new List<Candle>();
        }

        public async Task<List<Candle>> GetChunkTickerHistory(string market, Period period, DateTime startDate, DateTime? endDate = null)
        {
            var totaltickers = new List<MarketCandle>();

            var hh = 840;

            //gdax needs a small granularity
            if (_exchange == Exchange.Gdax)
                hh = 24;

            var cendDate = endDate;
            if (endDate > startDate.AddHours(hh))
                cendDate = startDate.AddHours(hh);

            while (cendDate <= endDate)
            {
                int k = 0;

                IEnumerable<MarketCandle> ticker = new List<MarketCandle>();
                while (ticker.Count() <= 0 && k < 10)
                {
                    k++;
                    Console.WriteLine("GetCandlesAsync:" + market + " / " + startDate + " / " + cendDate);
                    try
                    {
                        ticker = await _api.GetCandlesAsync(market, period.ToMinutesEquivalent() * 60, startDate, cendDate);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERROR GetCandlesAsync:" + market + " / " + startDate + " / " + cendDate + " / " + ex);
                        Thread.Sleep(3000);
                    }
                }

                totaltickers.AddRange(ticker);

                startDate = cendDate.Value;
                cendDate = cendDate.Value.AddHours(hh);
            }

            if (totaltickers.Any())
            {
                var totalCandles = totaltickers.Select(x => new Candle
                {
                    Close = x.ClosePrice,
                    High = x.HighPrice,
                    Low = x.LowPrice,
                    Open = x.OpenPrice,
                    Timestamp = x.Timestamp,
                    Volume = (decimal)x.ConvertedVolume
                }).ToList();

                // totalCandles = await FillCandleGaps(totalCandles, period);

                return totalCandles.OrderBy(x => x.Timestamp).ToList();
            }

            return new List<Candle>();
        }

        /// <summary>
        /// For candle data with inconsistent intervals 
        ///   (ie., for coins that don't have activity between two periods),
        ///   filling the gaps by extending the candle preceeding the gap until the next candle.
        /// This is usually more of an issue with low volume coins and shortewr time intervals.
        /// </summary>
        /// <param name="candles">Candle list containing time gaps.</param>
        /// <param name="period">Period of candle.</param>
        /// <returns></returns>
        public async Task<List<Candle>> FillCandleGaps(List<Candle> candles, Period period)
        {
            // Candle response
            var filledCandles = new List<Candle>();
            var orderedCandles = candles.OrderBy(x => x.Timestamp).ToList();

            // Datetime variables
            DateTime nextTime;
            DateTime startDate = orderedCandles.First().Timestamp;
            DateTime endDate = DateTime.UtcNow;

            // Walk through the candles and fill any gaps
            for (int i = 0; i < orderedCandles.Count() - 1; i++)
            {
                var c1 = orderedCandles[i];
                var c2 = orderedCandles[i + 1];
                filledCandles.Add(c1);
                nextTime = c1.Timestamp.AddMinutes(period.ToMinutesEquivalent());
                while (nextTime < c2.Timestamp)
                {
                    var cNext = c1;
                    cNext.Timestamp = nextTime;
                    filledCandles.Add(cNext);
                    nextTime = cNext.Timestamp.AddMinutes(period.ToMinutesEquivalent());
                }
            }

            // Fill "extend" the last candle gap
            var cLast = candles.Last();
            filledCandles.Add(cLast);
            nextTime = cLast.Timestamp.AddMinutes(period.ToMinutesEquivalent());
            while (nextTime < endDate)
            {
                var cNext = cLast;
                cNext.Timestamp = nextTime;
                filledCandles.Add(cNext);
                nextTime = cNext.Timestamp.AddMinutes(period.ToMinutesEquivalent());
            }

            // Debugging. Don't need when running for real.
            await Task.Delay(10);

            // Return the no-gap candles
            return filledCandles;
        }

        public async Task<string> Sell(string market, decimal quantity, decimal rate)
        {
            var request = new ExchangeSharp.ExchangeOrderRequest()
            {
                Amount = quantity,
                IsBuy = false,
                OrderType = ExchangeSharp.OrderType.Limit,
                Price = rate,
                Symbol = market
            };
            try
            {
                var order = await _api.PlaceOrderAsync(request);
                return order.OrderId;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return null;

        }

        public async Task<ExchangeMarket> GetSymbolInfo(string symbol)
        {
            if (_exchangeInfo == null)
            {
                var result = (await _api.GetSymbolsMetadataAsync()).ToList();
                _exchangeInfo = result;
            }

            return _exchangeInfo.FirstOrDefault(x => x.MarketName == symbol);
        }

        public async Task<string> GlobalSymbolToExchangeSymbol(string symbol)
        {
            return _api.GlobalSymbolToExchangeSymbol(symbol);
        }

        public async Task<string> ExchangeCurrencyToGlobalCurrency(string symbol)
        {
            return _api.ExchangeSymbolToGlobalSymbol(symbol);
        }

        public async Task<ExchangeAPI> GetFullApi()
        {
            return _api;
        }

        #endregion

        #region non-default implementations

        private async Task<List<Models.MarketSummary>> GetExtendedMarketSummaries(string quoteCurrency)
        {
            var summaries = new List<Models.MarketSummary>();
            var symbols = await _api.GetSymbolsMetadataAsync();
            var list = await _api.GetSymbolsAsync();
            var filteredList = list.Where(x => x.ToLower().EndsWith(quoteCurrency.ToLower(), StringComparison.Ordinal));

            foreach (var item in filteredList)
            {
                var ticker = await _api.GetTickerAsync(item);

                if (ticker == null)
                    continue;

                var symbol = symbols.FirstOrDefault(x => x.MarketName == item);

                if (symbol != null)
                {
                    summaries.Add(new Models.MarketSummary()
                    {
                        CurrencyPair = new CurrencyPair() { BaseCurrency = symbol.MarketCurrency, QuoteCurrency = symbol.BaseCurrency },
                        MarketName = item,
                        Ask = ticker.Ask,
                        Bid = ticker.Bid,
                        Last = ticker.Last,
                        Volume = ticker.Volume.ConvertedVolume,
                    });
                }
            }

            return summaries;
        }

        #endregion

    }
}
