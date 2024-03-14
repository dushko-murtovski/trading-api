using System;
using System.Collections.Generic;

namespace StockWolfTrading.Core.Models
{
    public partial class OrderDetailsTicker
    {
        public int Id { get; set; }
        public int OrderDetailsRefId { get; set; }
        public string TickerName { get; set; }
        public DateTime DateSelected { get; set; }
        public int? Priority { get; set; }
        public bool? Active { get; set; }

        public virtual OrderDetail OrderDetailsRef { get; set; }
    }
}
