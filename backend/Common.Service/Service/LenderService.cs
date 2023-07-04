
using log4net;
using Microsoft.EntityFrameworkCore;
using Report.Domain.Models.Common;
using Report.Domain.Models.CRM;
using Report.Infra.Data.Context;
using Report.Infra.Data.Utlis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common.Service
{
    public class LenderService : ILenderService
    {
        private readonly CrmDBContext _crmDBContext;
        private readonly DatacentreDBContext _dataCentreContext;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public LenderService(DatacentreDBContext dataCentreContext, CrmDBContext crmDBContext)
        {
            _dataCentreContext = dataCentreContext;
            _crmDBContext = crmDBContext;
        }

        public async Task<ServiceResponse<List<LenderData>>> GetAllLenders()
        {
            var serviceResponse = new ServiceResponse<List<LenderData>>();
            try
            {
                string sql = $@"SELECT distinct lendername, Lendershowname FROM `morganse_crm`.ams_lendersetting;";
                List<LenderData> rs = await _crmDBContext.LenderData.FromSqlRaw($"{sql}").ToListAsync();
                serviceResponse.Data = rs;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                log.Error(ex.Message);
                var result = await CommonFunction.SaveActivityLog("GetAllLenders", "GetAllLenders", ex, _dataCentreContext);
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error Id: {result.Id}";
            }
            return serviceResponse;
        }

    }
}
