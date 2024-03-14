using System;
using System.Collections.Generic;

namespace StockWolfTrading.Core.Models
{
    public partial class OrderDetail
    {
        public OrderDetail()
        {
            OrderDetailsTicker = new HashSet<OrderDetailsTicker>();
        }

        public int Id { get; set; }
        public int OrderRefId { get; set; }
        public int ProductRefId { get; set; }
        public float? Price { get; set; }
        public string Note { get; set; }

        public virtual Order OrderRef { get; set; }
        public virtual Product ProductRef { get; set; }
        public virtual ICollection<OrderDetailsTicker> OrderDetailsTicker { get; set; }
    }
}
