using System;
using System.Collections.Generic;

namespace StockWolfTrading.Core.Models
{
    public partial class UserSettings
    {
        public int Id { get; set; }
        public int UserRefId { get; set; }
        public int? CandleTime { get; set; }
        public float? CandleOpen { get; set; }
        public float? CandleClose { get; set; }
        public float? CandleHigh { get; set; }
        public float? CandleLow { get; set; }
        public string Interval { get; set; }
        public string Ticker { get; set; }

        public virtual User UserRef { get; set; }
    }
}
