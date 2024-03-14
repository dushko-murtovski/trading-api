using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StockwolfTrading.ViewModel;
using StockWolfTrading.Core.Models;
using StockWolfTrading.Services.Interfaces;
using Alpaca.Markets;
using Microsoft.AspNetCore.Http;

namespace StockWolfTradingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TradingController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITickerService _tickerService;
        private readonly ITradeService _tradeService;
        private readonly IDailyAnalysisService _dailyAnalysisService;
        private readonly ISettingsService _settingsService;
        public TradingController(IUserService userService, ITickerService tickerService, ITradeService tradeService
            , IDailyAnalysisService dailyAnalysisService, ISettingsService settingsService)
        {
            _userService = userService;
            _tickerService = tickerService;
            _tradeService = tradeService;
            _dailyAnalysisService = dailyAnalysisService;
            _settingsService = settingsService;
        }
        public string Get(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            //request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        [HttpGet("getupdatestatus")]
        public IActionResult GetUpateStatus()
        {
            var res = _settingsService.GetUpdateStatus();
            return Ok(res);
        }

        [HttpGet("forceupdate")]
        public IActionResult SetForceUpdate()
        {
            var res = _settingsService.SetForceUpdate();
            return Ok(res);
        }

        [HttpGet("getmarketdata")]
        public async Task<MarketData> GetMarketData(string ticker = "AAPL")
        {
            string API_KEY = "PKUG8TX0P2XOF8R4DFUD";

            string API_SECRET = "NI3AVS9dlaZDDnKsE25fQFr2N1fEW5WsJu1vh0Kp";

            var client = Alpaca.Markets.Environments.Paper
                .GetAlpacaDataClient(new SecretKey(API_KEY, API_SECRET));

            var into = DateTime.Today;
            var from = into.AddDays(-5);
            // Get daily price data for AAPL over the last 5 trading days.
            var bars = await client.ListHistoricalBarsAsync(
                new HistoricalBarsRequest(ticker, from, into, BarTimeFrame.Day));

            var bars1 = await client.ListNewsArticlesAsync(
                new NewsArticlesRequest(new List<string>() { ticker }));

            // See how much AAPL moved in that timeframe.
            var startPrice = bars.Items.First().Open;
            var endPrice = bars.Items.Last().Close;

            var percentChange = Math.Round(((endPrice - startPrice) / startPrice) * 100, 2);
            //Console.WriteLine($"AAPL moved {percentChange:P} over the last 5 days.");

            //Console.Read();

            MarketData marketData = new MarketData();
            marketData.Name = ticker;
            marketData.PriceChange = percentChange.ToString();
            foreach (var news in bars1.Items)
            {
                marketData.TickerNews.Add(new TickerNews()
                {
                    Author = news.Author,
                    ArticleUrl = news.ArticleUrl.ToString(),
                    Summary = news.Summary
                });
            }
            return marketData;
        }

        [HttpPost("updatetickers")]
        public IActionResult UpdateTickers([FromBody] JObject data)
        {
            var parsed = JsonConvert.DeserializeObject<Dictionary<string, string>>(data.ToString());

            var user = _userService.GetUserByUsername(parsed["email"]);
            if (user == null)
                return NotFound();

            var newtickers = JsonConvert.DeserializeObject<List<string>>(parsed["tickers"]);
            //var newtickers = new List<string>();

            var ods = user.Order.Where(o => o.IsExpired != true).OrderByDescending(o => o.DateFinished).ToList();
            var lastord = ods[0];
            var orderdet = lastord.OrderDetail.ToList()[0];
            var lasttickers = orderdet.OrderDetailsTicker.ToList();
            foreach (var ticker in lasttickers)
            {
                var newtickersF = newtickers.FirstOrDefault(t => t == ticker.TickerName.Trim());
                if (newtickersF == null)
                {
                    _tickerService.DeleteTicker(ticker.TickerName);
                }
            }
            foreach (var ticker in newtickers)
            {
                var newtickersF = lasttickers.FirstOrDefault(t => t.TickerName.Trim() == ticker);
                if (newtickersF == null)
                {
                    var tck = new OrderDetailsTicker();
                    tck.TickerName = ticker;
                    tck.OrderDetailsRefId = orderdet.Id;
                    tck.DateSelected = DateTime.Now;
                    tck.Priority = 1;
                    tck.Active = true;
                    _tickerService.AddTicker(tck);
                }
            }
            return Ok();
        }

        [HttpPost("loadstocks")]
        public IActionResult LoadStocks(JObject data)
        {
            List<ReturnTicker> lasttickers = new List<ReturnTicker>();
            try
            {
                var username = JsonConvert.DeserializeObject<Dictionary<string, string>>(data.ToString());
                var email = username["email"];
                var user = _userService.GetUserByUsername(email);
                if (user == null)
                    return NotFound();

                var ods = user.Order.Where(o => o.IsExpired != true).OrderByDescending(o => o.DateFinished).ToList();
                var lastord = ods[0];
                var orderdet = lastord.OrderDetail.ToList()[0];
                lasttickers = orderdet.OrderDetailsTicker.Select(t => new ReturnTicker() { name = t.TickerName.Trim(), dateadded = t.DateSelected }).Distinct().ToList();
            }
            catch (Exception)
            {

            }

            return Ok(lasttickers);
        }

        [HttpGet("getstocknames")]
        public ActionResult GetStockNames()
        {
            string[] tickerList = new string[] { "MMM", "ABT", "ABBV", "ABMD", "ACN", "ATVI", "ADBE", "AMD", "AAP", "AES", "AFL", "A", "APD", "AKAM", "ALK", "ALB", "ARE", "ALGN", "ALLE", "LNT", "ALL", "GOOGL", "GOOG", "MO", "AMZN", "AMCR", "AEE", "AAL", "AEP", "AXP", "AIG", "AMT", "AWK", "AMP", "ABC", "AME", "AMGN", "APH", "ADI", "ANSS", "ANTM", "AON", "AOS", "APA", "AAPL", "AMAT", "APTV", "ADM", "ANET", "AJG", "AIZ", "T", "ATO", "ADSK", "ADP", "AZO", "AVB", "AVY", "BKR", "BLL", "BAC", "BBWI", "BAX", "BDX", "BRK.B", "BBY", "BIO", "TECH", "BIIB", "BLK", "BK", "BA", "BKNG", "BWA", "BXP", "BSX", "BMY", "AVGO", "BR", "BF.B", "CHRW", "COG", "CDNS", "CZR", "CPB", "COF", "CAH", "KMX", "CCL", "CARR", "CTLT", "CAT", "CBOE", "CBRE", "CDW", "CE", "CNC", "CNP", "CERN", "CF", "CRL", "SCHW", "CHTR", "CVX", "CMG", "CB", "CHD", "CI", "CINF", "CTAS", "CSCO", "C", "CFG", "CTXS", "CLX", "CME", "CMS", "KO", "CTSH", "CL", "CMCSA", "CMA", "CAG", "COP", "ED", "STZ", "COO", "CPRT", "GLW", "CTVA", "COST", "CCI", "CSX", "CMI", "CVS", "DHI", "DHR", "DRI", "DVA", "DE", "DAL", "XRAY", "DVN", "DXCM", "FANG", "DLR", "DFS", "DISCA", "DISCK", "DISH", "DG", "DLTR", "D", "DPZ", "DOV", "DOW", "DTE", "DUK", "DRE", "DD", "DXC", "EMN", "ETN", "EBAY", "ECL", "EIX", "EW", "EA", "EMR", "ENPH", "ETR", "EOG", "EFX", "EQIX", "EQR", "ESS", "EL", "ETSY", "EVRG", "ES", "RE", "EXC", "EXPE", "EXPD", "EXR", "XOM", "FFIV", "FB", "FAST", "FRT", "FDX", "FIS", "FITB", "FE", "FRC", "FISV", "FLT", "FMC", "F", "FTNT", "FTV", "FBHS", "FOXA", "FOX", "BEN", "FCX", "GPS", "GRMN", "IT", "GNRC", "GD", "GE", "GIS", "GM", "GPC", "GILD", "GL", "GPN", "GS", "GWW", "HAL", "HBI", "HIG", "HAS", "HCA", "PEAK", "HSIC", "HSY", "HES", "HPE", "HLT", "HOLX", "HD", "HON", "HRL", "HST", "HWM", "HPQ", "HUM", "HBAN", "HII", "IEX", "IDXX", "INFO", "ITW", "ILMN", "INCY", "IR", "INTC", "ICE", "IBM", "IP", "IPG", "IFF", "INTU", "ISRG", "IVZ", "IPGP", "IQV", "IRM", "JKHY", "J", "JBHT", "SJM", "JNJ", "JCI", "JPM", "JNPR", "KSU", "K", "KEY", "KEYS", "KMB", "KIM", "KMI", "KLAC", "KHC", "KR", "LHX", "LH", "LRCX", "LW", "LVS", "LEG", "LDOS", "LEN", "LLY", "LNC", "LIN", "LYV", "LKQ", "LMT", "L", "LOW", "LUMN", "LYB", "MTB", "MRO", "MPC", "MKTX", "MAR", "MMC", "MLM", "MAS", "MA", "MKC", "MCD", "MCK", "MDT", "MRK", "MET", "MTD", "MGM", "MCHP", "MU", "MSFT", "MAA", "MRNA", "MHK", "TAP", "MDLZ", "MPWR", "MNST", "MCO", "MS", "MOS", "MSI", "MSCI", "NDAQ", "NTAP", "NFLX", "NWL", "NEM", "NWSA", "NWS", "NEE", "NLSN", "NKE", "NI", "NSC", "NTRS", "NOC", "NLOK", "NCLH", "NOV", "NRG", "NUE", "NVDA", "NVR", "NXPI", "ORLY", "OXY", "ODFL", "OMC", "OKE", "ORCL", "OGN", "OTIS", "PCAR", "PKG", "PH", "PAYX", "PAYC", "PYPL", "PENN", "PNR", "PBCT", "PEP", "PKI", "PRGO", "PFE", "PM", "PSX", "PNW", "PXD", "PNC", "POOL", "PPG", "PPL", "PFG", "PG", "PGR", "PLD", "PRU", "PTC", "PEG", "PSA", "PHM", "PVH", "QRVO", "PWR", "QCOM", "DGX", "RL", "RJF", "RTX", "O", "REG", "REGN", "RF", "RSG", "RMD", "RHI", "ROK", "ROL", "ROP", "ROST", "RCL", "SPGI", "CRM", "SBAC", "SLB", "STX", "SEE", "SRE", "NOW", "SHW", "SPG", "SWKS", "SNA", "SO", "LUV", "SWK", "SBUX", "STT", "STE", "SYK", "SIVB", "SYF", "SNPS", "SYY", "TMUS", "TROW", "TTWO", "TPR", "TGT", "TEL", "TDY", "TFX", "TER", "TSLA", "TXN", "TXT", "TMO", "TJX", "TSCO", "TT", "TDG", "TRV", "TRMB", "TFC", "TWTR", "TYL", "TSN", "UDR", "ULTA", "USB", "UAA", "UA", "UNP", "UAL", "UNH", "UPS", "URI", "UHS", "UNM", "VLO", "VTR", "VRSN", "VRSK", "VZ", "VRTX", "VFC", "VIAC", "VTRS", "V", "VNO", "VMC", "WRB", "WAB", "WMT", "WBA", "DIS", "WM", "WAT", "WEC", "WFC", "WELL", "WST", "WDC", "WU", "WRK", "WY", "WHR", "WMB", "WLTW", "WYNN", "XEL", "XLNX", "XYL", "YUM", "ZBRA", "ZBH", "ZION", "ZTS" };
            var p = Directory.GetCurrentDirectory();
            var fp = Path.Combine(p, "symbols.txt");
            if (System.IO.File.Exists(fp))
            {
                tickerList = System.IO.File.ReadAllLines(fp);
            }
            return Ok(tickerList);
        }


        [HttpGet("lastprice")]
        public ActionResult GetLastPrice(string tickers)
        {
            var tickers1 = JsonConvert.DeserializeObject<List<string>>(tickers);
            List<double> prices = new List<double>();
            List<double> prevPrices = new List<double>();
            List<double> change = new List<double>();
            foreach (var tick in tickers1)
            {
                var res = Get("https://query2.finance.yahoo.com/v8/finance/chart/" + tick + "?symbol=" + tick + "&range=1d" + "&interval=1h");// + interval.ToLower());
                JObject data = JObject.Parse(res);

                var ch = data["chart"]["result"][0];

                var history = ch.ToObject<YResultViewModel>();
                var timestamp = ch["timestamp"];
                var indicators = ch["indicators"];
                List<CandleViewModel> candles = new List<CandleViewModel>();
                for (int i = 0; i < history.timestamp.Count; i++)
                {
                    candles.Add(new CandleViewModel()
                    {
                        time = history.timestamp[i],
                        low = history.indicators.quote[0].low[i],
                        high = history.indicators.quote[0].high[i],
                        open = history.indicators.quote[0].open[i],
                        close = history.indicators.quote[0].close[i],
                        volume = history.indicators.quote[0].volume[i],
                    });
                }
                if (candles.Count > 1)
                {
                    prices.Add(candles[candles.Count - 1].open != null ? (double)candles[candles.Count - 1].open : 0);
                    prevPrices.Add(candles[candles.Count - 2].open != null ? (double)candles[candles.Count - 2].open : 0);
                }
            }
            if (prices.Count > 0)
            {
                for (int i = 0; i < prices.Count; i++)
                {
                    change.Add(Math.Round((prices[i] - prevPrices[i]) / prevPrices[i] * 100, 2));
                }
                dynamic result = new System.Dynamic.ExpandoObject();
                result.prices = prices.Select(p => Math.Round(p, 3)).ToList();
                result.change = change;
                return Ok(result);
            }
            return NotFound();
        }
        [HttpGet("history1")]
        public ActionResult GetHistory1(string name = "AAPL", string range = "", string interval = "1d", string start = "2021-09-14", string end = "2021-09-19")
        {
            var candles = CreateCandles(name, range, interval, start, end);
            dynamic result = new System.Dynamic.ExpandoObject();
            result.Aggregated = false;
            result.Data = candles;
            return Ok(result);
        }

        List<CandleViewModel> CreateCandles(string name = "AAPL", string range = "", string interval = "1d", string start = "2021-09-14", string end = "2021-09-19")
        {
            List<CandleViewModel> candles = new List<CandleViewModel>();

            if (name.EndsWith("USD"))
            {
                name = name.Substring(0, name.Length - 4);
            }

            string API_KEY = "PKUG8TX0P2XOF8R4DFUD";

            string API_SECRET = "NI3AVS9dlaZDDnKsE25fQFr2N1fEW5WsJu1vh0Kp";

            var client = Alpaca.Markets.Environments.Paper
                .GetAlpacaDataClient(new SecretKey(API_KEY, API_SECRET));

            DateTime startDate = DateTime.ParseExact(start, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            //startDate = startDate.AddHours(14);
            //startDate = startDate.AddMinutes(30);
            DateTime endDate = DateTime.ParseExact(end, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture); // DateTime.Now.AddMinutes(-16); // DateTime.ParseExact(end, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture).AddDays(-1);
            if (endDate.Date > DateTime.Now.Date)
            {
                //endDate = endDate.AddHours(-(endDate.Date - DateTime.Now.Date).TotalHours);
                endDate = DateTime.Now.AddMinutes(-16);
            }
            else if ((DateTime.Now - endDate).Minutes < 16)
            {
                endDate = DateTime.Now.AddMinutes(-16);
            }
            if (startDate.Date == DateTime.Now.Date && startDate > endDate)
            {
                startDate = endDate;
            }
            if (startDate == endDate && startDate < DateTime.Now.Date)
            {
                startDate = endDate.AddDays(-1);
            }
            //else
            //{
            //    endDate = endDate.AddHours(20);
            //    endDate = endDate.AddMinutes(30);
            //}
            var inter = BarTimeFrame.Day;

            switch (interval)
            {
                case "1D":
                    inter = BarTimeFrame.Day;
                    break;
                case "1m":
                    inter = BarTimeFrame.Minute;
                    if (startDate != endDate && endDate.Date != DateTime.Now.Date && startDate.Date > DateTime.Now.Date.AddDays(-7))
                    {
                        startDate = startDate.AddDays(-7);
                    }
                    break;
                case "2m":
                    inter = new BarTimeFrame(2, BarTimeFrameUnit.Minute);
                    if (startDate != endDate && endDate.Date != DateTime.Now.Date && startDate.Date > DateTime.Now.Date.AddDays(-7))
                    {
                        startDate = startDate.AddDays(-7);
                    }
                    break;
                case "5m":
                    inter = new BarTimeFrame(5, BarTimeFrameUnit.Minute);
                    if (startDate != endDate && endDate.Date != DateTime.Now.Date && startDate.Date > DateTime.Now.Date.AddDays(-7))
                    {
                        startDate = startDate.AddDays(-7);
                    }
                    break;
                case "15m":
                    inter = new BarTimeFrame(15, BarTimeFrameUnit.Minute);
                    if (startDate != endDate && endDate.Date != DateTime.Now.Date && startDate.Date > DateTime.Now.Date.AddDays(-7))
                    {
                        startDate = startDate.AddDays(-7);
                    }
                    break;
                case "60":
                    inter = BarTimeFrame.Hour;
                    break;
                case "120":
                    inter = new BarTimeFrame(2, BarTimeFrameUnit.Hour);
                    break;
                case "180":
                    inter = new BarTimeFrame(3, BarTimeFrameUnit.Hour);
                    break;
                case "240":
                    inter = new BarTimeFrame(4, BarTimeFrameUnit.Hour);
                    break;
            }
            HistoricalBarsRequest hist = new HistoricalBarsRequest(name, startDate, endDate, inter);
            var bars = client.ListHistoricalBarsAsync(hist).Result;

            var items = bars.Items.ToList();
            foreach (var item in items)
            {
                var tm = item.TimeUtc.ToUniversalTime();
                if (tm.Hour < 13 || tm.Hour > 20)
                {
                    continue;
                }
                var ms = new DateTimeOffset(tm).ToUnixTimeMilliseconds() / 1000;
                //System.Diagnostics.Debug.WriteLine(tm);
                var candle = new CandleViewModel()
                {
                    time = (int)ms,
                    low = (double)item.Low,
                    high = (double)item.High,
                    open = (double)item.Open,
                    close = (double)item.Close,
                    volume = (int)item.Volume,
                };
                candles.Add(candle);
            }
            if (candles.Count == 0)
            {

            }
            return candles;
        }
        [HttpGet("history")]
        public ActionResult GetHistory(string name = "AAPL", string range = "", string interval = "1d", string start = "2021-09-14", string end = "2021-09-19")
        {
            string res = "";
            if (name.EndsWith("USD"))
            {
                name = name.Substring(0, name.Length - 4);
            }
            if (range != "")
            {
                res = Get("https://query2.finance.yahoo.com/v8/finance/chart/" + name + "?" + name + "&range=" + range + "&interval=" + interval.ToLower());
            }
            else
            {
                var ds1 = start.Split('-').ToList();
                var ds2 = end.Split('-').ToList();
                var d1 = new DateTime(Int32.Parse(ds1[0]), Int32.Parse(ds1[1]), Int32.Parse(ds1[2]));
                var d2 = new DateTime(Int32.Parse(ds2[0]), Int32.Parse(ds2[1]), Int32.Parse(ds2[2]));
                if (d2 == DateTime.Today)
                {
                    d2 = d2.AddDays(1);
                }
                if (d1 == DateTime.Today || d1 == d2)
                {
                    if (interval == "1m" || interval == "2m" || interval == "5m" || interval == "15m")
                    {
                        //d1 = d1.AddDays(-1);
                    }
                }
                TimeSpan t1 = d1 - new DateTime(1970, 1, 1);
                int secondsSinceEpoch1 = (int)t1.TotalSeconds;
                TimeSpan t2 = d2 - new DateTime(1970, 1, 1);
                int secondsSinceEpoch2 = (int)t2.TotalSeconds;

                var url = "https://query2.finance.yahoo.com/v8/finance/chart/"
                    + name + "?symbol=" + name + "&period1=" + secondsSinceEpoch1 + "&period2=" + secondsSinceEpoch2 + "&interval=" + interval.ToLower();
                res = Get(url);
            }

            JObject data = JObject.Parse(res);

            var ch = data["chart"]["result"][0];

            var history = ch.ToObject<YResultViewModel>();
            var timestamp = ch["timestamp"];
            var indicators = ch["indicators"];
            List<CandleViewModel> candles = new List<CandleViewModel>();
            for (int i = 0; i < history.timestamp.Count; i++)
            {
                if (history.indicators.quote[0].low[i] == null || history.indicators.quote[0].high[i] == null || history.indicators.quote[0].open[i] == null || history.indicators.quote[0].close[i] == null)
                {

                }
                else
                {
                    candles.Add(new CandleViewModel()
                    {
                        time = history.timestamp[i],
                        low = history.indicators.quote[0].low[i],
                        high = history.indicators.quote[0].high[i],
                        open = history.indicators.quote[0].open[i],
                        close = history.indicators.quote[0].close[i],
                        volume = history.indicators.quote[0].volume[i],
                    });
                }
            }
            dynamic result = new System.Dynamic.ExpandoObject();
            result.Aggregated = false;
            result.Data = candles;
            return Ok(result);
        }


        //[HttpPost("insertdailyanalysis")]
        //public IActionResult InsertDailyAnalysis([FromBody] JObject model)
        //{
        //    try
        //    {
        //        var davm = JsonConvert.DeserializeObject<DailyAnalysisViewModel>(model.ToString());
        //        DailyAnalysis da = new DailyAnalysis();
        //        da.Stock = davm.Stock;
        //        da.DateTime = davm.DateTime;
        //        da.PocLevel = davm.PocLevel;
        //        da.ResistanceGp = davm.ResistanceGp;
        //        da.SupportGp = davm.SupportGp;
        //        da.ResistanceG = davm.ResistanceG;
        //        da.SupportG = davm.SupportG;

        //        _dailyAnalysisService.Save(da);
        //        return Ok(da);
        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //    return BadRequest();
        //}

        [HttpGet("getlastdailyanalysis")]
        public ActionResult GetLastDailyAnalysis(string stock, int index = 0)
        {
            var last = _dailyAnalysisService.GetLast(stock, index);
            if (last == null)
            {
                return NotFound();
            }
            return Ok(last);
        }

        [HttpGet("getlastdailyanalysislist")]
        public ActionResult GetLastDailyAnalysisList(string stock)
        {
            var dalist = _dailyAnalysisService.GetAnalysisList(stock);
            if (dalist == null)
            {
                return NotFound();
            }
            return Ok(dalist);
        }

        [HttpPost("trades")]
        public IActionResult Trades([FromBody] JObject model)
        {
            var puser = JsonConvert.DeserializeObject<UserViewModel>(model.ToString());
            var user = _userService.GetUserByUsername(puser.Email);
            if (user == null)
                return NotFound();
            var orders = user.Order.Where(o => o.IsExpired != true).OrderByDescending(o => o.DateFinished).ToList();
            List<string> tickers = new List<string>();
            List<TradeViewModel> tradeout = new List<TradeViewModel>();
            if (orders.Count > 0)
            {
                foreach (var order in orders)
                {
                    foreach (var ordet in order.OrderDetail)
                    {
                        tickers.AddRange(ordet.OrderDetailsTicker.Select(t => t.TickerName.Trim()));
                    }
                }
                if (tickers.Count > 0)
                {
                    var dt = DateTime.Now.Date;
                    //dt = dt.AddDays(-1); //Uncomment for test with previous day trades
                    var trades = _tradeService.GetAllTrades().Where(t => t.Active == true && tickers.Contains(t.Ticker.Trim()) && t.DateCreated > dt).ToList();
                    if (trades.Count > 0)
                    {
                        foreach (var trade in trades)
                        {
                            tradeout.Add(new TradeViewModel()
                            {
                                id = trade.Id,
                                ticker = trade.Ticker,
                                datecreated = trade.DateCreated,
                                interval = trade.Interval,
                                buysell = trade.BuySell != null ? (bool)trade.BuySell : true,
                                algorithm = trade.AlgorithmRef.AlgorithmName
                            });
                        }
                        //return Ok(tradeout);
                    }
                }
            }
            List<TradeViewModel> result = tradeout.GroupBy(g => new { g.ticker, g.datecreated, g.interval, g.algorithm })
                         .Select(g => g.First())
                         .ToList();
            tradeout = result.OrderByDescending(t => t.datecreated)/*.Take(10)*/.ToList();
            return Ok(tradeout);
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
