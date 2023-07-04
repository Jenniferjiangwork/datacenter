using Report.Domain.Models.Common;
using Report.Domain.Models.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Service
{
    public interface ILenderService
    {
        Task<ServiceResponse<List<LenderData>>> GetAllLenders();
    }
}
