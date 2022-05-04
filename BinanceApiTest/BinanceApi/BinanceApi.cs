using Binance.Client.Websocket;
using Binance.Client.Websocket.Client;
using Binance.Client.Websocket.Subscriptions;
using Binance.Client.Websocket.Websockets;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceApiTest.BinanceApi
{
    public class BinanceApi
    {
        public async Task GetBinanceData(List<double> tradePriceList, List<string> tradeSymbolList, int seconds, List<string> pairList)
        {
            var exitEvent = new ManualResetEvent(false);
            var url = BinanceValues.ApiWebsocketUrl;

            using (var communicator = new BinanceWebsocketCommunicator(url))
            {
                using (var client = new BinanceWebsocketClient(communicator))
                {
                    List<SubscriptionBase> subList = new List<SubscriptionBase>();

                    foreach (var pair in pairList)
                    {
                        subList.Add(new TradeSubscription(pair));
                    }

                    client.SetSubscriptions(subList.ToArray());

                    client.Streams.TradesStream.Subscribe(response =>
                    {
                        var trade = response.Data;
                        tradeSymbolList.Add(trade.Symbol);
                        tradePriceList.Add(trade.Price);
                        Console.WriteLine($"Trade executed [{trade.Symbol}] price: {trade.Price}");
                    });

                    
                    await communicator.Start();

                    exitEvent.WaitOne(TimeSpan.FromSeconds(seconds));
                }
            }
        }
    }
}
