using Report.Domain.Models.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Infra.Data.Utlis.PortfolioProvider
{
    public class PortfolioHelper
    {
        public static PortfolioReportData GroupRow(PortfolioDBSet dataSet, string groupName, string rowName, string label, string labelCss = "L_bold L_left")
        {
            return new PortfolioReportData
            {
                lender = dataSet.lender,
                year = dataSet.year,
                data_name = $"{groupName}:{rowName}",
                label = label,
                labelCss = labelCss,
                dataCss = "",
                dataFormat = ""
            };
        }

        public static PortfolioReportData BuildRow(PortfolioDBSet dataSet,
            string dataName, string mode = "Sum", string labelCss = "", string dataCss = "", string dataFormat = "", string lender = "", string label = "")
        {
            int endIdx, beginIdx;
            var subset = dataSet.rows.Where(r => r.data_name == dataName
                && dataSet.months.IndexOf(r.data_year * 100 + r.data_month) > 0).Select(r => dataSet.months.IndexOf(r.data_year * 100 + r.data_month)).ToList();
            beginIdx = subset.Count > 0 ? subset.Min() : 1;
            endIdx = subset.Count > 0 ? subset.Max() : 1;
            var set = dataSet.rows.Where(r => r.data_name == dataName);
            if (mode == "Single" && !string.IsNullOrEmpty(lender))
            {
                set = set.Where(r => r.lender == lender);
            }
            string labelName = String.IsNullOrEmpty(label) ? dataName.Split(new char[] { ':' }).Last() : label;
            PortfolioReportData resultRow = new PortfolioReportData
            {
                data_name = dataName,
                label = labelName,
                year = dataSet.year,
                lender = (mode == "Single" && !string.IsNullOrEmpty(lender)) ? lender : dataSet.lender,
                labelCss = labelCss,
                dataCss = dataCss,
                dataFormat = dataFormat,
                endIdx = endIdx,
                beginIdx = beginIdx
            };
            foreach (var dbRow in set)
            {
                int idx = dataSet.months.IndexOf(dbRow.data_year * 100 + dbRow.data_month);
                if (mode == "Single")
                {
                    resultRow.values[idx] = dbRow.data_value;
                    continue;
                }
                else
                {
                    switch (mode)
                    {
                        case "Sum":
                            resultRow.values[idx] += dbRow.data_value;
                            break;
                        case "Max":
                            if (resultRow.values[idx] < dbRow.data_value) resultRow.values[idx] = dbRow.data_value;
                            break;
                        case "Min":
                            if (resultRow.values[idx] > dbRow.data_value) resultRow.values[idx] = dbRow.data_value;
                            break;
                    }
                }
            }
            CalsAvgColumn(resultRow);
            return resultRow;
        }

        public static PortfolioReportData GetRunningBalanceRow(PortfolioReportData srcRow, string labelCss = "")
        {
            PortfolioReportData row = new PortfolioReportData();
            for (int i = srcRow.beginIdx; i <= 12 && i <= srcRow.endIdx; i++)
            {
                row.values[i] = srcRow.values[i] - srcRow.values[i - 1];
            }
            if (srcRow.beginIdx == 1) row.values[1] = srcRow.values[1] - srcRow.values[0];
            row.avg = null;
            row.data_name = srcRow.data_name + "_RunningBalance";
            row.label = "Running Balance";
            row.year = srcRow.year;
            row.lender = srcRow.lender;
            row.dataCss = srcRow.dataCss;
            row.dataFormat = srcRow.dataFormat;
            row.beginIdx = srcRow.beginIdx;
            row.endIdx = srcRow.endIdx;
            row.labelCss = string.IsNullOrEmpty(labelCss) ? srcRow.labelCss : labelCss;
            return row;
        }

        public static PortfolioReportData BlankRow(PortfolioDBSet dataSet, string groupName, string rowName)
        {
            return new PortfolioReportData
            {
                lender = dataSet.lender,
                year = dataSet.year,
                data_name = $"{groupName}:{rowName}",
                label = "",
                labelCss = "",
                dataCss = "",
                dataFormat = ""
            };
        }

        public static PortfolioReportData DivRow(PortfolioReportData row1, PortfolioReportData row2, int bit = 4)
        {
            PortfolioReportData row = new PortfolioReportData();
            for (int i = 0; i <= 12; i++)
            {
                row.values[i] = (row2.values[i] != 0) ? Math.Round(row1.values[i] / row2.values[i], bit, 0) : 0;
            }
            row.beginIdx = Math.Min(row1.beginIdx, row2.beginIdx);
            row.endIdx = Math.Max(row1.endIdx, row2.endIdx);
            row.dataFormat = "percent";
            CalsAvgColumn(row);
            return row;
        }

        public static PortfolioReportData SumRow(PortfolioReportData row1, PortfolioReportData row2)
        {
            PortfolioReportData row = new PortfolioReportData();
            for (int i = 0; i <= 12; i++)
            {
                row.values[i] = row1.values[i] + row2.values[i];
            }
            row.beginIdx = Math.Min(row1.beginIdx, row2.beginIdx);
            row.endIdx = Math.Max(row1.endIdx, row2.endIdx);
            row.dataFormat = row1.dataFormat;
            CalsAvgColumn(row);
            return row;
        }

        private static void CalsAvgColumn(PortfolioReportData row)
        {
            decimal sum = 0;
            for (int i = 1; i <= 12; i++)
            {
                sum += row.values[i];
            }
            int decimals = 0;
            switch (row.dataFormat)
            {
                case "percent":
                    decimals = 4; break;
                case "currency":
                    decimals = 2; break;
            }
            row.avg = Math.Round(sum / (row.endIdx - row.beginIdx + 1), decimals);
        }
    }
}
