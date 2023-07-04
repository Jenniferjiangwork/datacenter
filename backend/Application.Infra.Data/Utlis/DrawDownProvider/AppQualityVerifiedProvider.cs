using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Report.Domain.Models.CRM;

namespace Report.Infra.Data.Utlis.DrawDownProvider
{
    public class AppQualityVerifiedProvider
    {
        public static void AppQualityVerified(DrawdownDBSet rawDataSet, List<DrawdownData> output)
        {
            DrawDownHelper.StartGroup(output, "App Quality - Verified", labelCss: "L_bold L_toggle");
            output.Add(DrawDownHelper.BuildRow("App Quality - Verified:Non-Financed Car", dataFormat: "percent0"));
            output.Add(DrawDownHelper.BuildRow("App Quality - Verified:Property Declared vs Adopted", dataFormat: "percent0"));
            output.Add(DrawDownHelper.BuildRow("App Quality - Verified:EquifaxScore (Avg)", dataFormat: "number2"));
            output.Add(DrawDownHelper.BlankRow("App Quality - Verified", "split2"));
            DrawDownHelper.CloseGroup();
        }
    }
}
