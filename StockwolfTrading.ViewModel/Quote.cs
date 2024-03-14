using System;
using System.Collections.Generic;
using System.Text;

namespace StockwolfTrading.ViewModel
{
    public class QuoteEl
    {
        public List<double?> low { get; set; }
        public List<double?> high { get; set; }
        public List<double?> open { get; set; }
        public List<double?> close { get; set; }
        public List<int?> volume { get; set; }
    }
}
