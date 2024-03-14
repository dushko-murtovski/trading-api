using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockWolfTrading.Core.Models
{
    public partial class UserProducts
    {
        public int Id { get; set; }
        public int UserUserId { get; set; }
        public int ProductProductId { get; set; }
        public DateTime DateAdded { get; set; } 

        public virtual Product ProductProduct { get; set; }
        public virtual User UserUser { get; set; }
    }
}
