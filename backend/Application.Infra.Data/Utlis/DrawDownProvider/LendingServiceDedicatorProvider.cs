using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Report.Domain.Models.CRM;

namespace Report.Infra.Data.Utlis.DrawDownProvider
{
    public class LendingServiceDedicatorProvider
    {
        public static void LendingServiceDedicator(DrawdownDBSet rawDataSet, List<DrawdownData> output)
        {
            DrawDownHelper.StartGroup(output, "Lending Service Dedicator", labelCss: "L_bold L_toggle");
            output.Add(DrawDownHelper.BlankRow("Lending Service Dedicator", "split1"));
            AddSection(output, "All Clients");
            AddSection(output, "A Clients - Property");
            AddSection(output, "B Clients - No Property");
            AddWorkload(output);
            DrawDownHelper.CloseGroup();
        }
        private static void AddSection(List<DrawdownData> output, string subGroup)
        {
            DrawDownHelper.StartGroup(output, subGroup, labelCss: "L_bold");
            var r_apporve = DrawDownHelper.BuildRow($"Lending Service Dedicator:{subGroup}:Total Pre-Approved", labelCss: "", dataFormat: "number0");
            var r_cash = DrawDownHelper.BuildRow($"Lending Service Dedicator:{subGroup}:1st Draw-down $", labelCss: "", dataFormat: "number2");
            var r_no = DrawDownHelper.BuildRow($"Lending Service Dedicator:{subGroup}:1st Draw-down No", labelCss: "", dataFormat: "number0");
            var r_engagement = DrawDownHelper.BuildRow($"Lending Service Dedicator:{subGroup}:Engagement Indicator", labelCss: "", dataFormat: "number0");
            var r_cash_conversion = DrawDownHelper.DivRow(r_cash, r_apporve, bit: 0, dataName: $"Lending Service Dedicator:{subGroup}:Conversion Value (CV)", dataFormat: "currency0", comment: $"Lending Service Dedicator{subGroup}-ConversionValue(CV)");
            var r_no_conversion = DrawDownHelper.DivRow(r_apporve, r_no, bit: 1, dataName: $"Lending Service Dedicator:{subGroup}:Conversion No. (CN)", dataFormat: "number1", comment: $"Lending Service Dedicator{subGroup}-ConversionNo.(CN)");
            var r_engagement_conversion = DrawDownHelper.DivRow(r_engagement, r_apporve, bit: 2, dataName: $"Lending Service Dedicator:{subGroup}:Engagement Indicator", dataFormat: "number2", comment: $"Lending Service Dedicator{subGroup}-EngagementIndicator");

            output.Add(r_cash_conversion);
            output.Add(r_no_conversion);
            output.Add(r_engagement_conversion);

            output.Add(DrawDownHelper.BuildRow($"Lending Service Dedicator:{subGroup}:Settled App duration (days) Avg", labelCss: "", dataFormat: "number1"));
            output.Add(DrawDownHelper.BuildRow($"Lending Service Dedicator:{subGroup}:Settled App duration (days) Medium", labelCss: "", dataFormat: "number1"));

            output.Add(DrawDownHelper.BlankRow($"Lending Service Dedicator:{subGroup}", "split1"));

            DrawDownHelper.CloseGroup();
        }
        private static void AddWorkload(List<DrawdownData> output)
        {
            DrawDownHelper.StartGroup(output, "Workload", labelCss: "L_bold");
            output.Add(DrawDownHelper.BuildRow("Lending Service Dedicator:Workload:Email Received", labelCss: "", dataFormat: "number0"));
            output.Add(DrawDownHelper.BlankRow("Lending Service Dedicator:Workload", "split1"));
            DrawDownHelper.CloseGroup();
        }
    }
}
