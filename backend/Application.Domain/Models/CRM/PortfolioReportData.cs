using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Domain.Models.CRM
{
    public class PortfolioDBSet
    {
        public string lender { get; set; }
        public string[] lenders { get; set; }
        public List<string> lendersInDB { get; set; }
        public string year { get; set; }
        public List<PortfolioDBRow> rows { get; set; }
        private bool? _isCrossYear = null;
        public bool isCrossYear
        {
            get
            {
                if (_isCrossYear == null)
                {
                    _isCrossYear = year.Contains("-");
                }
                return _isCrossYear.Value;
            }
        }
        private List<int> _months = null;
        public List<int> months
        {
            get
            {
                if (_months == null)
                {
                    _months = new List<int>();
                    if (isCrossYear)
                    {
                        var _years = year.Split(new char[] { '-' });
                        var _year1 = int.Parse(_years[0]);
                        var _year2 = int.Parse(_years[1]);
                        for (int i = 6; i <= 12; i++)
                        {
                            months.Add(_year1 * 100 + i);
                        }
                        for (int i = 1; i <= 6; i++)
                        {
                            months.Add(_year2 * 100 + i);
                        }
                    }
                    else
                    {
                        int _year = int.Parse(year);
                        months.Add((_year - 1) * 100 + 12);
                        for (int i = 1; i <= 12; i++)
                        {
                            months.Add(_year * 100 + i);
                        }
                    }
                }
                return _months;
            }
        }
    }

    [Keyless]
    public class PortfolioReportData
    {
        public string lender { get; set; }
        public string year { get; set; }
        public string data_name { get; set; }
        public string label { get; set; }
        public string labelCss { get; set; } = "";
        public string dataCss { get; set; } = "";
        public string dataFormat { get; set; } = "";
        public decimal? avg { get; set; }
        public decimal[] values { get; set; } = new decimal[13];
        public int beginIdx { get; set; }
        public int endIdx { get; set; }
    }

    [Keyless]
    public class LenderData
    {
        public string lendername { get; set; }
        public string Lendershowname { get; set; }
    }

    [Keyless]
    public class PortfolioDBRow
    {
        public string lender { get; set; }
        public string data_name { get; set; }
        public int data_year { get; set; }
        public int data_month { get; set; }
        public decimal data_value { get; set; }
    }
}
