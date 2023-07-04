using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Report.Domain.Models.CRM;

namespace Report.Infra.Data.Utlis.DrawDownProvider
{
    public class RetentionProvider
    {
        public static void Retention(DrawdownDBSet rawDataSet, List<DrawdownData> output)
        {
            DrawDownHelper.StartGroup(output, "Retention", labelCss: "L_bold L_toggle");

            var r_exist_client_no = DrawDownHelper.BuildRow("Retention:Existing Client No.", labelCss: "", dataFormat: "number0", enableDetail: true);
            output.Add(r_exist_client_no);
            output.Add(DrawDownHelper.BuildRow("Retention:Existing Client Loan $", labelCss: "", dataFormat: "currency0"));

            var r_drawdown = DrawDownHelper.SumRow(
                    DrawDownHelper.BuildRow("Total Draw-Down:1st Draw-Down no"),
                    DrawDownHelper.BuildRow("Total Draw-Down:Sub Draw-down no"));
            var r_exist_client_drawdown = DrawDownHelper.DivRow(r_exist_client_no, r_drawdown, 2,
                dataName: "Retention:Existing Client / Total DrawDown", dataFormat: "percent0");
            output.Add(r_exist_client_drawdown);

            var r_exist_contact_no = DrawDownHelper.BuildRow("Retention:Existing Contacts (No Loans) No.", labelCss: "", dataFormat: "number0", enableDetail: true);
            output.Add(r_exist_contact_no);
            output.Add(DrawDownHelper.BuildRow("Retention:Existing Contacts Loan$", dataFormat: "currency0"));

            var r_exist_contact_drawdown = DrawDownHelper.DivRow(r_exist_contact_no, r_drawdown, 2,
                dataName: "Retention:Existing Contacts / Total DrawDown", dataFormat: "percent0");
            output.Add(r_exist_contact_drawdown);

            output.Add(DrawDownHelper.SumRow(r_exist_contact_drawdown, r_exist_client_drawdown, dataName: "Retention:Existing / Total DrawDown", dataFormat: "percent0", comment: "Retention-ExistingTotalDrawDown"));

            output.Add(DrawDownHelper.BlankRow("Retention", "split1"));
            DrawDownHelper.CloseGroup();
        }
    }
}
