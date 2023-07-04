using Dapper;
using log4net;
using Microsoft.EntityFrameworkCore;
using Report.Domain.Models.AM;
using Report.Domain.Models.Common;
using Report.Domain.Models.CRM;
using Report.Infra.Data.Context;
using Report.Infra.Data.Utlis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AMReport.Service
{
    public class LoanPerformanceService : ILoanPerformanceService
    {
        private readonly CrmDBContext _crmDBContext;
        private readonly DatacentreDBContext _dataCentreContext;
        IDbConnection _db;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public LoanPerformanceService(DatacentreDBContext dataCentreContext, CrmDBContext crmDBContext)
        {
            _db = new DbFactory(Config.DatacentreDatabase).db;
            _dataCentreContext = dataCentreContext;
            _crmDBContext = crmDBContext;
        }

        public  async Task<ServiceResponse<List<LoanPerformanceColumn>>> GetLoanPerformanceColumn(string loantype, string lenderList)
        {
            var serviceResponse = new ServiceResponse<List<LoanPerformanceColumn>>();
            try
            {
                string sql = $@"select distinct (CAST(CONCAT('month',monthno) as CHAR(25))) AS columnMonth from dc_rpt_loanperformance where typename = '{loantype}' and lender in ( {lenderList} ) order by monthno;";
                serviceResponse.Data = (List<LoanPerformanceColumn>)_db.Query<LoanPerformanceColumn>(sql);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                log.Error(ex.Message);
                var result = await CommonFunction.SaveActivityLog("GetLoanPerformanceColumn", "GetLoanPerformanceColumn", ex, _dataCentreContext);
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error Id: {result.Id}";
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<dynamic>>> GetLoanPerformanceData(string loantype, string lenderList, string amounttype)
        {
            var serviceResponse = new ServiceResponse<List<dynamic>>();
            DataSet dsloanmonthno;
            DataSet dsloanperformancedata;
            DataTable tbl = new DataTable();
            DataRow row;
            try
            {
                tbl.Columns.Add("LoanMonth");
                tbl.Columns.Add("LoanYear");
                var para = new[]
                {
                    DbHelper.SetParameter("@loantype",loantype),
                    DbHelper.SetParameter("@lender",lenderList),
                };
                //Get month column
                dsloanmonthno = DbHelper.ExecuteDataset(string.Format("select distinct monthno from dc_rpt_loanperformance where typename = '{0}' and lender in( {1} ) order by monthno", loantype, lenderList), para);
                foreach (DataRow drmonth in dsloanmonthno.Tables[0].Rows)
                {
                    tbl.Columns.Add("month" + drmonth["monthno"].ToString());
                }

                //Get loan performance data
                dsloanperformancedata = DbHelper.ExecuteDataset(string.Format("select * from dc_rpt_loanperformance where typename = '{0}' and lender in( {1} ) order by loanbook_year,loanbook_month * 1", loantype, lenderList), para);
                foreach (DataRow drloandata in dsloanperformancedata.Tables[0].Rows)
                {
                    row = tbl.NewRow();
                    var month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt32(drloandata["loanbook_month"].ToString()));
                    var loanmonth = month;
                    var loanyear = drloandata["loanbook_year"].ToString();
                    DataRow[] founddate = tbl.Select("LoanMonth = '" + loanmonth + "' and LoanYear = '" + loanyear + "'");
                    if (founddate.Length == 0)
                    {
                        row["LoanMonth"] = loanmonth;
                        row["LoanYear"] = loanyear;
                        foreach (DataColumn column in tbl.Columns)
                        {
                            if (column.ColumnName == "month" + drloandata["monthno"].ToString())
                            {
                                if (amounttype == "CashAmount")
                                {
                                    if (string.IsNullOrEmpty(drloandata["cashamount_rate"].ToString()))
                                        row["month" + drloandata["monthno"].ToString()] = string.Empty;
                                    else
                                        row["month" + drloandata["monthno"].ToString()] = Math.Round((Convert.ToDouble(drloandata["cashamount_rate"].ToString()) * 100)) + "%";
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(drloandata["type_rate"].ToString()))
                                        row["month" + drloandata["monthno"].ToString()] = string.Empty;
                                    else
                                        row["month" + drloandata["monthno"].ToString()] = Math.Round((Convert.ToDouble(drloandata["type_rate"].ToString()) * 100)) + "%";
                                }

                                break;
                            }
                        }
                        tbl.Rows.Add(row);
                    }
                    else
                    {
                        foreach (DataColumn column in tbl.Columns)
                        {
                            if (column.ColumnName == "month" + drloandata["monthno"].ToString())
                            {
                                if (amounttype == "CashAmount")
                                {
                                    if (string.IsNullOrEmpty(drloandata["cashamount_rate"].ToString()))
                                        founddate[0]["month" + drloandata["monthno"].ToString()] = string.Empty;
                                    else
                                        founddate[0]["month" + drloandata["monthno"].ToString()] = Math.Round((Convert.ToDouble(drloandata["cashamount_rate"].ToString()) * 100)) + "%";
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(drloandata["type_rate"].ToString()))
                                        founddate[0]["month" + drloandata["monthno"].ToString()] = string.Empty;
                                    else
                                        founddate[0]["month" + drloandata["monthno"].ToString()] = Math.Round((Convert.ToDouble(drloandata["type_rate"].ToString()) * 100)) + "%";
                                }

                                break;
                            }
                        }
                    }
                }

                //Calculate Average of loan performance
                DataRow blankrow = tbl.NewRow();
                tbl.Rows.Add(blankrow);
                DataRow avgmonth = tbl.NewRow();
                foreach (DataColumn column in tbl.Columns)
                {
                    double totalNumber = 0.0;
                    int countmonth = 0;
                    if (tbl.Columns.IndexOf(column) > 1)
                    {
                        foreach (DataRow totalRow in tbl.Rows)
                        {
                            if (!string.IsNullOrEmpty(totalRow[column.ColumnName].ToString()))
                            {
                                countmonth += 1;
                                totalNumber = totalNumber + Convert.ToDouble(totalRow[column.ColumnName].ToString().Replace("%", ""));
                            }
                        }
                        avgmonth[column.ColumnName] = "<span style=\"font-weight:bold;color:Red\">" + Math.Round((totalNumber / countmonth), 2) + "%" + "</span>";
                        avgmonth[column.ColumnName] = Math.Round((totalNumber / countmonth), 2) + "%";
                    }
                    else
                    {
                        avgmonth["LoanMonth"] = "<span style=\"font-weight:bold;color:Red\">" + "Average" + "</span>";
                        avgmonth["LoanMonth"] = "Average";
                        avgmonth["LoanYear"] = "";
                    }
                }
                tbl.Rows.Add(avgmonth);
                serviceResponse.Data = ConvertDataTableToList(tbl);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                log.Error(ex.Message);
                var result = await CommonFunction.SaveActivityLog("GetLoanPerformanceData", "GetLoanPerformanceData", ex, _dataCentreContext);
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error Id: {result.Id}";
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<StatisticColumn>>> GetStatisticLoanPerformanceColumn(string loantype, string lenderList)
        {
            var serviceResponse = new ServiceResponse<List<StatisticColumn>>();
            try
            {
                string sql = $@"SELECT distinct loanbook_year as year, upper(date_format(concat(loanbook_year, '-',loanbook_month,'-','01'), '%b')) as month from dc_rpt_loanperformance where loanbook_year*12+loanbook_month>=2016*12+12;";
                serviceResponse.Data = _db.Query<StatisticColumn>(sql,
                    new
                    {
                        p_loantype = loantype,
                        p_lender = lenderList
                    }).ToList();
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                log.Error(ex.Message);
                var result = await CommonFunction.SaveActivityLog("GetStatisticLoanPerformanceColumn", "GetStatisticLoanPerformanceColumn", ex, _dataCentreContext);
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error Id: {result.Id}";
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<dynamic>>> GetStatisticLoanPerformanceData(string loantype, string lender, string amounttype, int monthno)
        {
            var serviceResponse = new ServiceResponse<List<dynamic>>();
            DataTable tbl = new DataTable();
            try
            {
                DataRow row;
                DataTable dtloanperformancedata = new DataTable();
                var para = new[]
                {
                    DbHelper.SetParameter("@p_loantype",loantype),
                    DbHelper.SetParameter("@p_lender",lender),
                    DbHelper.SetParameter("@p_amounttype",amounttype),
                    DbHelper.SetParameter("@p_monthno",monthno)
                };
                DataTable dtloanperformance = DbHelper.ExecuteDatasetSP("sp_rpt_loan_performance_statistics_get", para);

                tbl.Columns.Add("StatisticType");
                dtloanperformancedata.Columns.Add("year");
                dtloanperformancedata.Columns.Add("month");
                dtloanperformancedata.Columns.Add("overallavg");
                dtloanperformancedata.Columns.Add("lastmonthavg");
                dtloanperformancedata.Columns.Add("currmonthavg");
                dtloanperformancedata.Columns.Add("difference");
                dtloanperformancedata.Columns.Add("differenceper");

                if (monthno == 0)
                {
                    foreach (DataRow dr in dtloanperformance.Rows)
                    {
                        tbl.Columns.Add(dr["mnth"].ToString() + " " + dr["yr"].ToString());

                        row = dtloanperformancedata.NewRow();
                        row["year"] = dr["y"].ToString();
                        row["month"] = dr["m"].ToString();
                        row["overallavg"] = dr["overall_type_rate_all"].ToString();
                        row["lastmonthavg"] = dr["last_month_type_rate_all"].ToString();
                        row["currmonthavg"] = dr["this_month_type_rate_all"].ToString();
                        row["difference"] = dr["difference_month_type_rate_all"].ToString();
                        row["differenceper"] = dr["differenceper_month_type_rate_all"].ToString();
                        dtloanperformancedata.Rows.Add(row);
                    }
                }
                else
                {
                    foreach (DataRow dr in dtloanperformance.Rows)
                    {
                        tbl.Columns.Add(dr["mnth"].ToString() + " " + dr["yr"].ToString());

                        row = dtloanperformancedata.NewRow();
                        row["year"] = dr["y"].ToString();
                        row["month"] = dr["m"].ToString();
                        row["overallavg"] = dr["overall_type_rate"].ToString();
                        row["lastmonthavg"] = dr["last_month_type_rate"].ToString();
                        row["currmonthavg"] = dr["this_month_type_rate"].ToString();
                        row["difference"] = dr["difference_month_type_rate"].ToString();
                        row["differenceper"] = dr["differenceper_month_type_rate"].ToString();
                        dtloanperformancedata.Rows.Add(row);
                    }
                }

                //Get loan performance data
                string[] type = new string[5] { "Overall Average", "Last Month Average", "This Month Average", "Difference", "Difference (%)" };
                string[] typecondition = new string[5] { "overallavg", "lastmonthavg", "currmonthavg", "difference", "differenceper" };
                for (int i = 0; i < type.Length; i++)
                {
                    row = tbl.NewRow();
                    row["StatisticType"] = type[i];
                    foreach (DataColumn column in tbl.Columns)
                    {
                        if (column.ColumnName != "StatisticType")
                        {
                            string[] duration = column.ColumnName.Split(' ');
                            DataRow[] founddata = dtloanperformancedata.Select("year = '" + duration[1] + "' and month = '" + DateTime.ParseExact(duration[0], "MMM", CultureInfo.CurrentCulture).Month + "'");
                            row[column.ColumnName] = founddata[0][typecondition[i]].ToString();
                        }
                    }
                    tbl.Rows.Add(row);
                }

                serviceResponse.Data = ConvertDataTableToList(tbl);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                log.Error(ex.Message);
                var result = await CommonFunction.SaveActivityLog("GetStatisticLoanPerformanceData", "GetStatisticLoanPerformanceData", ex, _dataCentreContext);
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error Id: {result.Id}";
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<LoanPerformanceExportDetail>>> GetLoanPerformanceExportDetail(string loantype, string lenderList, string amounttype)
        {
            var serviceResponse = new ServiceResponse<List<LoanPerformanceExportDetail>>();
            LoanPerformanceExportDetail loanPerformanceExport;
            List<LoanPerformanceExportDetail> lstLoanPerformance = new List<LoanPerformanceExportDetail>();

            try
            {
                #region loan performance

                DataSet dsloanmonthno;
                DataSet dsloanperformancedata;
                DataTable tbl = new DataTable();
                DataRow row;

                tbl.Columns.Add("LoanMonth");
                tbl.Columns.Add("LoanYear");
                var para = new[]
                {
                    DbHelper.SetParameter("@loantype",loantype),
                    DbHelper.SetParameter("@lender",lenderList),
                };
                //Get month column
                dsloanmonthno = DbHelper.ExecuteDataset(string.Format("select distinct monthno from dc_rpt_loanperformance where typename = '{0}' and lender in( {1} ) order by monthno", loantype, lenderList), para);
                foreach (DataRow drmonth in dsloanmonthno.Tables[0].Rows)
                {
                    tbl.Columns.Add("month" + drmonth["monthno"].ToString());
                }

                //Get loan performance data
                dsloanperformancedata = DbHelper.ExecuteDataset(string.Format("select * from dc_rpt_loanperformance where typename = '{0}' and lender in( {1} ) order by loanbook_year,loanbook_month * 1", loantype, lenderList), para);
                foreach (DataRow drloandata in dsloanperformancedata.Tables[0].Rows)
                {
                    row = tbl.NewRow();
                    var month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt32(drloandata["loanbook_month"].ToString()));
                    var loanmonth = month;
                    var loanyear = drloandata["loanbook_year"].ToString();
                    DataRow[] founddate = tbl.Select("LoanMonth = '" + loanmonth + "' and LoanYear = '" + loanyear + "'");
                    if (founddate.Length == 0)
                    {
                        row["LoanMonth"] = loanmonth;
                        row["LoanYear"] = loanyear;
                        foreach (DataColumn column in tbl.Columns)
                        {
                            if (column.ColumnName == "month" + drloandata["monthno"].ToString())
                            {
                                if (amounttype == "CashAmount")
                                {
                                    if (string.IsNullOrEmpty(drloandata["cashamount_rate"].ToString()))
                                        row["month" + drloandata["monthno"].ToString()] = string.Empty;
                                    else
                                        row["month" + drloandata["monthno"].ToString()] = Math.Round(Convert.ToDouble(drloandata["cashamount_rate"].ToString()) * 100) + "%";
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(drloandata["type_rate"].ToString()))
                                        row["month" + drloandata["monthno"].ToString()] = string.Empty;
                                    else
                                        row["month" + drloandata["monthno"].ToString()] = Math.Round(Convert.ToDouble(drloandata["type_rate"].ToString()) * 100) + "%";
                                }

                                break;
                            }
                        }                    
                        tbl.Rows.Add(row);
                    }
                    else
                    {
                        foreach (DataColumn column in tbl.Columns)
                        {
                            if (column.ColumnName == "month" + drloandata["monthno"].ToString())
                            {
                                if (amounttype == "CashAmount")
                                {
                                    if (string.IsNullOrEmpty(drloandata["cashamount_rate"].ToString()))
                                        founddate[0]["month" + drloandata["monthno"].ToString()] = string.Empty;
                                    else
                                        founddate[0]["month" + drloandata["monthno"].ToString()] = Math.Round(Convert.ToDouble(drloandata["cashamount_rate"].ToString()) * 100) + "%";
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(drloandata["type_rate"].ToString()))
                                        founddate[0]["month" + drloandata["monthno"].ToString()] = string.Empty;
                                    else
                                        founddate[0]["month" + drloandata["monthno"].ToString()] = Math.Round(Convert.ToDouble(drloandata["type_rate"].ToString()) * 100) + "%";
                                }

                                break;
                            }
                        }
                    }
                }

                //Calculate Average of loan performance
                DataRow blankrow = tbl.NewRow();
                tbl.Rows.Add(blankrow);
                DataRow avgmonth = tbl.NewRow();
                foreach (DataColumn column in tbl.Columns)
                {
                    double totalNumber = 0.0;
                    int countmonth = 0;
                    if (tbl.Columns.IndexOf(column) > 1)
                    {
                        foreach (DataRow totalRow in tbl.Rows)
                        {
                            if (!string.IsNullOrEmpty(totalRow[column.ColumnName].ToString()))
                            {
                                countmonth += 1;
                                totalNumber = totalNumber + Convert.ToDouble(totalRow[column.ColumnName].ToString().Replace("%", ""));
                            }
                        }
                        avgmonth[column.ColumnName] = "<span style=\"font-weight:bold;color:Red\">" + Math.Round((totalNumber / countmonth), 2) + "%" + "</span>";
                        avgmonth[column.ColumnName] = Math.Round((totalNumber / countmonth), 2) + "%";
                    }
                    else
                    {
                        avgmonth["LoanMonth"] = "<span style=\"font-weight:bold;color:Red\">" + "Average" + "</span>";
                        avgmonth["LoanMonth"] = "Average";
                        avgmonth["LoanYear"] = "";
                    }
                }
                tbl.Rows.Add(avgmonth);
                loanPerformanceExport = new LoanPerformanceExportDetail
                {
                    sheetName = "Loan Performance",
                    dataTable = tbl                       
                };
                lstLoanPerformance.Add(loanPerformanceExport);

                #endregion


                #region statistics loan performance

                DataTable dtstatisticloanperformancedata = new DataTable();
                DataTable tbl1 = new DataTable();
                DataRow row1;
                int[] lstMonthNo = new int[4] { 0,12,18,24 };
                foreach(int monthNo in lstMonthNo)
                {
                    var para1 = new[]
                    {
                        DbHelper.SetParameter("@p_loantype",loantype),
                        DbHelper.SetParameter("@p_lender",lenderList),
                        DbHelper.SetParameter("@p_amounttype",amounttype),
                        DbHelper.SetParameter("@p_monthno",monthNo)
                    };

                    DataTable dtstatisticloanperformance = DbHelper.ExecuteDatasetSP("sp_rpt_loan_performance_statistics_get", para1);

                    tbl1 = new DataTable();
                    dtstatisticloanperformancedata = new DataTable();
                    tbl1.Columns.Add("StatisticType");
                    dtstatisticloanperformancedata.Columns.Add("year");
                    dtstatisticloanperformancedata.Columns.Add("month");
                    dtstatisticloanperformancedata.Columns.Add("overallavg");
                    dtstatisticloanperformancedata.Columns.Add("lastmonthavg");
                    dtstatisticloanperformancedata.Columns.Add("currmonthavg");
                    dtstatisticloanperformancedata.Columns.Add("difference");
                    dtstatisticloanperformancedata.Columns.Add("differenceper");

                    if (monthNo == 0)
                    {
                        foreach (DataRow dr in dtstatisticloanperformance.Rows)
                        {

                            tbl1.Columns.Add(dr["mnth"].ToString() + " " + dr["yr"].ToString());

                            row1 = dtstatisticloanperformancedata.NewRow();
                            row1["year"] = dr["y"].ToString();
                            row1["month"] = dr["m"].ToString();
                            row1["overallavg"] = dr["overall_type_rate_all"].ToString();
                            row1["lastmonthavg"] = dr["last_month_type_rate_all"].ToString();
                            row1["currmonthavg"] = dr["this_month_type_rate_all"].ToString();
                            row1["difference"] = dr["difference_month_type_rate_all"].ToString();
                            row1["differenceper"] = dr["differenceper_month_type_rate_all"].ToString();
                            dtstatisticloanperformancedata.Rows.Add(row1);
                        }
                    }
                    else
                    {
                        foreach (DataRow dr in dtstatisticloanperformance.Rows)
                        {
                            tbl1.Columns.Add(dr["mnth"].ToString() + " " + dr["yr"].ToString());

                            row1 = dtstatisticloanperformancedata.NewRow();
                            row1["year"] = dr["y"].ToString();
                            row1["month"] = dr["m"].ToString();
                            row1["overallavg"] = dr["overall_type_rate"].ToString();
                            row1["lastmonthavg"] = dr["last_month_type_rate"].ToString();
                            row1["currmonthavg"] = dr["this_month_type_rate"].ToString();
                            row1["difference"] = dr["difference_month_type_rate"].ToString();
                            row1["differenceper"] = dr["differenceper_month_type_rate"].ToString();
                            dtstatisticloanperformancedata.Rows.Add(row1);
                        }
                    }

                    string[] type = new string[5] { "Overall Average", "Last Month Average", "This Month Average", "Difference", "Difference (%)" };
                    string[] typecondition = new string[5] { "overallavg", "lastmonthavg", "currmonthavg", "difference", "differenceper" };
                    for (int i = 0; i < type.Length; i++)
                    {
                        row1 = tbl1.NewRow();
                        row1["StatisticType"] = type[i];
                        foreach (DataColumn column in tbl1.Columns)
                        {
                            if (column.ColumnName != "StatisticType")
                            {
                                string[] duration = column.ColumnName.Split(' ');
                                DataRow[] founddata = dtstatisticloanperformancedata.Select("year = '" + duration[1] + "' and month = '" + DateTime.ParseExact(duration[0], "MMM", CultureInfo.CurrentCulture).Month + "'");
                                row1[column.ColumnName] = founddata[0][typecondition[i]].ToString();
                            }
                        }
                        tbl1.Rows.Add(row1);                    
                    }
                     loanPerformanceExport = new LoanPerformanceExportDetail
                        {
                            sheetName = "Statistics" + monthNo,
                            dataTable = tbl1
                        };
                    lstLoanPerformance.Add(loanPerformanceExport);
                }

                #endregion
                serviceResponse.Data = lstLoanPerformance;             
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                log.Error(ex.Message);
                var result = await CommonFunction.SaveActivityLog("GetLoanPerformanceExportDetail", "GetLoanPerformanceExportDetail", ex, _dataCentreContext);
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error Id: {result.Id}";
            }
            return serviceResponse;
        }


        private List<dynamic> ConvertDataTableToList(DataTable dt)
        {
            var dynamicDt = new List<dynamic>();
            foreach (DataRow row in dt.Rows)
            {
                dynamic dyn = new ExpandoObject();
                dynamicDt.Add(dyn);
                foreach (DataColumn column in dt.Columns)
                {
                    var dic = (IDictionary<string, object>)dyn;
                    
                    if (!string.IsNullOrWhiteSpace(row[column].ToString()))
                    {
                        dic[column.ColumnName] = row[column];
                    } else
                    {
                        dic[column.ColumnName] = null;
                    }
                }
            }
            return dynamicDt;
        }
    }
}
