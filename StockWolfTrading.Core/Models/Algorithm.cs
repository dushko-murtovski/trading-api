using System;
using System.Collections.Generic;

namespace StockWolfTrading.Core.Models
{
    public partial class Algorithm
    {
        public Algorithm()
        {
            Trade = new HashSet<Trade>();
        }

        public int Id { get; set; }
        public string AlgorithmName { get; set; }

        public virtual ICollection<Trade> Trade { get; set; }
    }
}
