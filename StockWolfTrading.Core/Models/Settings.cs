using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockWolfTrading.Core.Models
{
    public class Settings
    {
        public int SettingsId { get; set; }
        public string SettingName { get; set; }
        public string SettingValue { get; set; }
    }
}
