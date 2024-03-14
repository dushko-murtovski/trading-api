using System;
using System.Collections.Generic;

namespace StockWolfTrading.Core.Models
{
    public partial class Trade
    {
        public int Id { get; set; }
        public int AlgorithmRefId { get; set; }
        public DateTime DateCreated { get; set; }
        public float CandleOpen { get; set; }
        public float CandleClose { get; set; }
        public float CandleHigh { get; set; }
        public float CandleLow { get; set; }
        public string Interval { get; set; }
        public string Ticker { get; set; }
        public bool? BuySell { get; set; }
        public bool? Active { get; set; }

        public virtual Algorithm AlgorithmRef { get; set; }
    }
}
