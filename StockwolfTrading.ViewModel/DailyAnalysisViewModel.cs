using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockwolfTrading.ViewModel
{
    public class DailyAnalysisViewModel
    {
        public int Id { get; set; }
        public string Stock { get; set; }
        public DateTime DateTime { get; set; }
        public float? PocLevel { get; set; }
        public string ResistanceG { get; set; }
        public string SupportG { get; set; }
        public string ResistanceGp { get; set; }
        public string SupportGp { get; set; }
    }
}
