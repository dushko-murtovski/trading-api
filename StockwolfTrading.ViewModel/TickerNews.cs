using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockwolfTrading.ViewModel
{
    public class MarketData
    {
        public string Name { get; set; }
        public List<TickerNews> TickerNews { get; set; } = new List<TickerNews>();
        public string PriceChange { get; set; }
    }
    public class TickerNews
    {
        public string Author { get; set; }
        public string Summary { get; set; }
        public string ArticleUrl { get; set; }
        
    }
}
