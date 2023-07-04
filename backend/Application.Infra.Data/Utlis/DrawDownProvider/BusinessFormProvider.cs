using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Report.Domain.Models.CRM;

namespace Report.Infra.Data.Utlis.DrawDownProvider
{
    public class BusinessFormProvider
    {
        public static void BusinessForm(DrawdownDBSet rawDataSet, List<DrawdownData> output)
        {
            DrawDownHelper.StartGroup(output, "Business Form", labelCss: "L_bold L_toggle");
            var r_total = DrawDownHelper.BuildRow("Business Form:Total", dataFormat: "number0");
            output.Add(r_total);
            var r_totle_pre_approved = DrawDownHelper.BuildRow("Business Form:Total Pre-Approved",
                dataFormat: "number0");
            output.Add(r_totle_pre_approved);
            var r_pre_approval_rate = DrawDownHelper.DivRow(r_totle_pre_approved, r_total, 2,
                dataName: "Business Form:Pre - Approval Rate",
                dataFormat: "percent0");
            output.Add(r_pre_approval_rate);
            output.Add(DrawDownHelper.BlankRow("Business Form", "split1"));
            DrawDownHelper.CloseGroup();
        }
    }
}
