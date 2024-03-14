using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using SocketIOSharp.Common;
using SocketIOSharp.Server;
using StockwolfTrading.ViewModel;
using StockWolfTrading.Core;
using StockWolfTrading.Core.Models;
using StockWolfTrading.DataModel.UnitOfWork;
using StockWolfTrading.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Alpaca.Markets;

namespace StockWolfTrading.RealTime
{
    public class LastPrice
    {
        public int Time { get; set; } = 0;
        public double Price { get; set; }
    }
    class Program
    {
        static private AppSettings appSettings;
        static public string Get(string uri)
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
        static string LastCandle(SocketIOServer server, string tick)
        {
            CandleViewModel candle = new CandleViewModel();

            var name = tick;

            string API_KEY = "PKUG8TX0P2XOF8R4DFUD";

            string API_SECRET = "NI3AVS9dlaZDDnKsE25fQFr2N1fEW5WsJu1vh0Kp";

            var client = Alpaca.Markets.Environments.Paper
                .GetAlpacaDataClient(new SecretKey(API_KEY, API_SECRET));

            LatestMarketDataRequest latest = new LatestMarketDataRequest(name);

            var item = client.GetLatestBarAsync(latest).Result;
            var tm = item.TimeUtc.ToUniversalTime();
            var ms = new DateTimeOffset(tm).ToUnixTimeMilliseconds() / 1000;
            candle = new CandleViewModel()
            {
                time = (int)ms,
                low = (double)item.Low,
                high = (double)item.High,
                open = (double)item.Open,
                close = (double)item.Close,
                volume = (int)item.Volume,
            };

            var str = tick.ToUpper() + "~USD" + " time: " + candle.time.ToString() + " close: " + candle.close + " open: " + candle.open + " low: " + candle.low + " high: " + candle.high;
            server.Emit("m", str);
            return str;
        }
        static LastPrice LastPrice(SocketIOServer server, string tick)
        {
            LastPrice result = new LastPrice();
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
                result.Time = candles[candles.Count - 1].time;
                result.Price = candles[candles.Count - 1].close != null ? Math.Round((double)candles[candles.Count - 1].close, 3) : -1;
            }
            var str = "0~NYSE~" + tick.ToUpper() + "~USD" + "~1~789311658~" + result.Time.ToString() + "~0.0006~" + result.Price + "~23.1306~1627983141~640000000~659000000~1bf";
            server.Emit("m", str);
            return result;
        }
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            appSettings = new AppSettings();
            config.GetSection("AppSettings").Bind(appSettings);
            try
            {
                using (SocketIOServer server = new SocketIOServer(new SocketIOServerOption(9001)))
                //using (SocketIOServer server = new SocketIOServer(new SocketIOServerOption(443, "/socket.io", true
                //    , 5000, 25000, 10000, true, true, true, true, "io", null, null, null, null, cert)))
                {
                    using (UnitOfWork uv = new UnitOfWork(appSettings))
                    {
                        try
                        {
                            var tickerService = new TickerService(uv);

                            Console.WriteLine("Listening on " + server.Option.Port);

                            server.OnConnection((socket) =>
                            {
                                Console.WriteLine("Client connected!");

                                socket.On("input", (data) =>
                                {
                                    foreach (JToken token in data)
                                    {
                                        Console.Write(token + " ");
                                    }

                                    Console.WriteLine();
                                    socket.Emit("echo", data);
                                });

                                socket.On(SocketIOEvent.DISCONNECT, () =>
                                {
                                    Console.WriteLine("Client disconnected!");
                                });

                                socket.Emit("echo", new byte[] { 0, 1, 2, 3, 4, 5 });
                            });

                            server.Start();

                            //Console.WriteLine("Input /exit to exit program.");
                            //string line;
                            int i = 0;

                            while (true)
                            {
                                var tickers = tickerService.GetAllTickers().Select(t => t.TickerName).Distinct().ToList();
                                Parallel.ForEach(tickers, ticker =>
                                {
                                    try
                                    {
                                        var tick = ticker.Trim();
                                        //var lastprice = LastPrice(server, tick);
                                        //var str = "0~NYSE~" + tick.ToUpper() + "~USD" + "~1~789311658~" + lastprice.Time.ToString() + "~0.0006~" + lastprice.Price + "~23.1306~1627983141~640000000~659000000~1bf";
                                        
                                        var str = LastCandle(server, tick);
                                        Console.WriteLine(i.ToString() + " " + str);
                                        i++;
                                        //server.Emit("m", str);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("Exception: " + ex.Message + Environment.NewLine + ex.StackTrace);
                                    }
                                });
                                Console.WriteLine();
                                Thread.Sleep(3000);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Exception: " + ex.Message + Environment.NewLine + ex.StackTrace);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message + Environment.NewLine + ex.StackTrace);
            }
            //Console.WriteLine("Press enter to continue...");
            Console.Read();
        }
    }
}
