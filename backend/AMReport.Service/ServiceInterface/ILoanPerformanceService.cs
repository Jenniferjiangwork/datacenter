using Report.Domain.Models.AM;
using Report.Domain.Models.Common;
using Report.Domain.Models.CRM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMReport.Service
{
    public interface ILoanPerformanceService
    {
        Task<ServiceResponse<List<LoanPerformanceColumn>>> GetLoanPerformanceColumn(string loantype, string lenderList);

        Task<ServiceResponse<List<dynamic>>> GetLoanPerformanceData(string loantype, string lenderList, string amounttype);

        Task<ServiceResponse<List<LoanPerformanceExportDetail>>> GetLoanPerformanceExportDetail(string loantype, string lenderList, string amounttype);

        Task<ServiceResponse<List<StatisticColumn>>> GetStatisticLoanPerformanceColumn(string loantype, string lenderList);

        Task<ServiceResponse<List<dynamic>>> GetStatisticLoanPerformanceData(string loantype, string lender, string amounttype, int monthno);
    }
}
