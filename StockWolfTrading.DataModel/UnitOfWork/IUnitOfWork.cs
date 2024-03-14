using System;
using System.Collections.Generic;
using System.Text;
using StockWolfTrading.Core.Models;

namespace StockWolfTrading.DataModel.UnitOfWork
{
    public interface IUnitOfWork
    {
        GenericRepository<Trade> TradeRepository { get; }
        GenericRepository<User> UserRepository { get; }
        GenericRepository<Order> OrderRepository { get; }
        GenericRepository<OrderDetailsTicker> TickerRepository { get; }
        GenericRepository<Cert> CertRepository { get; }
        GenericRepository<DailyAnalysis> DailyAnalysisRepository { get; }
        GenericRepository<Settings> SettingsRepository { get; }
        void Save();
        void SaveAsync();
        void Dispose();
    }
}
