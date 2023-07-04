using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Report.Domain.Models.CRM;

namespace Report.Infra.Data.Utlis.DrawDownProvider
{
    public class ChannelProvider
    {
        public static void Channel(DrawdownDBSet rawDataSet, List<DrawdownData> output)
        {
            DrawDownHelper.StartGroup(output, "Channel", label: "Marketing Channels", labelCss: "L_bold L_toggle"); 
            output.Add(DrawDownHelper.BuildRow("Channel: Adwords", label: "Adwords", dataFormat: "currency0", comment: "Channel-Adwords", enableDetail: true));
            output.Add(DrawDownHelper.BuildRow("Channel: Email Outreach", label: "Email Outreach", dataFormat: "currency0", comment: "Channel Email-Outreach", enableDetail: true));
            output.Add(DrawDownHelper.BuildRow("Channel: Referrer", label: "Referrer", dataFormat: "currency0", comment: "Channel-Referrer", enableDetail: true));
            output.Add(DrawDownHelper.BuildRow("Channel: Other", label: "Others", dataFormat: "currency0", comment: "Channel-Other", enableDetail: true));
            output.Add(DrawDownHelper.BlankRow("Channel", "split1"));
            DrawDownHelper.CloseGroup();
        }
    }
}
