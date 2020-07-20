using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChamdrimERP.Models
{
    public class ProfitAndLoss
    {
        public string Name { get; set; }
        public decimal? DrBalance { get; set; }
        public decimal? CrBalance { get; set; }
        public decimal? ClosingBalance { get; set; }
        public string Data { get; set; }
    }
}