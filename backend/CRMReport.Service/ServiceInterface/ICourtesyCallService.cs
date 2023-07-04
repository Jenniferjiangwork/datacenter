using Report.Domain.Models.Common;
using Report.Domain.Models.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMReport.Service
{
    public interface ICourtesyCallService
    {
        Task<ServiceResponse<CourtesyCallOutput>> GetCourtesyCall(string year, string month);
    }
}
