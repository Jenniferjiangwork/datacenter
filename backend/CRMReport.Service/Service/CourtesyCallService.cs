using log4net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Report.Infra.Data.Context;
using Report.Infra.Data.Utlis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using Dapper;
using System.Threading.Tasks;
using Report.Domain.Models.CRM;
using Report.Domain.Models;
using Newtonsoft.Json;
using Report.Domain.Models.Common;

namespace CRMReport.Service
{
    public class CourtesyCallService : ICourtesyCallService
    {
        private readonly DatacentreDBContext _dataCentreContext;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        IDbConnection _db;
        public CourtesyCallService(DatacentreDBContext context)
        {
            _db = new DbFactory(Config.DatacentreDatabase).db;
            _dataCentreContext = context;
        }

        public async Task<ServiceResponse<CourtesyCallOutput>> GetCourtesyCall(string year, string month)
        {
            CourtesyCallDBSet dataSet = null;
            CourtesyCallOutput courtesyCallOutput = null;
            var serviceResponse = new ServiceResponse<CourtesyCallOutput>();
            try
            {
                string rowSql = $@"select * 
                from dc_rpt_courtesycall
                            where data_year = { year}
                and data_month = { month }
                            order by data_name, data_year, data_month";

                string operatorSql = $@"select distinct data_operator
                                        from dc_rpt_courtesycall
                                        where data_year = {year}
                                        and data_month = {month}
                                        order by data_name, data_year, data_month";

                dataSet = new CourtesyCallDBSet
                {
                    rows = _db.Query<CourtesyCallDBRow>(rowSql).ToList(),
                    operators = _db.Query<string>(operatorSql).ToList()
                };
                courtesyCallOutput = CourtesyCallBuild(dataSet);
               

                var result = new CourtesyCallOutput()
                {
                    List = courtesyCallOutput.List,
                    Operators = courtesyCallOutput.Operators
                };
                serviceResponse.Data = result;


            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                log.Error(ex.Message);
                var result = await CommonFunction.SaveActivityLog<string, Exception>("GetCourtesyCall", "GetCourtesyCall", ex, _dataCentreContext);
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error Id: {result.Id}";
            }
            return serviceResponse;
        }

        public static CourtesyCallOutput CourtesyCallBuild(CourtesyCallDBSet dataSet)
        {
            var output = new CourtesyCallOutput
            {
                List = new List<CourtesyCall>(),
                Operators = dataSet.operators
            };
            dataSet.rows.ForEach(value =>
            {
                var search = output.List.Find(x =>
                {
                    return x.Year == value.data_year && x.Month == value.data_month && x.Name == value.data_name;
                });
                if (search != null)
                {
                    search.Data.Add(new CourtesyCallRow
                    {
                        Operator = value.data_operator,
                        Value = value.data_value,
                    });
                }
                else
                {
                    var temp = new CourtesyCall
                    {
                        Year = value.data_year,
                        Month = value.data_month,
                        Name = value.data_name,
                        Data = new List<CourtesyCallRow>()
                    };
                    temp.Data.Add(new CourtesyCallRow
                    {
                        Operator = value.data_operator,
                        Value = value.data_value
                    });
                    output.List.Add(temp);
                }

            });
            return output;
        }
    }
}
