using System;
using System.Linq;
using StockWolfTrading.Core.Models;

namespace StockWolfTrading.Services.Interfaces
{
    public interface ITickerService
    {
        bool DeleteTicker(string tickername);
        OrderDetailsTicker AddTicker(OrderDetailsTicker ticker);
        IQueryable<OrderDetailsTicker> GetAllTickers();
    }
}
