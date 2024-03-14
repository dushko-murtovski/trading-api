using System;
using System.Collections.Generic;
using System.Text;

namespace StockWolfTrading.Core.Models
{
    public partial class Cert
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Value { get; set; }
    }
}
