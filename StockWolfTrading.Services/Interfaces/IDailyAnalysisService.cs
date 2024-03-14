using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockWolfTrading.Core.Models;

namespace StockWolfTrading.Services.Interfaces
{
    public interface IDailyAnalysisService
    {
        DailyAnalysis GetLast(string stock, int index);
        List<DailyAnalysis> GetAnalysisList(string stock);
        DailyAnalysis GetDailyAnalysis(int id);

        bool Save(DailyAnalysis analysis);
    }
}
