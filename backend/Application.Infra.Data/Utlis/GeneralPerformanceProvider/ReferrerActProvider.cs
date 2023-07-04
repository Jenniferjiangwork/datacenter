using Report.Domain.Models.RMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Infra.Data.Utlis.GeneralPerformanceProvider
{
    public class ReferrerActProvider
    {
        public static void ReferrerAct(GeneralPerformanceDBSet rawDataSet, List<GeneralPerformanceData> output)
        {
            /////////////////////////////// Existing Referrer///////////////////////////////////////////////////////
            GeneralPerformanceHelper.StartGroup(output, "Existing Referrer", labelCss: "L_bold L_toggle");

            var r_total = GeneralPerformanceHelper.BuildRow("Existing Referrer:Total (#)", dataFormat: "number0");
            var r_totle_subscribed = GeneralPerformanceHelper.BuildRow("Existing Referrer:Subscribed (#)", dataFormat: "number0");
            var r_totle_unsubscribed = GeneralPerformanceHelper.BuildRow("Existing Referrer:Unsubscribed (#)", dataFormat: "number0");
            var r_totle_pu = GeneralPerformanceHelper.BuildRow("PU Existing Referrer:Portal Usage (#)", label: "Portal Usage (#)", labelCss: "", dataFormat: "number0", enableDetail: true);

            output.Add(r_total);
            output.Add(r_totle_subscribed);
            output.Add(r_totle_unsubscribed);
            output.Add(r_totle_pu);
            output.Add(GeneralPerformanceHelper.BlankRow("Existing Referrer", "split1"));
            GeneralPerformanceHelper.CloseGroup();


            /////////////////////////////// Expansion ///////////////////////////////////////////////////////
            GeneralPerformanceHelper.StartGroup(output, "Expansion", labelCss: "L_bold L_toggle");

            var r_e_r = GeneralPerformanceHelper.BuildRow("Expansion Referrer:New Referrer (#)", label: "New Referrer (#)", labelCss: "", dataFormat: "number0", enableDetail: true);
            var r_e_a = GeneralPerformanceHelper.BuildRow("Expansion Active:New Active (#)", label: "New Active (#)", labelCss: "", dataFormat: "number0", enableDetail: true);

            output.Add(r_e_r);
            output.Add(r_e_a);
            output.Add(GeneralPerformanceHelper.BlankRow("Expansion", "split1"));
            GeneralPerformanceHelper.CloseGroup();

        }
    }
}
