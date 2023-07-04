using Dapper;
using log4net;
using Microsoft.EntityFrameworkCore;
using Report.Domain.Models.AM;
using Report.Domain.Models.Common;
using Report.Infra.Data.Context;
using Report.Infra.Data.Utlis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AMReport.Service
{
    public class CloseLoanAnalysisService : ICloseLoanAnalysisService
    {

        private readonly DatacentreDBContext _dataCentreContext;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        IDbConnection _db;
        public CloseLoanAnalysisService(DatacentreDBContext context)
        {
            _db = new DbFactory(Config.DatacentreDatabase).db;
            _dataCentreContext = context;
        }

        public async Task<ServiceResponse<List<CloseLoanAnalysisData>>> GetCloseLoanAnalysisService(int selectedYear, int selectedMonth)
        {
            var serviceResponse = new ServiceResponse<List<CloseLoanAnalysisData>>();
            CloseLoanAnalysisData closeLoanAnalysisData;
            List<CloseLoanAnalysisData> lstCloseLoanAnalysis = new List<CloseLoanAnalysisData>();
            try
            {
                string sql = $@"SELECT 
                                     data_year, data_mth,data_day, loantype, dataname, datavalue ,updatedate
                                FROM 
                                     dc_rpt_closeloans
                                WHERE data_year = '{selectedYear}' AND data_mth = '{selectedMonth}';";
                List<CloseLoanAnalysisData> rs = await _dataCentreContext.CloseLoanAnalysisData.FromSqlRaw($"{sql}").ToListAsync();
                serviceResponse.Data = rs;
            }          
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                log.Error(ex.Message);
                var result = await CommonFunction.SaveActivityLog("GetCloseLoanAnalysisService", "GetCloseLoanAnalysisService", ex, _dataCentreContext);
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error Id: {result.Id}";
            }
            return serviceResponse;
        }

        public Task<ServiceResponse<List<string>>> GetCloseLoanAnalysisColsByMonth(int selectedYear, int selectedMont)
        {
            throw new NotImplementedException();
        }
    }
}
