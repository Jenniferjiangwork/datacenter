using Report.Domain.Models.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Infra.Data.Utlis.PortfolioProvider
{
    public class PortfolioInOutProvider
    {
        public static void PortfolioInOut(PortfolioDBSet rawDataSet, List<PortfolioReportData> Output)
        {
            Output.Add(PortfolioHelper.GroupRow(rawDataSet, "PORTFOLIO - IN & OUT", "TopRow", "2 PORTFOLIO - IN & OUT"));
            Output.Add(PortfolioHelper.GroupRow(rawDataSet, "PORTFOLIO - IN & OUT", "Cash Amount", "Cash Amount", labelCss: "L_right L_bold"));
            Output.Add(PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - IN - OUT:Cash Amount:In - 1st Drawdown", labelCss: "L_right", dataFormat: "currency"));
            Output.Add(PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - IN - OUT:Cash Amount:In - Sub Drawdown (LOC)", labelCss: "L_right", dataFormat: "currency"));
            Output.Add(PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - IN - OUT:Cash Amount:In - Written off to Repay", labelCss: "L_right", dataFormat: "currency"));
            Output.Add(PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - IN - OUT:Cash Amount:Out - Discharge", labelCss: "L_right", dataFormat: "currency"));
            Output.Add(PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - IN - OUT:Cash Amount:Out - Written off", labelCss: "L_right", dataFormat: "currency"));
            Output.Add(PortfolioHelper.BlankRow(rawDataSet, "PORTFOLIO - IN - OUT", "Split1"));
            Output.Add(PortfolioHelper.GroupRow(rawDataSet, "PORTFOLIO - IN & OUT", "Number", "Number", labelCss: "L_right L_bold"));
            Output.Add(PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - IN - OUT:Number:In - New Loans", labelCss: "L_right", dataFormat: "count"));
            Output.Add(PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - IN - OUT:Number:In - New LOC", labelCss: "L_right", dataFormat: "count"));
            Output.Add(PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - IN - OUT:Number:In - Written off to Repay", labelCss: "L_right", dataFormat: "count"));
            Output.Add(PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - IN - OUT:Number:Out - Discharge", labelCss: "L_right", dataFormat: "count"));
            Output.Add(PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - IN - OUT:Number:Out - Written off", labelCss: "L_right", dataFormat: "count"));
            Output.Add(PortfolioHelper.BlankRow(rawDataSet, "PORTFOLIO - IN - OUT", "Split2"));
        }
    }
}
