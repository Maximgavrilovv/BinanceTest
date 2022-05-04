using BinanceApiTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BinanceApiTest.BinanceApi;
using System.Dynamic;
using Hangfire;
using BinanceApiTest.Data;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Threading;
using Microsoft.EntityFrameworkCore.Storage;
using Hangfire.Storage;

namespace BinanceApiTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public const string SessionKeyJobId = "JobId";
        public const string SessionKeyPairId = "PairId";
        public const string SessionKeySecondsId = "SecondsId";

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task RunBinanceTrackerAsync(List<string> pairList, int seconds, CancellationToken cancellationToken)
        {
            var tradePriceList = new List<double>();
            var tradeSymbolList = new List<string>();
            var api = new BinanceApi.BinanceApi();

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                tradePriceList.Clear();
                tradeSymbolList.Clear();
                await api.GetBinanceData(tradePriceList, tradeSymbolList, seconds, pairList);

                for (int i = 0; i < tradeSymbolList.Count; i++) {
                    var binanceRecord = new BinanceRecord()
                    {
                        Pair = tradeSymbolList[i],
                        Price = tradePriceList[i]
                    };

                    _context.Add(binanceRecord);
                }

                await _context.SaveChangesAsync();
            }
        }

        public IActionResult Index()
        {
            var seconds = HttpContext.Session.GetInt32(SessionKeySecondsId);
            var pairPostString = HttpContext.Session.GetString(SessionKeyPairId);

            var api = JobStorage.Current.GetMonitoringApi();
            var processingJobs = api.ProcessingJobs(0, 100);

            foreach (var prevJob in processingJobs)
            {
                BackgroundJob.Delete(prevJob.Key);
            }

            if (seconds == null)
            {
                seconds = 10;
                ViewBag.Seconds = 10;
            } else
            {
                ViewBag.Seconds = seconds;
            }

            if (pairPostString == null)
            {
                pairPostString = "btcusdt, ethbtc";
                ViewBag.Pairs = "btcusdt, ethbtc";
            } else
            {
                ViewBag.Pairs = pairPostString;
            }

            string[] pairArray = pairPostString.Replace(" ", "").Split(",");
            var pairList = pairArray.ToList<string>();

            var jobId = int.Parse(BackgroundJob.Enqueue(() => RunBinanceTrackerAsync(pairList, seconds.Value, CancellationToken.None)));

            return View();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UserSettings userSettings)
        {
            if (ModelState.IsValid)
            {
                HttpContext.Session.SetString(SessionKeyPairId, userSettings.Pair);
                HttpContext.Session.SetInt32(SessionKeySecondsId, userSettings.Seconds);
            }
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
