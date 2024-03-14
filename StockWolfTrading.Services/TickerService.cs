using StockWolfTrading.Core.Models;
using StockWolfTrading.DataModel.UnitOfWork;
using StockWolfTrading.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockWolfTrading.Services
{
    public class TickerService : ITickerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TickerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public bool DeleteTicker(string tickername)
        {
            var ticker = _unitOfWork.TickerRepository.GetAll().FirstOrDefault(pu => pu.TickerName == tickername);
            if (ticker == null)
            {
                return false;
            }
            else
            {
                _unitOfWork.TickerRepository.Delete(ticker);
                _unitOfWork.Save();
                return true;
            }
        }

        public OrderDetailsTicker AddTicker(OrderDetailsTicker ticker)
        {
            var res = _unitOfWork.TickerRepository.Insert(ticker);
            _unitOfWork.Save();
            return res;
        }

        public IQueryable<OrderDetailsTicker> GetAllTickers()
        {
            return _unitOfWork.TickerRepository.GetAll();
        }
    }
}
