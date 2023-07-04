using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Report.Domain.Models.CRM;

namespace Report.Infra.Data.Utlis.DrawDownProvider
{
    public class FirstDrawProfileProvider
    {
        public static void FirstDrawProfile(DrawdownDBSet rawDataSet, List<DrawdownData> output)
        {
            var total = DrawDownHelper.BuildRow("Total Draw-Down:1st Draw-Down no");
            var p = DrawDownHelper.BuildRow("1st Draw Profile:Property");
            var c = DrawDownHelper.BuildRow("1st Draw Profile:Company");

            DrawDownHelper.StartGroup(output, "1st Draw Profile", label: "1st Draw Profile", labelCss: "L_bold L_toggle");
            output.Add(DrawDownHelper.DivRow(p, total, 2, dataName: "1st Draw Profile:Property", comment: "1st Draw Profile-Property", label: "Property %", dataFormat: "percent1"));
            output.Add(DrawDownHelper.BuildRow("1st Draw Profile:Average $", label: "Average $", dataFormat: "currency0", comment: "1st Draw Profile-Average"));
            output.Add(DrawDownHelper.BuildRow("1st Draw Profile:Medium $", label: "Medium $", dataFormat: "currency0", comment: "1st Draw Profile-Medium"));
            output.Add(DrawDownHelper.BuildRow("1st Draw Profile:Veda Score", label: "Veda Score", dataFormat: "number1", comment: "1st Draw Profile-Veda-Score"));
            output.Add(DrawDownHelper.BuildRow("1st Draw Profile:Borrower Age", label: "Borrower Age", dataFormat: "number1", comment: "1st Draw Profile-Borrower-Age"));
            output.Add(DrawDownHelper.DivRow(c, total, 2, dataName: "1st Draw Profile:Company", comment: "1st Draw Profile-Company", label: "Company %", dataFormat: "percent1"));
            output.Add(DrawDownHelper.BlankRow("1st Draw Profile", "split1"));
            DrawDownHelper.CloseGroup();
        }
    }
}
