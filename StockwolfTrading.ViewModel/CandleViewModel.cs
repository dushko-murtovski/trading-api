using System;
using System.Collections.Generic;
using System.Text;

namespace StockwolfTrading.ViewModel
{
    public class CandleViewModel
    {
        public int time { get; set; }
        public double? low { get; set; }
        public double? high { get; set; }
        public double? open { get; set; }
        public double? close { get; set; }
        public int? volume { get; set; }
        public string conversionSymbol { get; set; }
        public string conversionType { get; set; } = "force_direct";
    }
}
