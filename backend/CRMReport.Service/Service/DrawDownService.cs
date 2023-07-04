using Dapper;
using log4net;
using Microsoft.EntityFrameworkCore;
using Report.Domain.Models.Common;
using Report.Domain.Models.CRM;
using Report.Infra.Data.Context;
using Report.Infra.Data.Utlis;
using Report.Infra.Data.Utlis.DrawDownProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CRMReport.Service
{
    public class DrawDownService : IDrawDownService
    {
        IDbConnection _mdb, _cdb, _db;
        private readonly DatacentreDBContext _dataCentreContext;
        private static DrawdownUpdatedDate drawdownUpdatedDate = null;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public DrawDownService(DatacentreDBContext context)
        {
            _mdb = new DbFactory(Config.MfappDatabase).db;
            _cdb = new DbFactory(Config.CrmDatabase).db;
            _db = new DbFactory(Config.DatacentreDatabase).db;
            _dataCentreContext = context;
        }

        public async Task<ServiceResponse<List<DrawdownData>>> GetDrawDownData()
        {
            var serviceResponse = new ServiceResponse<List<DrawdownData>>();
            try
            {
                var result = new List<DrawdownData>();
                var rawData = DrawDownHelper.LoadDbSet(true, _db);
                BusinessFormProvider.BusinessForm(rawData, result);
                BusinessApplicationFlowProvider.BusinessApplication(rawData, result);
                BusinessApplicationFlowPPTYProvider.BusinessApplicationFlowPPTY(rawData, result);
                AppQualityDeclaredProvider.AppQualityDeclared(rawData, result);
                AppQualityVerifiedProvider.AppQualityVerified(rawData, result);
                LendingServiceDedicatorProvider.LendingServiceDedicator(rawData, result);
                RetentionProvider.Retention(rawData, result);
                TotalDrawDownProvider.TotalDrawDown(rawData, result);
                BusinessProductsAnalysisProvider.Build(rawData, result);
                BusinessProposalProvider.BusinessProposal(rawData, result);
                ToProvider.To(rawData, result);
                FirstDrawProfileProvider.FirstDrawProfile(rawData, result);
                ChannelProvider.Channel(rawData, result);
                OtherProvider.Other(rawData, result);
                serviceResponse.Data = result;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                log.Error(ex.Message);
                var result = await CommonFunction.SaveActivityLog("GetDrawDownData", "GetDrawDownData", ex, _dataCentreContext);
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error Id: {result.Id}";
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<DrawdownUpdatedDate>> GetDrawdownUpdatedDate(bool force)
        {
            var serviceResponse = new ServiceResponse<DrawdownUpdatedDate>();
            try
            {
                DrawdownUpdatedDate drawdownUpdatedDate_mfapp = null;
                DrawdownUpdatedDate drawdownUpdatedDate_crm = null;
                if (drawdownUpdatedDate == null || force)
                {
                    string sql = $@"select max(CreatedDate) from crm_application";
                    drawdownUpdatedDate_mfapp = new DrawdownUpdatedDate
                    {
                        updatedDate = Convert.ToDateTime(await _mdb.QueryFirstOrDefaultAsync<DateTime>(sql))
                    };

                    string sqlQry = $@"select max(CreatedDate) from crm_application";
                    drawdownUpdatedDate_crm = new DrawdownUpdatedDate
                    {
                        updatedDate = Convert.ToDateTime(await _cdb.QueryFirstOrDefaultAsync<DateTime>(sql))
                    };
                }
                drawdownUpdatedDate = drawdownUpdatedDate_mfapp.updatedDate >= drawdownUpdatedDate_crm.updatedDate ? drawdownUpdatedDate_crm : drawdownUpdatedDate_mfapp;
                serviceResponse.Data = drawdownUpdatedDate;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                log.Error(ex.Message);
                var result = await CommonFunction.SaveActivityLog("GetDrawdownUpdatedDate", "GetDrawdownUpdatedDate", ex, _dataCentreContext);
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error Id: {result.Id}";
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<DrawdownDetailDTO>> GetDrawdownDetail(string data_name, int year, int month)
        {
            DrawdownDetailDTO detailDTO = null;
            var serviceResponse = new ServiceResponse<DrawdownDetailDTO>();
            try
            {
                var param = getQP(data_name, year, month);
                if (param != null)
                {
                    var data = _db.Query<dynamic>((string)param.sp, param.param,
                                commandType: CommandType.StoredProcedure).ToList();
                    detailDTO = new DrawdownDetailDTO
                    {
                        template = param.template,
                        detailData = data
                    };
                }
                serviceResponse.Data = detailDTO;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                log.Error(ex.Message);
                var result = await CommonFunction.SaveActivityLog("GetDrawdownDetail", "GetDrawdownDetail", ex, _dataCentreContext);
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error Id: {result.Id}";
            }
            return serviceResponse;
        }

        private static QueryParam getQP(string data_name, int year, int month)
        {
            switch (data_name)
            {
                case "Retention:Existing Client No.":
                    return new QueryParam
                    {
                        template = "loan",
                        sp = "sp_rpt_drawdown_retention_detail_Existing_Client_No",
                        param = new { p_year = year, p_month = month }
                    };
                case "Retention:Existing Contacts (No Loans) No.":
                    return new QueryParam
                    {
                        template = "loan",
                        sp = "sp_rpt_drawdown_retention_detail_Existing_Contact_No",
                        param = new { p_year = year, p_month = month }
                    };
                case "Total Draw-Down:1st Draw-Down no":
                    return new QueryParam
                    {
                        template = "loan",
                        sp = "sp_rpt_drawdown_total_draw_down_1st_Draw_Down_No",
                        param = new { p_year = year, p_month = month }
                    };
                case "Total Draw-Down:Sub Draw-down no":
                    return new QueryParam
                    {
                        template = "loan",
                        sp = "sp_rpt_drawdown_total_draw_down_Sub_Draw_Down_No",
                        param = new { p_year = year, p_month = month }
                    };
                case "Business Products Analysis:Fast Loan Under $10k:Draw-down No":
                    return new QueryParam
                    {
                        template = "loan",
                        sp = "sp_rpt_drawdown_business_products_analysis_draw_down_no",
                        param = new { p_year = year, p_month = month, p_min_value = 1000, p_max_value = 9999.99 }
                    };
                case "Business Products Analysis:Fast Bus $10k - $15k:Draw-down No":
                    return new QueryParam
                    {
                        template = "loan",
                        sp = "sp_rpt_drawdown_business_products_analysis_draw_down_no",
                        param = new { p_year = year, p_month = month, p_min_value = 10000, p_max_value = 15000 }
                    };
                case "Business Products Analysis:Premium $15k+:Draw-down No":
                    return new QueryParam
                    {
                        template = "loan",
                        sp = "sp_rpt_drawdown_business_products_analysis_draw_down_no",
                        param = new { p_year = year, p_month = month, p_min_value = 15000.01, p_max_value = 99999999 }
                    };
                case "Business Products Analysis:USB:Draw-down No":
                    return new QueryParam
                    {
                        template = "loan",
                        sp = "sp_rpt_drawdown_business_products_analysis_draw_down_no",
                        param = new { p_year = year, p_month = month, p_min_value = 0, p_max_value = 0 }
                    };
                case "Channel: Adwords":
                    return new QueryParam
                    {
                        template = "loan",
                        sp = "sp_rpt_drawdown_channel_detail_Adwords",
                        param = new { p_year = year, p_month = month }
                    };
                case "Channel: Email Outreach":
                    return new QueryParam
                    {
                        template = "loan",
                        sp = "sp_rpt_drawdown_channel_detail_Outreach",
                        param = new { p_year = year, p_month = month }
                    };
                case "Channel: Referrer":
                    return new QueryParam
                    {
                        template = "loan",
                        sp = "sp_rpt_drawdown_channel_detail_Referrer",
                        param = new { p_year = year, p_month = month }
                    };
                case "Channel: Other":
                    return new QueryParam
                    {
                        template = "loan",
                        sp = "sp_rpt_drawdown_channel_detail_Other",
                        param = new { p_year = year, p_month = month }
                    };
            }
            return null;
        }
    }
}
