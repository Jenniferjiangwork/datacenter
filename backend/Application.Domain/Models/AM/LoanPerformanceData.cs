using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Domain.Models.AM
{
    public class LoanPerformanceData
    {
        public string LoanMonth { get; set; }
        public string LoanYear { get; set; }
        public List<string> month { get; set; }
    }

    [Keyless]
    public class LoanPerformanceColumn
    {
        public string columnMonth { get; set; }
    }

    [Keyless]
    public class LoanPerformanceColumns
    {
        public string month { get; set; }
        public string Value { get; set; }
    }

    [Keyless]
    public class LoanPerformanceStatisticData
    {
        public string mnth { get; set; }
        public string yr { get; set; }
        public string y { get; set; }
        public string m { get; set; }
        public decimal overall_type_rate_all { get; set; }
        public decimal overall_type_rate { get; set; }
        public decimal this_month_type_rate_all { get; set; }
        public decimal last_month_type_rate_all { get; set; }
        public decimal this_month_type_rate { get; set; }
        public decimal last_month_type_rate { get; set; }
        public decimal difference_month_type_rate { get; set; }
        public decimal differenceper_month_type_rate { get; set; }
        public decimal difference_month_type_rate_all { get; set; }
        public decimal differenceper_month_type_rate_all { get; set; }
    }

    [Keyless]
    public class LoanPerformanceStatistic
    {
        public string year { get; set; }
        public string month { get; set; }
        public decimal overallavg { get; set; }
        public decimal lastmonthavg { get; set; }
        public decimal currmonthavg { get; set; }
        public decimal difference { get; set; }
        public decimal differenceper { get; set; }
    }

    [Keyless]
    public class StatisticColumn
    {
        public string year { get; set; }
        public string month { get; set; }
    }

    [Keyless]
    public class LoanPerformanceExportDetail
    {
        public string sheetName { get; set; }
        public DataTable dataTable { get; set; }
    }
}
