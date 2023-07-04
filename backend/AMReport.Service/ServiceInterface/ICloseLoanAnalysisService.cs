using Report.Domain.Models.AM;
using Report.Domain.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMReport.Service
{
    public interface ICloseLoanAnalysisService
    {
        Task<ServiceResponse<List<CloseLoanAnalysisData>>> GetCloseLoanAnalysisService(int selectedYear, int selectedMonth);
    }
}
