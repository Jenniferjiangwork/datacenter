using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Report.Domain.Models.CRM;

namespace Report.Infra.Data.Utlis.DrawDownProvider
{
    public class AppQualityDeclaredProvider
    {       
        public static void AppQualityDeclared(DrawdownDBSet rawDataSet, List<DrawdownData> output)
        {
            DrawDownHelper.StartGroup(output, "App Quality - Declared", labelCss: "L_bold L_toggle");
            var r_total = output.Find(r => r.data_name == "Business Form:Total");
            var r_total_approve = output.Find(r => r.data_name == "Business Form:Total Pre-Approved");
            var r_d = DrawDownHelper.BuildRow("App Quality - Declared:ABN", dataFormat: "number0");
            var r_rate = DrawDownHelper.DivRow(r_d, r_total, 2,
               dataName: "App Quality - Declared:ABN %", dataFormat: "percent0");
            output.Add(r_rate);

            r_d = DrawDownHelper.BuildRow("App Quality - Declared:Planning", dataFormat: "number0");
            r_rate = DrawDownHelper.DivRow(r_d, r_total, 2,
               dataName: "App Quality - Declared:Planning %", dataFormat: "percent0");
            output.Add(r_rate);

            r_d = DrawDownHelper.BuildRow("App Quality - Declared:Verification Decline", dataFormat: "number0");
            r_rate = DrawDownHelper.DivRow(r_d, r_total_approve, 2,
               dataName: "App Quality - Declared:Verification Decline %", dataFormat: "percent0");
            output.Add(r_rate);

            r_d = DrawDownHelper.BuildRow("App Quality - Declared:Business over 6 mths", dataFormat: "number0");
            r_rate = DrawDownHelper.DivRow(r_d, r_total, 2,
               dataName: "App Quality - Declared:Business over 6 mths %", dataFormat: "percent0");
            output.Add(r_rate);

            r_d = DrawDownHelper.BuildRow("App Quality - Declared:Joint", dataFormat: "number0");
            r_rate = DrawDownHelper.DivRow(r_d, r_total, 2,
               dataName: "App Quality - Declared:Joint / Add - On Application %", dataFormat: "percent0");
            output.Add(r_rate);

            output.Add(DrawDownHelper.BlankRow("App Quality - Declared", "split1"));

            r_d = DrawDownHelper.BuildRow("App Quality - Declared:Real estate owner", dataFormat: "number0");
            r_rate = DrawDownHelper.DivRow(r_d, r_total, 2,
               dataName: "App Quality - Declared:Real estate owner %", dataFormat: "percent0");
            output.Add(r_rate);

            r_d = DrawDownHelper.BuildRow("App Quality - Declared:No asset", dataFormat: "number0");
            r_rate = DrawDownHelper.DivRow(r_d, r_total, 2,
               dataName: "App Quality - Declared:No asset %", dataFormat: "percent0");
            output.Add(r_rate);

            r_d = DrawDownHelper.BuildRow("App Quality - Declared:Credit History Clean", dataFormat: "number0");
            r_rate = DrawDownHelper.DivRow(r_d, r_total, 2,
               dataName: "App Quality - Declared:Credit History Clean % (Claim)", dataFormat: "percent0");
            output.Add(r_rate);

            output.Add(DrawDownHelper.BuildRow("App Quality - Declared:Age of Applicant (Avg)", dataFormat: "number2"));

            output.Add(DrawDownHelper.BlankRow("App Quality - Declared", "split2"));
            DrawDownHelper.CloseGroup();
        }       
    }
}
