using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Domain.Models.AM
{
    [Keyless]
    public class PortfolioSummaryData
    {
        public int data_year { get; set; }
        public int data_month { get; set; }
        public string status { get; set; }
        public int count { get; set; }
        public double percentageCount { get; set; }
        public decimal cashamount { get; set; }
        public double percentageCashAmount { get; set; }
        public decimal repaidamount { get; set; }
        public double percentageRepaidAmount { get; set; }
        public decimal expectedamount { get; set; }
        public double percentageExpectedAmount { get; set; }
        public string lender { get; set; }
        public DateTime? update_date { get;set;}
        public int isActiveLoan { get; set; }
        public decimal discrepancy { get; set; }
    }

    [Keyless]
    public class PortfolioSummaryLatestDrawDownDate
    {
        public string latestDrawDownDate { get; set; }
    }

    [Keyless]
    public class PortfolioSummaryDetail
    {
        public int loanaccountid { get; set; }
        public DateTime? settleDate { get; set; }
        public string loanNo { get; set; }
        public string loanStatus { get; set; }
        public decimal cashamount { get; set; }
        public decimal loanBalance { get; set; }
        public decimal totalReceived { get; set; }
        public decimal arrears { get; set; }
    }

    [Keyless]
    public class PortfolioSummaryExport
    {
        public string loanaccountid { get; set; }
        public string drawDownDate { get; set; }
        public string loanNo { get; set; }
        public string loanStatus { get; set; }
        public decimal cashAmount { get; set; }
        public decimal loanBalance { get; set; }
        public decimal totalReceived { get; set; }
        public decimal arrears { get; set; }
    }


    [Keyless]
    public class PortfolioSummaryExportDetail
    {
        public string sheetName { get; set; }
        public DataTable dataTable { get; set; }
    }
}
