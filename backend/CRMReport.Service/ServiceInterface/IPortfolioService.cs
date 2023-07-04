using Report.Domain.Models.Common;
using Report.Domain.Models.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMReport.Service
{
    public interface IPortfolioService
    {
        Task<ServiceResponse<List<PortfolioReportData>>> GetPortfolioData(string lender, string year);
    }
}
