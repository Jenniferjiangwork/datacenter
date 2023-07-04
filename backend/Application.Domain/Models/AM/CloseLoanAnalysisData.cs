using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Domain.Models.AM
{

    [Keyless]
    public class CloseLoanAnalysis
    {
        public int data_year { get; set; }
        public int data_mth { get; set; }
        public int data_day { get; set; }
        public string loantype { get; set; }
        public string dataname { get; set; }
        public string datavalue { get; set; }
        public DateTime updatedate { get; set; }
    }

    [Keyless]
    public class CloseLoanAnalysisData
    {
        public int data_year { get; set; }
        public int data_mth { get; set; }
        public int data_day { get; set; }
        public string loantype { get; set; }
        public string dataname { get; set; }
        public string datavalue { get; set; }
        public DateTime updatedate { get; set; }
    }
}
