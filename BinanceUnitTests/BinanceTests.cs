using Microsoft.VisualStudio.TestTools.UnitTesting;
using BinanceApiTest.BinanceApi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BinanceUnitTests
{
    [TestClass]
    public class BinanceTests
    {
        [TestMethod]
        public async Task TestBinanceApi()
        {
            var tradePriceList = new List<double>();
            var tradeSymbolList = new List<string>();
            var api = new BinanceApi();
            var seconds = 15;
            var pairList = new List<string>()
            {
                "btcusdt",
                "ethbtc"
            };

            await api.GetBinanceData(tradePriceList, tradeSymbolList, seconds, pairList);
            Assert.IsTrue(tradeSymbolList.Count > 0 && tradePriceList.Count > 0, "No data found!");
            Assert.IsTrue(tradePriceList.Count == tradeSymbolList.Count);
        }
    }
}
