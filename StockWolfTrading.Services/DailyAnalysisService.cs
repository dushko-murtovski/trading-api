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
    public class DailyAnalysisService : IDailyAnalysisService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DailyAnalysisService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IQueryable<Trade> GetAllTrades() =>
            _unitOfWork.TradeRepository.GetAll();

        public DailyAnalysis GetDailyAnalysis(int id)
        {
            return _unitOfWork.DailyAnalysisRepository.Get(x => x.Id == id);
        }

        public DailyAnalysis GetLast(string stock, int index)
        {
            var list = _unitOfWork.DailyAnalysisRepository.GetAll().OrderByDescending(x => x.DateTime).Where(x => x.Stock == stock).ToList();
            if (list.Count > index)
                return list[index];

            return null;
        }

        public List<DailyAnalysis> GetAnalysisList(string stock)
        {
            return _unitOfWork.DailyAnalysisRepository.GetAll().Where(x => x.Stock == stock).OrderByDescending(x => x.DateTime).ToList();
        }
        public bool Save(DailyAnalysis analysis)
        {
            try
            {
                _unitOfWork.DailyAnalysisRepository.Insert(analysis);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
