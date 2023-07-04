using log4net;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Report.Domain.Models;
using Report.Domain.Models.Common;
using Report.Domain.Models.CRM;
using Report.Infra.Data.Context;
using Report.Infra.Data.Utlis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Report.Infra.Data.Utlis.PortfolioProvider;

namespace CRMReport.Service
{
    public class PortfolioService : IPortfolioService
    {
        IDbConnection _db;
        private readonly CrmDBContext _crmDBContext;
        private readonly DatacentreDBContext _dataCentreContext;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public PortfolioService(DatacentreDBContext dataCentreContext, CrmDBContext crmDBContext)
        {
            _db = new DbFactory(Config.DatacentreDatabase).db;
            _dataCentreContext = dataCentreContext;
            _crmDBContext = crmDBContext;         
        }
      
        public async Task<ServiceResponse<List<PortfolioReportData>>> GetPortfolioData(string lender, string year)
        {
            var serviceResponse = new ServiceResponse<List<PortfolioReportData>>();
            try
            {
                var result = new List<PortfolioReportData>();
                var rawData = LoadDbSet(lender, year);
                PortfolioSizeProvider.PortfolioSize(rawData, result);
                PortfolioInOutProvider.PortfolioInOut(rawData, result);
                PortfolioCashFlowProvider.PortfolioCashFlow(rawData, result);
                serviceResponse.Data = result;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                log.Error(ex.Message);
                var result = await CommonFunction.SaveActivityLog("GetPortfolioData", "GetPortfolioData", ex, _dataCentreContext);
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error Id: {result.Id}";
            }
            return serviceResponse;
        }

        public PortfolioDBSet LoadDbSet(string lender, string year)
        {
            var lenders = GetLenders(lender);
            int begin, end, beginSelect;
            if (year.Contains("-"))
            {
                int bYear = int.Parse(year.Split(new char[] { '-' })[0]);
                begin = bYear * 100 + 7;
                beginSelect = bYear * 100 + 6;
                end = (bYear + 1) * 100 + 6;
            }
            else
            {
                begin = int.Parse(year) * 100 + 1;
                beginSelect = (int.Parse(year) - 1) * 100 + 12;
                end = int.Parse(year) * 100 + 12;
            }
            string lenderCondition = lenders.Count() > 0 ? " and lender in @lenders " : "";

            string sql = $@"select * 
                        from dc_rpt_portfolio 
                        where (data_year * 100 + data_month >= @begin and data_year * 100 + data_month <= @end ) {lenderCondition} 
                        order by data_name, data_month, lender";

            string sqlLender = $@"select distinct lender 
                        from dc_rpt_portfolio 
                        where (data_year * 100 + data_month >= @begin and data_year * 100 + data_month <= @end ) {lenderCondition}";
            var dataSet = new PortfolioDBSet
            {
                lender = lender,
                lenders = lenders,
                lendersInDB = _db.Query<string>(sqlLender, new { begin = beginSelect, end = end, lenders = lenders }).ToList(),
                year = year,
                rows = _db.Query<PortfolioDBRow>(sql, new { begin = beginSelect, end = end, lenders = lenders }).ToList()
            };
            return dataSet;
        }

        private static string[] GetLenders(string lender)
        {
            if (string.IsNullOrEmpty(lender))
            {
                return new string[] { };
            }
            return lender.Split(new char[] { ',' });
        }
    }
}
