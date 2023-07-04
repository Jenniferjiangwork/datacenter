using Report.Domain.Models.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Infra.Data.Utlis.PortfolioProvider
{
    public class PortfolioSizeProvider
    {       
        public static void PortfolioSize(PortfolioDBSet rawDataSet, List<PortfolioReportData> Output)
        {
            Output.Add(PortfolioHelper.GroupRow(rawDataSet, "PORTFOLIO - SIZE", "TopRow", "1 PORTFOLIO - SIZE"));
            var toatalActiveRow = PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - SIZE:Total Active Loans", labelCss: "L_right", dataFormat: "count");

            Output.Add(toatalActiveRow);
            Output.Add(PortfolioHelper.GetRunningBalanceRow(toatalActiveRow, labelCss: "L_italic L_right"));
            Output.Add(PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - SIZE:Repaying", labelCss: "L_right L_gray", dataFormat: "count"));
            var defaultRow = PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - SIZE:Default", labelCss: "L_right L_gray", dataFormat: "count");
            Output.Add(defaultRow);
            var recoveryRow = PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - SIZE:Recovery", labelCss: "L_right L_gray", dataFormat: "count");
            Output.Add(recoveryRow);
            Output.Add(PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - SIZE:Close", labelCss: "L_right L_gray", dataFormat: "count"));
            Output.Add(PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - SIZE:Settled", labelCss: "L_right L_gray", dataFormat: "count"));
            var writtenRow = PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - SIZE:Written Off", label: "Written", labelCss: "L_right L_gray", dataFormat: "count");
            Output.Add(writtenRow);
            Output.Add(PortfolioHelper.BlankRow(rawDataSet, "PORTFOLIO - SIZE", "Split1"));
            var totalRow = PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - SIZE:Total Loans to Date", labelCss: "L_right L_gray", dataFormat: "count");
            Output.Add(totalRow);

            var underDefaultRow = PortfolioHelper.DivRow(PortfolioHelper.SumRow(defaultRow, recoveryRow), toatalActiveRow);
            underDefaultRow.dataFormat = "percent";
            underDefaultRow.labelCss = "L_right L_gray";
            underDefaultRow.label = "% Under Default";
            underDefaultRow.data_name = "PORTFOLIO - SIZE:% Under Default";
            underDefaultRow.year = rawDataSet.year;
            underDefaultRow.lender = rawDataSet.lender;
            Output.Add(underDefaultRow);

            var underWrittenRow = PortfolioHelper.DivRow(writtenRow, totalRow);
            underWrittenRow.dataFormat = "percent";
            underWrittenRow.labelCss = "L_right L_gray";
            underWrittenRow.label = "% Under Written Off";
            underWrittenRow.data_name = "PORTFOLIO - SIZE:% Under Written Off";
            underWrittenRow.year = rawDataSet.year;
            underWrittenRow.lender = rawDataSet.lender;
            Output.Add(underWrittenRow);

            if (rawDataSet.lendersInDB.Count > 1)
            {
                Output.Add(PortfolioHelper.BlankRow(rawDataSet, "PORTFOLIO - SIZE", "Split2"));
                Output.Add(PortfolioHelper.GroupRow(rawDataSet, "PORTFOLIO - SIZE:ACTIVE LOANS", "TopRow", "ACTIVE LOANS", "L_right L_gray L_bold"));
                foreach (var lender in rawDataSet.lendersInDB)
                {
                    var activeLoanRow = PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - SIZE:Total Active Loans", mode: "Single", labelCss: "L_right L_gray L_bold L_italic", dataFormat: "count", lender: lender);
                    activeLoanRow.label = lender;
                    Output.Add(activeLoanRow);
                    var runningBalanceRow = PortfolioHelper.BuildRow(rawDataSet, "PORTFOLIO - SIZE:Running Balance Less Arrears", mode: "Single", labelCss: "L_right L_gray L_italic", dataFormat: "currency", lender: lender);
                    runningBalanceRow.label = "Balance (Less Arrears)";
                    Output.Add(runningBalanceRow);
                }
            }
            Output.Add(PortfolioHelper.BlankRow(rawDataSet, "PORTFOLIO - SIZE", "Split3"));
        }

    }
}
