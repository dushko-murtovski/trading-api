using StockWolfTrading.Core.Models;
using StockWolfTrading.DataModel.UnitOfWork;
using StockWolfTrading.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockWolfTrading.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public Order GetOrderById(int orderid)
        {
            var order = _unitOfWork.OrderRepository.GetByID(orderid);
            return order;
        }

        public Order AddOrder(Order order)
        {
            var res = _unitOfWork.OrderRepository.Insert(order);
            _unitOfWork.Save();
            return res;
        }
    }
}
