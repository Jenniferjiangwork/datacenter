using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Report.Domain.Models.CRM;

namespace Report.Infra.Data.Utlis.DrawDownProvider
{
    public class TotalDrawDownProvider
    {
        public static void TotalDrawDown(DrawdownDBSet rawDataSet, List<DrawdownData> output)
        {
            var r_1_c = DrawDownHelper.BuildRow("Total Draw-Down:1st Draw-down $", dataFormat: "currency0");
            var r_s_c = DrawDownHelper.BuildRow("Total Draw-Down:Sub Draw-down $", dataFormat: "currency0");
            var r_1_n = DrawDownHelper.BuildRow("Total Draw-Down:1st Draw-Down no", label: "1st Draw-down no", labelCss: "", dataFormat: "number0", enableDetail: true);
            var r_s_n = DrawDownHelper.BuildRow("Total Draw-Down:Sub Draw-down no", dataFormat: "number0", enableDetail: true);
            DrawDownHelper.StartGroup(output, "Total Draw-Down",
                DrawDownHelper.SumRow(r_1_c, r_s_c, dataName: "Total Draw-Down:Total Draw-Down", labelCss: "L_bold L_toggle", dataFormat: "currency0", dataCss: "L_bold "));
            r_1_c = DrawDownHelper.BuildRow("Total Draw-Down:1st Draw-down $", dataFormat: "currency0");
            r_s_c = DrawDownHelper.BuildRow("Total Draw-Down:Sub Draw-down $", dataFormat: "currency0");
            r_1_n = DrawDownHelper.BuildRow("Total Draw-Down:1st Draw-Down no", label: "1st Draw-down no", labelCss: "", dataFormat: "number0", enableDetail: true);
            r_s_n = DrawDownHelper.BuildRow("Total Draw-Down:Sub Draw-down no", dataFormat: "number0", enableDetail: true);
            output.Add(DrawDownHelper.SumRow(r_1_n, r_s_n, dataName: "Total Draw-Down:Total Draw-Down no", dataFormat: "number0"));
            output.Add(r_1_c);
            output.Add(r_1_n);
            output.Add(r_s_c);
            output.Add(r_s_n);
            output.Add(DrawDownHelper.BlankRow("Total Draw-Down", "split1"));
            DrawDownHelper.CloseGroup();
        }
    }
}
