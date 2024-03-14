using System;
using System.Collections.Generic;
using System.Text;

namespace StockwolfTrading.ViewModel
{
    public class YResultViewModel
    {
        public List<int> timestamp { get; set; } = new List<int>();
        public Indicator indicators { get; set; }
    }
    public class Indicator
    {
        public List<QuoteEl> quote { get; set; } = new List<QuoteEl>();
    }
}
