using Report.Domain.Models.RMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Infra.Data.Utlis.GeneralPerformanceProvider
{
    public class SettlementBreakProvider
    {
        public static void SettlementBreak(GeneralPerformanceDBSet rawData, List<GeneralPerformanceData> output)
        {
            ///////////General Quality///////////////////////////////////////////////////////////////////
            GeneralPerformanceHelper.StartGroup(output, "General Quality", labelCss: "L_bold L_toggle");

            var r_gq_h = GeneralPerformanceHelper.BuildRow("Settlement-Break:Highest Settlement ($)", dataFormat: "currency0");
            var r_gq_l = GeneralPerformanceHelper.BuildRow("Settlement-Break:Lowest Settlement ($)", dataFormat: "currency0");
            var r_gq_a = GeneralPerformanceHelper.BuildRow("Settlement-Break:Average ($)", dataFormat: "currency0");
            var r_gq_m = GeneralPerformanceHelper.BuildRow("Settlement-Break Median:Median ($)", dataFormat: "currency0");


            output.Add(r_gq_h);
            output.Add(r_gq_l);
            output.Add(r_gq_a);
            output.Add(r_gq_m);

            output.Add(GeneralPerformanceHelper.BlankRow("General Quality", "split1"));
            GeneralPerformanceHelper.CloseGroup();



            /////////////Finance Broker/////////////////////////////////////////////////////////////////////
            GeneralPerformanceHelper.StartGroup(output, "Finance Broker", labelCss: "L_bold L_toggle");

            var r_no_fb = GeneralPerformanceHelper.BuildRow("Settlement-Break:Finance Broker:Settlement (#)", dataFormat: "number0");
            var r_amt_fb = GeneralPerformanceHelper.BuildRow("Settlement-Break:Finance Broker:Settlement ($)", dataFormat: "currency0");
            var r_qua_fb = GeneralPerformanceHelper.DivRow(r_amt_fb, r_no_fb, 2, dataName: "Settlement-Break:Finance Broker:Quality ($)", dataFormat: "currency0");


            output.Add(r_no_fb);
            output.Add(r_amt_fb);
            output.Add(r_qua_fb);
            output.Add(GeneralPerformanceHelper.BlankRow("Finance Broker", "split1"));
            GeneralPerformanceHelper.CloseGroup();



            /////////////Lead Gen/////////////////////////////////////////////////////////////////////
            GeneralPerformanceHelper.StartGroup(output, "Lead Gen", labelCss: "L_bold L_toggle");

            var r_no_lg = GeneralPerformanceHelper.BuildRow("Settlement-Break:Lead Gen:Settlement (#)", dataFormat: "number0");
            var r_amt_lg = GeneralPerformanceHelper.BuildRow("Settlement-Break:Lead Gen:Settlement ($)", dataFormat: "currency0");
            var r_qua_lg = GeneralPerformanceHelper.DivRow(r_amt_lg, r_no_lg, 2, dataName: "Settlement-Break:Lead Gen:Quality ($)", dataFormat: "currency0");


            output.Add(r_no_lg);
            output.Add(r_amt_lg);
            output.Add(r_qua_lg);
            output.Add(GeneralPerformanceHelper.BlankRow("Lead Gen", "split1"));
            GeneralPerformanceHelper.CloseGroup();

            /////////////Professional Services/////////////////////////////////////////////////////////////////////
            GeneralPerformanceHelper.StartGroup(output, "Professional Services", labelCss: "L_bold L_toggle");

            var r_no_ps = GeneralPerformanceHelper.BuildRow("Settlement-Break:Professional Services:Settlement (#)", dataFormat: "number0");
            var r_amt_ps = GeneralPerformanceHelper.BuildRow("Settlement-Break:Professional Services:Settlement ($)", dataFormat: "currency0");
            var r_qua_ps = GeneralPerformanceHelper.DivRow(r_amt_ps, r_no_ps, 2, dataName: "Settlement-Break:Professional Services:Quality ($)", dataFormat: "currency0");

            output.Add(r_no_ps);
            output.Add(r_amt_ps);
            output.Add(r_qua_ps);
            output.Add(GeneralPerformanceHelper.BlankRow("Professional Services", "split1"));
            GeneralPerformanceHelper.CloseGroup();

            /////////////Others/////////////////////////////////////////////////////////////////////
            GeneralPerformanceHelper.StartGroup(output, "Others", labelCss: "L_bold L_toggle");

            var r_no_os = GeneralPerformanceHelper.BuildRow("Settlement-Break:Others:Settlement (#)", dataFormat: "number0");
            var r_amt_os = GeneralPerformanceHelper.BuildRow("Settlement-Break:Others:Settlement ($)", dataFormat: "currency0");
            var r_qua_os = GeneralPerformanceHelper.DivRow(r_amt_os, r_no_os, 2, dataName: "Settlement-Break:Others:Quality ($)", dataFormat: "currency0");

            output.Add(r_no_os);
            output.Add(r_amt_os);
            output.Add(r_qua_os);
            output.Add(GeneralPerformanceHelper.BlankRow("Others", "split1"));
            GeneralPerformanceHelper.CloseGroup();
        }
    }
}
