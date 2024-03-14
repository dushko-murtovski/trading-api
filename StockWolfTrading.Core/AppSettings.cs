using System;
using System.Collections.Generic;
using System.Text;

namespace StockWolfTrading.Core
{
    public class AppSettings
    {
        public string SWTConnString { get; set; }
        public string Secret { get; set; }
        public string SiteUrl { get; set; }
        public string EmailApiKey { get; set; }
        public string EmailFrom { get; set; }
        public string NameFrom { get; set; }
    }
}
