using Dapper;
using log4net;
using Microsoft.EntityFrameworkCore;
using Report.Domain.Models.AM;
using Report.Domain.Models.Common;
using Report.Domain.Models.RMS;
using Report.Infra.Data.Context;
using Report.Infra.Data.Utlis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AMReport.Service
{
    public class PortfolioSummaryService : IPortfolioSummaryService
    {
        IDbConnection _db;
        private readonly DatacentreDBContext _dataCentreContext;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public PortfolioSummaryService(DatacentreDBContext context)
        {
            _db = new DbFactory(Config.DatacentreDatabase).db;
            _dataCentreContext = context;
        }

        public async Task<ServiceResponse<List<PortfolioSummaryData>>> GetPortfolioSummary(int selectedYear, int selectedMonth, string lender, bool isSeparateTable)
        {
            var serviceResponse = new ServiceResponse<List<PortfolioSummaryData>>();
            string sql = string.Empty;
            PortfolioSummaryData portfoliosummarydata;
            List<PortfolioSummaryData> lstPortfolioSummary = new List<PortfolioSummaryData>();
            bool isActiveLoan = false;
            int totalNumber = 0;
            decimal totalCashAmount = 0;
            decimal totalRepaidAmount = 0;
            decimal totalExpectedAmount = 0;
            try
            {
                string[] loanStatusList = new string[6] { "Repaying", "Default", "Recovery", "Close", "Settled", "Written Off" };
                foreach (string loanStatus in loanStatusList)
                {
                    if (lender == "All")
                    {
                        sql = $@"SELECT data_year, data_month,COUNT(count) AS count,IFNULL(CAST((SUM(cashamount)) as DECIMAL(10,2)),'0.0') AS cashamount, IFNULL(CAST(SUM(repaidamount) as DECIMAL(18,2)),'0.0') AS repaidamount, 
                                    IFNULL(CAST(SUM(expectedamount) as DECIMAL(18,2)),'0.0') AS expectedamount, status, lender, update_date FROM dc_rpt_portfoliosummary 
                                    WHERE status = '{loanStatus}' AND data_year = {selectedYear} AND data_month = {selectedMonth} 
                                    ORDER BY id DESC;";
                    }
                    else
                    {
                        sql = $@"SELECT data_year, data_month,COUNT(count) AS count,IFNULL(CAST((SUM(cashamount)) as DECIMAL(10,2)),'0.0') AS cashAmount, IFNULL(CAST(SUM(repaidamount) as DECIMAL(18,2)),'0.0') AS repaidamount, 
                                    IFNULL(CAST(SUM(expectedamount) as DECIMAL(18,2)),'0.0') AS expectedamount, status, lender, update_date FROM dc_rpt_portfoliosummary 
                                    WHERE status = '{loanStatus}' AND data_year = {selectedYear} AND data_month = {selectedMonth} AND lender = '{lender}'
                                    ORDER BY id DESC;";
                    }
                    portfoliosummarydata = new PortfolioSummaryData();
                    portfoliosummarydata = await _db.QueryFirstOrDefaultAsync<PortfolioSummaryData>(sql);
                    portfoliosummarydata.status = loanStatus.ToUpper();
                    portfoliosummarydata.discrepancy = (portfoliosummarydata.repaidamount - portfoliosummarydata.expectedamount);
                    if (portfoliosummarydata.status == "REPAYING" || portfoliosummarydata.status == "DEFAULT" || portfoliosummarydata.status == "RECOVERY")
                    {
                        isActiveLoan = true;
                        portfoliosummarydata.isActiveLoan = isActiveLoan ? 1 : 0;
                    }
                    lstPortfolioSummary.Add(portfoliosummarydata);
                }

                if (isSeparateTable == true)
                {
                    foreach (var listData in lstPortfolioSummary)
                    {
                        if (listData.count > 0 && listData.isActiveLoan == 1)
                        {
                            totalNumber = totalNumber + listData.count;
                            totalCashAmount = totalCashAmount + listData.cashamount;
                            totalRepaidAmount = totalRepaidAmount + listData.repaidamount;
                            totalExpectedAmount = totalExpectedAmount + listData.expectedamount;
                        }
                    }
                    List<PortfolioSummaryData> activeLoans = CalculatePercentageForPortfolioReport(lstPortfolioSummary, totalNumber, totalCashAmount, totalRepaidAmount, totalExpectedAmount, 1, isSeparateTable);
                    totalNumber = 0;
                    totalCashAmount = 0;
                    totalRepaidAmount = 0;
                    totalExpectedAmount = 0;

                    foreach (var data in lstPortfolioSummary)
                    {
                        if (data.count > 0 && data.isActiveLoan == 0)
                        {
                            totalNumber = totalNumber + data.count;
                            totalCashAmount = totalCashAmount + data.cashamount;
                            totalRepaidAmount = totalRepaidAmount + data.repaidamount;
                            totalExpectedAmount = totalExpectedAmount + data.expectedamount;
                        }
                    }
                    List<PortfolioSummaryData> inActiveLoans = CalculatePercentageForPortfolioReport(lstPortfolioSummary, totalNumber, totalCashAmount, totalRepaidAmount, totalExpectedAmount, 0, isSeparateTable);
                    serviceResponse.Data = activeLoans.Concat(inActiveLoans).ToList();
                }
                else
                {
                    foreach (var data in lstPortfolioSummary)
                    {
                        if (data.count > 0)
                        {
                            totalNumber = totalNumber + data.count;
                            totalCashAmount = totalCashAmount + data.cashamount;
                            totalRepaidAmount = totalRepaidAmount + data.repaidamount;
                            totalExpectedAmount = totalExpectedAmount + data.expectedamount;
                        }
                    }

                    lstPortfolioSummary = CalculatePercentageForPortfolioReport(lstPortfolioSummary, totalNumber, totalCashAmount, totalRepaidAmount, totalExpectedAmount, 1 , isSeparateTable);
                }

                serviceResponse.Data = lstPortfolioSummary;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                log.Error(ex.Message);
                var result = await CommonFunction.SaveActivityLog("GetPortfolioSummary", "GetPortfolioSummary", ex, _dataCentreContext);
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error Id: {result.Id}";
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<PortfolioSummaryLatestDrawDownDate>> GetPortfolioSummaryLatestDrawDownDate(int selectedYear, int selectedMonth, string lender)
        {
            var serviceResponse = new ServiceResponse<PortfolioSummaryLatestDrawDownDate>();
            string startDate = string.Empty;
            string finishDate = string.Empty;
            try
            {
                startDate = string.Concat(selectedYear, '-', selectedMonth, '-', 01);
                finishDate = string.Concat(selectedYear, '-', selectedMonth, '-', DateTime.DaysInMonth(selectedYear, selectedMonth));

                serviceResponse.Data = await _db.QueryFirstOrDefaultAsync<PortfolioSummaryLatestDrawDownDate>("sp_rpt_portfolio_summary_latest_drawdown_date",
                    new
                    {
                        p_startDate = startDate,
                        p_endDate = finishDate,
                        p_lender = lender
                    }, null, null, CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                log.Error(ex.Message);
                var result = await CommonFunction.SaveActivityLog("GetPortfolioSummaryLatestDrawDownDate", "GetPortfolioSummaryLatestDrawDownDate", ex, _dataCentreContext);
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error Id: {result.Id}";
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<PortfolioSummaryDetail>>> GetPortfolioSummaryDetail(int selectedYear, int selectedMonth, string lender,  string loanstatus, bool isSeparateTable, string sortColumn, string sortOrder)
        {
            var serviceResponse = new ServiceResponse<List<PortfolioSummaryDetail>>();
            string startDate = string.Empty;
            string finishDate = string.Empty;
            IEnumerable<PortfolioSummaryDetail> detailList;
            try
            {
                startDate = string.Concat(selectedYear, '-', selectedMonth, '-', 01);
                finishDate = string.Concat(selectedYear, '-', selectedMonth, '-', DateTime.DaysInMonth(selectedYear, selectedMonth));

                detailList = await _db.QueryAsync<PortfolioSummaryDetail>("sp_rpt_portfolio_summary_detail_data",
                    new
                    {
                        p_startDate = startDate,
                        p_endDate = finishDate,
                        p_status = loanstatus,
                        p_lender = lender,
                        p_sortColumn = sortColumn,
                        p_sortOrder = sortOrder
                    }, null, null, CommandType.StoredProcedure);

                serviceResponse.Data = detailList.ToList();
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                log.Error(ex.Message);
                var result = await CommonFunction.SaveActivityLog("GetPortfolioSummaryDetail", "GetPortfolioSummaryDetail", ex, _dataCentreContext);
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error Id: {result.Id}";
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<PortfolioSummaryExportDetail>> GetPortfolioSummaryExportDetail(int selectedYear, int selectedMonth, string lender)
        {
            var serviceResponse = new ServiceResponse<PortfolioSummaryExportDetail>();
            string startDate = string.Empty;
            string finishDate = string.Empty;
            IEnumerable<PortfolioSummaryExport> lstPortfolioSummaryDetail;
            try
            {
                startDate = string.Concat(selectedYear, '-', selectedMonth, '-', 01);
                finishDate = string.Concat(selectedYear, '-', selectedMonth, '-', DateTime.DaysInMonth(selectedYear, selectedMonth));
                lstPortfolioSummaryDetail = await _db.QueryAsync<PortfolioSummaryExport>("sp_rpt_portfolio_summary_export",
                   new
                   {
                       p_startDate = startDate,
                       p_endDate = finishDate,
                       p_lender = lender
                   }, null, null, CommandType.StoredProcedure);

                serviceResponse.Data = new PortfolioSummaryExportDetail
                {
                    sheetName = "Portfolio Summary",
                    dataTable = ConvertToDataTable(lstPortfolioSummaryDetail.ToList())
                };
                    
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                log.Error(ex.Message);
                var result = await CommonFunction.SaveActivityLog("GetPortfolioSummaryExportDetail", "GetPortfolioSummaryExportDetail", ex, _dataCentreContext);
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error Id: {result.Id}";
            }
            return serviceResponse;
        }

        private static DataTable ConvertToDataTable<T>(IList<T> list)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in list)
            {
                for (int i = 0; i < values.Length; i++)
                    values[i] = props[i].GetValue(item) ?? DBNull.Value;
                table.Rows.Add(values);
            }
            return table;
        }

        private static List<PortfolioSummaryData> CalculatePercentageForPortfolioReport(List<PortfolioSummaryData> portfolioSummaryData, int totalNumber, decimal totalCashAmount, decimal totalRepaidAmount, decimal totalExpectedAmount, int isActiveLoan, bool isSeparateTable)
        {
            List<PortfolioSummaryData> listPortfolioSummary = new List<PortfolioSummaryData>();
            try
            {
                if (isSeparateTable == true)
                {
                    foreach (var data in portfolioSummaryData)
                    {
                        if (data.isActiveLoan == isActiveLoan)
                        {
                            double percentage = 0;
                            if (data.count != 0 && totalNumber != 0)
                                percentage = Math.Round((double)data.count / totalNumber * 100, 2);
                            data.percentageCount = percentage;

                            if (data.cashamount != 0 && totalCashAmount != 0)
                                percentage = Math.Round((double)data.cashamount / (double)totalCashAmount * 100, 2);
                            else
                                percentage = 0;
                            data.percentageCashAmount = percentage;


                            if (data.repaidamount != 0 && totalRepaidAmount != 0)
                                percentage = Math.Round((double)data.repaidamount / (double)totalRepaidAmount * 100, 2);
                            else
                                percentage = 0;
                            data.percentageRepaidAmount = percentage;

                            listPortfolioSummary.Add(data);
                        }
                    }
                }

                else
                {
                    foreach (var resultData in portfolioSummaryData)
                    {
                        double percentage = 0;
                        if (resultData.count != 0 && totalNumber != 0)
                            percentage = Math.Round((double)resultData.count / totalNumber * 100, 2);
                        resultData.percentageCount = percentage;

                        if (resultData.cashamount != 0 && totalCashAmount != 0)
                            percentage = Math.Round((double)resultData.cashamount / (double)totalCashAmount * 100, 2);
                        else
                            percentage = 0;
                        resultData.percentageCashAmount = percentage;

                        if (resultData.repaidamount != 0 && totalRepaidAmount != 0)
                            percentage = Math.Round((double)resultData.repaidamount / (double)totalRepaidAmount * 100, 2);
                        else
                            percentage = 0;
                        resultData.percentageRepaidAmount = percentage;

                        listPortfolioSummary.Add(resultData);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listPortfolioSummary;
        }
    }
}
