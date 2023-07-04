using Report.Domain.Models.RMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Infra.Data.Utlis.GeneralPerformanceProvider
{
    public class ApplicationProvider
    {
        public static void Application(GeneralPerformanceDBSet rawDataSet, List<GeneralPerformanceData> output)
        {
            GeneralPerformanceHelper.StartGroup(output, "Application", labelCss: "L_bold L_toggle");

            var r_total = GeneralPerformanceHelper.BuildRow("Application:Total Apply (#)", dataFormat: "number0");
            var r_totle_pre_approved = GeneralPerformanceHelper.BuildRow("Application:Pre-Approval (#)", dataFormat: "number0");
            var r_pre_approval_rate = GeneralPerformanceHelper.DivRow(r_totle_pre_approved, r_total, 2,
                dataName: "Application:Pre-Approval Rate (%)",
                dataFormat: "percent0");


            output.Add(r_total);
            output.Add(r_totle_pre_approved);
            output.Add(r_pre_approval_rate);
            output.Add(GeneralPerformanceHelper.BlankRow("Application", "split1"));
            GeneralPerformanceHelper.CloseGroup();
        }
    }
}
