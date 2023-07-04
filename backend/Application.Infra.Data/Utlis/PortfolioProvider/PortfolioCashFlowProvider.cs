using Report.Domain.Models.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Infra.Data.Utlis.PortfolioProvider
{
    public class PortfolioCashFlowProvider
    {
        public static void PortfolioCashFlow(PortfolioDBSet rawDataSet, List<PortfolioReportData> Output)
        {
            #region Value

            Output.Add(PortfolioHelper.GroupRow(rawDataSet, "PORTFOLIO - CASH FLOW", "TopRow", "3 PORTFOLIO - CASH FLOW"));
            Output.Add(PortfolioHelper.BlankRow(rawDataSet, "PORTFOLIO - CASH FLOW", "Split0"));
            Output.Add(PortfolioHelper.GroupRow(rawDataSet, "PORTFOLIO - CASH FLOW:Value", "Value", "Value", labelCss: "L_right L_bold"));
            Output.Add(PortfolioHelper.BlankRow(rawDataSet, "PORTFOLIO - CASH FLOW", "Split1"));
            Output.Add(PortfolioHelper.GroupRow(rawDataSet, "PORTFOLIO - CASH FLOW:Value:Highest Single Payment", "Highest Single Payment", "Highest Single Payment", labelCss: "L_right L_bold"));
            Output.Add(PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - CASH FLOW:Value:Highest Single Payment:Direct Debit", labelCss: "L_right L_italic L_gray", dataFormat: "currency", label: "Direct Debit"));
            Output.Add(PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - CASH FLOW:Value:Highest Single Payment:Cash", labelCss: "L_right L_italic L_gray", dataFormat: "currency", label: "Cash"));
            Output.Add(PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - CASH FLOW:Value:Highest Single Payment:Card", labelCss: "L_right L_italic L_gray", dataFormat: "currency", label: "Card"));
            #region Total Payment Due

            Output.Add(PortfolioHelper.BlankRow(rawDataSet, "PORTFOLIO - CASH FLOW", "Split3"));
            var row_v_d_d = PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - CASH FLOW:Value:Total Payment Due:Direct Debit", labelCss: "L_right L_italic L_gray", dataFormat: "currency", label: "Direct Debit");
            var row_v_d_s = PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - CASH FLOW:Value:Total Payment Due:Cash", labelCss: "L_right L_italic L_gray", dataFormat: "currency", label: "Cash");
            var row_v_d_c = PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - CASH FLOW:Value:Total Payment Due:Card", labelCss: "L_right L_italic L_gray", dataFormat: "currency", label: "Card");
            PortfolioReportData row_v_d_sum = PortfolioHelper.SumRow(PortfolioHelper.SumRow(row_v_d_d, row_v_d_s), row_v_d_c);
            row_v_d_sum.year = rawDataSet.year;
            row_v_d_sum.lender = rawDataSet.lender;
            row_v_d_sum.data_name = "PORTFOLIO - CASH FLOW:Value:Total Payment Due";
            row_v_d_sum.label = "Total Payment Due";
            row_v_d_sum.labelCss = "L_right L_bold";
            row_v_d_sum.dataFormat = "currency";
            Output.Add(row_v_d_sum);
            Output.Add(row_v_d_d);
            Output.Add(row_v_d_s);
            Output.Add(row_v_d_c);

            #endregion

            #region Received
            Output.Add(PortfolioHelper.BlankRow(rawDataSet, "PORTFOLIO - CASH FLOW", "Split4"));
            var row_v_r_d = PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - CASH FLOW:Value:Received:Direct Debit", labelCss: "L_right L_italic L_gray", dataFormat: "currency", label: "Direct Debit");
            var row_v_r_s = PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - CASH FLOW:Value:Received:Cash", labelCss: "L_right L_italic L_gray", dataFormat: "currency", label: "Cash");
            var row_v_r_c = PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - CASH FLOW:Value:Received:Card", labelCss: "L_right L_italic L_gray", dataFormat: "currency", label: "Card");
            PortfolioReportData row_v_r_sum = PortfolioHelper.SumRow(PortfolioHelper.SumRow(row_v_r_d, row_v_r_s), row_v_r_c);
            row_v_r_sum.year = rawDataSet.year;
            row_v_r_sum.lender = rawDataSet.lender;
            row_v_r_sum.data_name = "PORTFOLIO - CASH FLOW:Value:Received";
            row_v_r_sum.label = "Received";
            row_v_r_sum.labelCss = "L_right L_bold";
            row_v_r_sum.dataFormat = "currency";
            Output.Add(row_v_r_sum);
            Output.Add(PortfolioHelper.GetRunningBalanceRow(row_v_r_sum, labelCss: "L_right L_italic"));
            Output.Add(row_v_r_d);
            Output.Add(row_v_r_s);
            Output.Add(row_v_r_c);
            Output.Add(PortfolioHelper.BlankRow(rawDataSet, "PORTFOLIO - CASH FLOW", "Split5"));
            #endregion

            #region Payment Success Rate
            var row_sr_v_sum = PortfolioHelper.DivRow(row_v_r_sum, row_v_d_sum);
            row_sr_v_sum.year = rawDataSet.year;
            row_sr_v_sum.lender = rawDataSet.lender;
            row_sr_v_sum.data_name = "PORTFOLIO - CASH FLOW:Value:Payment Success Rate";
            row_sr_v_sum.label = "Payment Success Rate";
            row_sr_v_sum.labelCss = "L_right L_bold";
            row_sr_v_sum.dataFormat = "percent";
            Output.Add(row_sr_v_sum);
            Output.Add(PortfolioHelper.GetRunningBalanceRow(row_sr_v_sum, labelCss: "L_right L_italic"));

            var row_sr_v_d = PortfolioHelper.DivRow(row_v_r_d, row_v_d_d);
            row_sr_v_d.year = rawDataSet.year;
            row_sr_v_d.lender = rawDataSet.lender;
            row_sr_v_d.data_name = "PORTFOLIO - CASH FLOW:Value:Payment Success Rate:Direct Debit";
            row_sr_v_d.label = row_v_r_d.label;
            row_sr_v_d.labelCss = row_v_r_d.labelCss;
            row_sr_v_d.dataFormat = "percent";
            Output.Add(row_sr_v_d);

            var row_sr_v_s = PortfolioHelper.DivRow(row_v_r_s, row_v_d_s);
            row_sr_v_s.year = rawDataSet.year;
            row_sr_v_s.lender = rawDataSet.lender;
            row_sr_v_s.data_name = "PORTFOLIO - CASH FLOW:Value:Payment Success Rate:Cash";
            row_sr_v_s.label = row_v_r_s.label;
            row_sr_v_s.labelCss = row_v_r_s.labelCss;
            row_sr_v_s.dataFormat = "percent";
            Output.Add(row_sr_v_s);

            var row_sr_v_c = PortfolioHelper.DivRow(row_v_r_c, row_v_d_c);
            row_sr_v_c.year = rawDataSet.year;
            row_sr_v_c.lender = rawDataSet.lender;
            row_sr_v_c.data_name = "PORTFOLIO - CASH FLOW:Value:Payment Success Rate:Card";
            row_sr_v_c.label = row_v_r_c.label;
            row_sr_v_c.labelCss = row_v_r_c.labelCss;
            row_sr_v_c.dataFormat = "percent";
            Output.Add(row_sr_v_c);
            Output.Add(PortfolioHelper.BlankRow(rawDataSet, "PORTFOLIO - CASH FLOW", "Split6"));

            #endregion

            #endregion

            #region Number

            Output.Add(PortfolioHelper.GroupRow(rawDataSet, "PORTFOLIO - CASH FLOW:Number", "Number", "Number", labelCss: "L_right L_bold"));

            #region Total Payment Due

            Output.Add(PortfolioHelper.BlankRow(rawDataSet, "PORTFOLIO - CASH FLOW", "Split10"));
            var row_n_d_d = PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - CASH FLOW:Number:Total Payment Due:Direct Debit", labelCss: "L_right L_italic L_gray", dataFormat: "count", label: "Direct Debit");
            var row_n_d_s = PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - CASH FLOW:Number:Total Payment Due:Cash", labelCss: "L_right L_italic L_gray", dataFormat: "count", label: "Cash");
            var row_n_d_c = PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - CASH FLOW:Number:Total Payment Due:Card", labelCss: "L_right L_italic L_gray", dataFormat: "count", label: "Card");
            PortfolioReportData row_n_d_sum = PortfolioHelper.SumRow(PortfolioHelper.SumRow(row_n_d_d, row_n_d_s), row_n_d_c);
            row_n_d_sum.year = rawDataSet.year;
            row_n_d_sum.lender = rawDataSet.lender;
            row_n_d_sum.data_name = "PORTFOLIO - CASH FLOW:Number:Total Payment Due";
            row_n_d_sum.label = "Total Payment Due";
            row_n_d_sum.labelCss = "L_right L_bold";
            row_n_d_sum.dataFormat = "count";
            Output.Add(row_n_d_sum);
            Output.Add(row_n_d_d);
            Output.Add(row_n_d_s);
            Output.Add(row_n_d_c);

            #endregion

            #region Received
            Output.Add(PortfolioHelper.BlankRow(rawDataSet, "PORTFOLIO - CASH FLOW", "Split14"));
            var row_n_r_d = PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - CASH FLOW:Number:Received:Direct Debit", labelCss: "L_right L_italic L_gray", dataFormat: "count", label: "Direct Debit");
            var row_n_r_s = PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - CASH FLOW:Number:Received:Cash", labelCss: "L_right L_italic L_gray", dataFormat: "count", label: "Cash");
            var row_n_r_c = PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - CASH FLOW:Number:Received:Card", labelCss: "L_right L_italic L_gray", dataFormat: "count", label: "Card");
            PortfolioReportData row_n_r_sum = PortfolioHelper.SumRow(PortfolioHelper.SumRow(row_n_r_d, row_n_r_s), row_n_r_c);
            row_n_r_sum.year = rawDataSet.year;
            row_n_r_sum.lender = rawDataSet.lender;
            row_n_r_sum.data_name = "PORTFOLIO - CASH FLOW:Number:Received";
            row_n_r_sum.label = "Received";
            row_n_r_sum.labelCss = "L_right L_bold";
            row_n_r_sum.dataFormat = "count";
            Output.Add(row_n_r_sum);
            Output.Add(PortfolioHelper.GetRunningBalanceRow(row_n_r_sum, labelCss: "L_right L_italic"));
            Output.Add(row_n_r_d);
            Output.Add(row_n_r_s);
            Output.Add(row_n_r_c);
            Output.Add(PortfolioHelper.BlankRow(rawDataSet, "PORTFOLIO - CASH FLOW", "Split15"));
            #endregion

            #region Payment Success Rate
            var row_sr_n_sum = PortfolioHelper.DivRow(row_n_r_sum, row_n_d_sum);
            row_sr_n_sum.year = rawDataSet.year;
            row_sr_n_sum.lender = rawDataSet.lender;
            row_sr_n_sum.data_name = "PORTFOLIO - CASH FLOW:Number:Payment Success Rate";
            row_sr_n_sum.label = "Payment Success Rate";
            row_sr_n_sum.labelCss = "L_right L_bold";
            row_sr_n_sum.dataFormat = "percent";
            Output.Add(row_sr_n_sum);
            Output.Add(PortfolioHelper.GetRunningBalanceRow(row_sr_n_sum, labelCss: "L_right L_italic"));

            var row_sr_n_d = PortfolioHelper.DivRow(row_n_r_d, row_n_d_d);
            row_sr_n_d.year = rawDataSet.year;
            row_sr_n_d.lender = rawDataSet.lender;
            row_sr_n_d.data_name = "PORTFOLIO - CASH FLOW:Value:Payment Success Rate:Direct Debit";
            row_sr_n_d.label = row_n_r_d.label;
            row_sr_n_d.labelCss = row_n_r_d.labelCss;
            row_sr_n_d.dataFormat = "percent";
            Output.Add(row_sr_v_d);

            var row_sr_n_s = PortfolioHelper.DivRow(row_n_r_s, row_n_d_s);
            row_sr_n_s.year = rawDataSet.year;
            row_sr_n_s.lender = rawDataSet.lender;
            row_sr_n_s.data_name = "PORTFOLIO - CASH FLOW:Value:Payment Success Rate:Cash";
            row_sr_n_s.label = row_n_r_s.label;
            row_sr_n_s.labelCss = row_n_r_s.labelCss;
            row_sr_n_s.dataFormat = "percent";
            Output.Add(row_sr_n_s);

            var row_sr_n_c = PortfolioHelper.DivRow(row_n_r_c, row_n_d_c);
            row_sr_n_c.year = rawDataSet.year;
            row_sr_n_c.lender = rawDataSet.lender;
            row_sr_n_c.data_name = "PORTFOLIO - CASH FLOW:Value:Payment Success Rate:Card";
            row_sr_n_c.label = row_n_r_c.label;
            row_sr_n_c.labelCss = row_n_r_c.labelCss;
            row_sr_n_c.dataFormat = "percent";
            Output.Add(row_sr_n_c);
            Output.Add(PortfolioHelper.BlankRow(rawDataSet, "PORTFOLIO - CASH FLOW", "Split16"));

            #endregion

            #endregion
        }
    }
}
