using Dapper;
using log4net;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Report.Domain.Models;
using Report.Domain.Models.Common;
using Report.Infra.Data.Context;
using Report.Infra.Data.Utlis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common.Service
{
    public class ReportNoteService : IReportNoteService
    {
        IDbConnection _db;
        private readonly DatacentreDBContext _dataCentreContext;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public ReportNoteService(DatacentreDBContext context)
        {
            _db = new DbFactory(Config.DatacentreDatabase).db;
            _dataCentreContext = context;
        }

        public async Task<ServiceResponse<string>> GetReportNote(string reportname, string company)
        {
            var serviceResponse = new ServiceResponse<string>();
            try
            {
                string sql = $@"SELECT reportnote FROM dc_rpt_report_note WHERE company='{company}' AND reportname='{reportname}'";
                serviceResponse.Data = await _db.QueryFirstOrDefaultAsync<string>(sql);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                log.Error(ex.Message);
                var result = await CommonFunction.SaveActivityLog<string, Exception>("GetReportNote", "GetReportNote", ex, _dataCentreContext);
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error Id: {result.Id}";
            }
            return serviceResponse;
        }


        public async Task<ServiceResponse<int>> SaveReportNote(string reportname, string reportnote, string company)
        {
            var serviceResponse = new ServiceResponse<int>();
            try
            {
                serviceResponse.Data =  await _db.ExecuteScalarAsync<int>("sp_rpt_report_note_save", 
                    new {
                            p_ReportName = reportname,
                            p_Note = reportnote,
                            p_Company = company 
                    }, null, null, CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                log.Error(ex.Message);
                var result = await CommonFunction.SaveActivityLog<string, Exception>("SaveReportNote", "SaveReportNote", ex, _dataCentreContext);
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error Id: {result.Id}";
            }
            return serviceResponse;
        }
    }
}
