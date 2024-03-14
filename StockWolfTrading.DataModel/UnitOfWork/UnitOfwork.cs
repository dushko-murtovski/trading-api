using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using StockWolfTrading.Core;
using StockWolfTrading.Core.Models;

namespace StockWolfTrading.DataModel.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        public GenericRepository<Trade> TradeRepository =>
            _tradeRepository ?? (_tradeRepository = new GenericRepository<Trade>(_swtContext));

        public GenericRepository<User> UserRepository =>
            _userRepository ?? (_userRepository = new GenericRepository<User>(_swtContext));

        public GenericRepository<OrderDetailsTicker> TickerRepository =>
            _tickerRepository ?? (_tickerRepository = new GenericRepository<OrderDetailsTicker>(_swtContext));

        public GenericRepository<Order> OrderRepository =>
            _orderRepository ?? (_orderRepository = new GenericRepository<Order>(_swtContext));

        public GenericRepository<Cert> CertRepository =>
            _certRepository ?? (_certRepository = new GenericRepository<Cert>(_swtContext));

        public GenericRepository<DailyAnalysis> DailyAnalysisRepository =>
            _dailyAnalysisRepository ?? (_dailyAnalysisRepository = new GenericRepository<DailyAnalysis>(_swtContext));

        public GenericRepository<Settings> SettingsRepository =>
            _settingsRepository ?? (_settingsRepository = new GenericRepository<Settings>(_swtContext));

        private readonly StockWolfTradingDBContext _swtContext = null;
        private AppSettings AppSettings { get; set; }

        public UnitOfWork(AppSettings settings)
        {
            _swtContext = new StockWolfTradingDBContext(settings);
        }

        public UnitOfWork(IOptions<AppSettings> settings)
        {
            _swtContext = new StockWolfTradingDBContext(settings.Value);
        }

        public void Save()
        {
            _swtContext.SaveChanges();
        }

        public void SaveAsync()
        {
            _swtContext.SaveChangesAsync();
        }
        public void Dispose()
        {
            _swtContext.Dispose();
        }

        private GenericRepository<Trade> _tradeRepository;
        private GenericRepository<User> _userRepository;
        private GenericRepository<OrderDetailsTicker> _tickerRepository;
        private GenericRepository<Order> _orderRepository;
        private GenericRepository<Cert> _certRepository;
        private GenericRepository<DailyAnalysis> _dailyAnalysisRepository;
        private GenericRepository<Settings> _settingsRepository;
    }
}
