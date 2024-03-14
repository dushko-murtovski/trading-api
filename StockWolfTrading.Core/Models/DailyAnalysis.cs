using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockWolfTrading.Core.Models
{
    public partial class DailyAnalysis
    {
        public int Id { get; set; }
        public string Stock { get; set; }
        public DateTime DateTime { get; set; }
        public float? PocLevel { get; set; }
        public string ResistanceG { get; set; }
        public string SupportG { get; set; }
        public string ResistanceGp { get; set; }
        public string SupportGp { get; set; }
        public string MS_Up { get; set; }
        public string MS_Down { get; set; }
        public string MS_Up5 { get; set; }
        public string MS_Down5 { get; set; }
        public string MS_Up15 { get; set; }
        public string MS_Down15 { get; set; }
    }
}
