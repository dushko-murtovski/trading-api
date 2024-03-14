using System;
using System.Collections.Generic;

namespace StockWolfTrading.Core.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetail = new HashSet<OrderDetail>();
        }

        public int Id { get; set; }
        public int UserRefId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateFinished { get; set; }
        public float? TotalAmount { get; set; }
        public string Note { get; set; }
        public bool? IsExpired { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
    }
}
