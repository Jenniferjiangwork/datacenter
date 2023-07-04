using Report.Domain.Models.RMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Infra.Data.Utlis.GeneralPerformanceProvider
{
    public class DrawdownProvider
    {
        public static void Drawdown(GeneralPerformanceDBSet rawDataSet, List<GeneralPerformanceData> output)
        {
            GeneralPerformanceHelper.StartGroup(output, "DrawDown", labelCss: "L_bold L_toggle");

            var r_1_c = GeneralPerformanceHelper.BuildRow("Draw-Down:1st Drawdown ($)", dataFormat: "currency0");
            var r_s_c = GeneralPerformanceHelper.BuildRow("Draw-Down:LOC ($)", dataFormat: "currency0");
            var r_1_n = GeneralPerformanceHelper.BuildRow("Draw-Down:1st Drawdown (#)", label: "1st Drawdown (#)", labelCss: "", dataFormat: "number0", enableDetail: true);  // enableDetail
            var r_s_n = GeneralPerformanceHelper.BuildRow("Draw-Down:LOC (#)", dataFormat: "number0", enableDetail: true);
            var r_totle_pre_approved = GeneralPerformanceHelper.BuildRow("Application:Pre-Approval (#)", dataFormat: "number0");


            output.Add(GeneralPerformanceHelper.SumRow(r_1_n, r_s_n, dataName: "Draw-Down:Total Settlement (#)", dataFormat: "number0"));
            output.Add(GeneralPerformanceHelper.DivRow(r_totle_pre_approved, r_1_n, 2, dataName: "Draw-Down:Conversion Rate", dataFormat: "number2"));
            output.Add(r_1_c);
            output.Add(r_1_n);
            output.Add(r_s_c);
            output.Add(r_s_n);

            output.Add(GeneralPerformanceHelper.BlankRow("DrawDown", "split1"));
            GeneralPerformanceHelper.CloseGroup();
        }
    }
}
