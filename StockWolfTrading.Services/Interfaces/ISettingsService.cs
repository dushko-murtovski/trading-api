using StockWolfTrading.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockWolfTrading.Services.Interfaces
{
    public interface ISettingsService
    {
        Settings GetByName(string name);
        void EditSetting(Settings setting);
        bool SetForceUpdate();
        bool GetUpdateStatus();
    }
}
