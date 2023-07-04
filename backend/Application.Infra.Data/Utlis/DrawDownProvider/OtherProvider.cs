using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Report.Domain.Models.CRM;

namespace Report.Infra.Data.Utlis.DrawDownProvider
{
    public class OtherProvider
    {
        public static void Other(DrawdownDBSet rawDataSet, List<DrawdownData> output)
        {
            DrawDownHelper.StartGroup(output, "Other", label: "Other", labelCss: "L_bold L_toggle");
            output.Add(DrawDownHelper.BuildRow("Other:Paterner Login No.", label: "Partner Login No.", dataFormat: "number0", comment: "Other-Partner-Login-No"));
            output.Add(DrawDownHelper.BuildRow("Other:Client Portal Login No.", label: "Client Portal Login No.", dataFormat: "number0", comment: "Other-Client-Portal-Login-No"));
            output.Add(DrawDownHelper.BuildRow("Other:LOC Draw Request No.", label: "LOC Draw Request No.", dataFormat: "number0", comment: "Other-LOC-Draw-Request-No"));
            output.Add(DrawDownHelper.BuildRow("Other:LOC Review Request No.", label: "LOC Review Request No.", dataFormat: "number0", comment: "Other-LOC-Review-Request-No"));
            DrawDownHelper.CloseGroup();
        }
    }
}
