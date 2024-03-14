using StockWolfTrading.Core.Models;
using StockWolfTrading.DataModel.UnitOfWork;
using StockWolfTrading.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockWolfTrading.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SettingsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public void EditSetting(Settings setting)
        {
            try
            {
                _unitOfWork.SettingsRepository.Update(setting);
                _unitOfWork.Save();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Settings GetByName(string name)
        {
            return _unitOfWork.SettingsRepository.Get(x => x.SettingName == name);
        }

        public bool GetUpdateStatus()
        {
            var fu = _unitOfWork.SettingsRepository.Get(x => x.SettingName == "ForceUpdate");
            if (fu != null)
            {
                if (fu.SettingValue == "1")
                    return true;
                else return false;
            }
            return true;
        }

        public bool SetForceUpdate()
        {
            var fu = _unitOfWork.SettingsRepository.Get(x => x.SettingName == "ForceUpdate");
            if (fu != null)
            {
                try
                {
                    fu.SettingValue = "1";
                    _unitOfWork.SettingsRepository.Update(fu);
                    _unitOfWork.Save();
                }
                catch(Exception)
                {
                    throw;
                }
            }
            return false;
        }
    }
}
