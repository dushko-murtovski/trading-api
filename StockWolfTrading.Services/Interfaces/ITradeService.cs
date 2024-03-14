using StockWolfTrading.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockWolfTrading.Services.Interfaces
{
    public interface ITradeService
    {
        IQueryable<Trade> GetAllTrades();
    }
}
