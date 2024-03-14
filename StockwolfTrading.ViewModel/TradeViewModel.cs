using System;
using System.Collections.Generic;
using System.Text;

namespace StockwolfTrading.ViewModel
{
    public class TradeViewModel
    {
        public int id { get; set; }
        public string ticker { get; set; }
        public DateTime datecreated { get; set; }
        public string interval { get; set; }
        public bool buysell { get; set; }
        public string algorithm { get; set; }
    }
}
