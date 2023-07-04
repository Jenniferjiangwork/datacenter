using Report.Domain.Models.Common;
using Report.Domain.Models.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMReport.Service
{
    public interface IDrawDownService
    {
        Task<ServiceResponse<List<DrawdownData>>> GetDrawDownData();
        Task<ServiceResponse<DrawdownUpdatedDate>> GetDrawdownUpdatedDate(bool force);
        Task<ServiceResponse<DrawdownDetailDTO>> GetDrawdownDetail(string data_name, int year, int month);
    }
}
