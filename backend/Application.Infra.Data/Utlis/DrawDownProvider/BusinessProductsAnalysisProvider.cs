using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Report.Domain.Models.CRM;

namespace Report.Infra.Data.Utlis.DrawDownProvider
{
    public class BusinessProductsAnalysisProvider
    {
        public static void Build(DrawdownDBSet rawDataSet, List<DrawdownData> output)
        {
            DrawDownHelper.StartGroup(output, "Business Products Analysis", labelCss: "L_bold L_toggle");
            output.Add(DrawDownHelper.BlankRow("Business Products Analysis", "split1"));
            output.Add(DrawDownHelper.BuildRow("Business Products Analysis:Premium $15k+:Draw-down $", label: "Premium $15k+", dataFormat: "currency0", comment: "BusinessProductsAnalysis-Premium15k"));
            output.Add(DrawDownHelper.BuildRow("Business Products Analysis:Fast Bus $10k - $15k:Draw-down $", label: "Fast Bus $10k - $15k", dataFormat: "currency0", comment: "BusinessProductsAnalysis-FastBus10k-15k"));
            output.Add(DrawDownHelper.BuildRow("Business Products Analysis:Fast Loan Under $10k:Draw-down $", label: "Fast Loan under $10k", dataFormat: "currency0", comment: "BusinessProductsAnalysis-FastLoanUnder10k"));
            output.Add(DrawDownHelper.BuildRow("Business Products Analysis:USB:Draw-down $", label: "USB", dataFormat: "currency0", comment: "BusinessProductsAnalysis-USB"));
            output.Add(DrawDownHelper.BlankRow("Business Products Analysis", "split2"));
            ProductDetailProvider(rawDataSet, output);
            DrawDownHelper.CloseGroup();
        }

        private static void ProductDetailProvider(DrawdownDBSet rawDataSet, List<DrawdownData> output)
        {
            Build("Premium $15k+", "Premium15k", rawDataSet, output);
            Build("Fast Bus $10k - $15k", "FastBus", rawDataSet, output);
            Build("Fast Loan Under $10k", "FastLoan", rawDataSet, output);
            Build("USB", "USB", rawDataSet, output);
        }

        private static void Build(string groupName1, string groupNam2, DrawdownDBSet rawDataSet, List<DrawdownData> output)
        {
            DrawDownHelper.StartGroup(output, groupName1, labelCss: "L_bold L_toggle", comment: groupName1 + "-sublabel");
            output.Add(DrawDownHelper.BuildRow($"Business Products Analysis:{groupName1}:Draw-down $", label: "Draw-down $", dataFormat: "currency0", comment: $"Business Products Analysis-{groupNam2}-Draw-down"));
            var r_no = DrawDownHelper.BuildRow($"Business Products Analysis:{groupName1}:Draw-down No", label: "Draw-down No", dataFormat: "number0", comment: $"Business Products Analysis{groupNam2}-Draw-downNo", enableDetail: true);
            output.Add(r_no);
            output.Add(DrawDownHelper.BuildRow($"Business Products Analysis:{groupName1}:Medium cash Amount", label: "Medium cash Amount", dataFormat: "currency0", comment: $"Business Products Analysis-{groupNam2}-Medium-cash-Amount"));
            output.Add(DrawDownHelper.BuildRow($"Business Products Analysis:{groupName1}:Highest cash Amount", label: "Highest cash Amount", dataFormat: "currency0", comment: $"Business Products Analysis-{groupNam2}-Highest-cash-Amount"));
            output.Add(DrawDownHelper.BuildRow($"Business Products Analysis:{groupName1}:Net Inflow - Avg per loan", label: "Net Inflow - Avg per loan", dataFormat: "currency0", comment: $"Business Products Analysis-{groupNam2}-Net-Inflow-Avg-per-loan"));
            output.Add(DrawDownHelper.BuildRow($"Business Products Analysis:{groupName1}:Total Group Inflow", label: "Total Group Inflow", dataFormat: "currency0", comment: $"Business Products Analysis-{groupNam2}-Total-Group-Inflow"));
            output.Add(DrawDownHelper.BuildRow($"Business Products Analysis:{groupName1}:Mark-up", label: "Mark-up", dataFormat: "percent0", comment: $"Business Products Analysis-{groupNam2}-Mark-up"));
            output.Add(DrawDownHelper.BlankRow(groupName1, "split1"));
            DrawDownHelper.StartGroup(output, "Loan Type", labelCss: "L_underline");
            output.Add(DrawDownHelper.DivRow(DrawDownHelper.BuildRow($"Business Products Analysis:{groupName1}:Loan Type/P & I"), r_no,
                    dataName: $"Business Products Analysis:{groupName1}:Loan Type/P & I",
                    label: "P & I %", dataFormat: "percent0",
                    comment: $"Business Products Analysis-{groupNam2}-Loan-Type-PandI"));
            output.Add(DrawDownHelper.DivRow(DrawDownHelper.BuildRow($"Business Products Analysis:{groupName1}:Loan Type/IO"), r_no,
                    dataName: $"Business Products Analysis:{groupName1}:Loan Type/IO",
                    label: "IO %", dataFormat: "percent0",
                    comment: $"Business Products Analysis-{groupNam2}-Loan-Type-IO"));
            output.Add(DrawDownHelper.DivRow(DrawDownHelper.BuildRow($"Business Products Analysis:{groupName1}:Loan Type/Flex"), r_no,
                dataName: $"Business Products Analysis:{groupName1}:Loan Type/Flex",
                    label: "Flex %", dataFormat: "percent0",
                    comment: $"Business Products Analysis-{groupNam2}-Loan-Type-Flex"));
            DrawDownHelper.CloseGroup();
            output.Add(DrawDownHelper.BlankRow(groupName1, "split2"));
            DrawDownHelper.StartGroup(output, "Loan Features", labelCss: "L_underline");
            output.Add(DrawDownHelper.BuildRow($"Business Products Analysis:{groupName1}:Loan Features/Loan Term Average (Mth)", label: "Loan Term Average (Mth)", dataFormat: "number2", comment: $"Business Products Analysis-{groupNam2}-Loan-Features-Loan-Term-Average-Mth"));
            DrawDownHelper.CloseGroup();
            output.Add(DrawDownHelper.BlankRow(groupName1, "split3"));
            DrawDownHelper.CloseGroup();
        }
    }
}
