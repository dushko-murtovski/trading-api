using StockWolfTrading.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockWolfTrading.Services.Interfaces
{
    public interface IOrderService
    {
        Order GetOrderById(int orderid);
        Order AddOrder(Order order);
    }
}
