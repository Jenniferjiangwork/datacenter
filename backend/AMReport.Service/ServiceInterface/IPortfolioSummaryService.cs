using Report.Domain.Models.AM;
using Report.Domain.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMReport.Service
{
    public interface IPortfolioSummaryService
    {
        Task<ServiceResponse<List<PortfolioSummaryData>>> GetPortfolioSummary(int selectedYear, int selectedMonth, string lender, bool isSeparateTable);
        Task<ServiceResponse<PortfolioSummaryLatestDrawDownDate>> GetPortfolioSummaryLatestDrawDownDate(int selectedYear, int selectedMonth, string lender);
        Task<ServiceResponse<List<PortfolioSummaryDetail>>> GetPortfolioSummaryDetail(int selectedYear, int selectedMonth, string lender,  string loanstatus, bool isSeparateTable, string sortColumn, string sortOrder);
        Task<ServiceResponse<PortfolioSummaryExportDetail>> GetPortfolioSummaryExportDetail(int selectedYear, int selectedMonth, string lender);
    }
}
