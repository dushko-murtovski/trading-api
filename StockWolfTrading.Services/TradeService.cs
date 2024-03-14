using StockWolfTrading.Core.Models;
using StockWolfTrading.DataModel.UnitOfWork;
using StockWolfTrading.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockWolfTrading.Services
{
    public class TradeService : ITradeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TradeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IQueryable<Trade> GetAllTrades() =>
            _unitOfWork.TradeRepository.GetAll();
    }
}
