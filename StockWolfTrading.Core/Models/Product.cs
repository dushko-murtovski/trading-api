using System;
using System.Collections.Generic;

namespace StockWolfTrading.Core.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderDetail = new HashSet<OrderDetail>();
            UserProducts = new HashSet<UserProducts>();
        }

        public int Id { get; set; }
        public string ProductName { get; set; }
        public float? Price { get; set; }
        public int MaxStocks { get; set; }
        public string Note { get; set; }
        public bool? Active { get; set; }

        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
        public virtual ICollection<UserProducts> UserProducts { get; set; }
    }
}
