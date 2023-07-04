using Report.Domain.Models.RMS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Infra.Data.Utlis.GeneralPerformanceProvider
{
    public class ServicePerformanceProvider
    {
        public static void ServicePerformance(GeneralPerformanceDBSet rawData, List<GeneralPerformanceData> output, IDbConnection _db)
        {
            ///////////Service Performance:///////////////////////////////////////////////////////////////////
            GeneralPerformanceHelper.StartGroup(output, "Service Performance", labelCss: "L_bold L_toggle");

            ///////////Service Performance:Inbound (#) ///////////////////////////////////////////////////////////////////
            var r_inbond = GeneralPerformanceHelper.BuildRow("Service Performance:Inbound (#)", dataFormat: "number0");
            GeneralPerformanceHelper.StartSubGroup(output, "Service Performance:Inbound (#)", r_inbond);
            List<GeneralPerformanceUser> userInbondList = GeneralPerformanceHelper.GetInbondUsers(_db);
            foreach (var item in userInbondList)
            {
                string data_name = "Service Performance:Inbound (#):" + item.userId;
                var r_inbond_u = GeneralPerformanceHelper.BuildSubGrpRow(dataName: data_name, label: item.userName, dataFormat: "number0");
                output.Add(r_inbond_u);
            }
            GeneralPerformanceHelper.CloseSubGroup();

            ///////////Service Performance:Outbound (#)///////////////////////////////////////////////////////////////////
            var r_outbond = GeneralPerformanceHelper.BuildRow("Service Performance:Outbound (#)", dataFormat: "number0");
            GeneralPerformanceHelper.StartSubGroup(output, "Service Performance:Outbound (#)", r_outbond);
            List<GeneralPerformanceUser> userOutboundList = GeneralPerformanceHelper.GetOutbondUsers(_db);
            foreach (var item in userOutboundList)
            {
                string data_name = "Service Performance:Outbound (#):" + item.userId;
                var r_outbound_u = GeneralPerformanceHelper.BuildSubGrpRow(dataName: data_name, label: item.userName, dataFormat: "number0");
                output.Add(r_outbound_u);
            }
            GeneralPerformanceHelper.CloseSubGroup();

            ///////////Service Performance:Email Interaction (#) ///////////////////////////////////////////////////////////////////
            var r_email = GeneralPerformanceHelper.BuildRow("Service Performance:Email Interaction (#)", dataFormat: "number0");
            GeneralPerformanceHelper.StartSubGroup(output, "Service Performance:Email Interaction (#)", r_email);
            List<GeneralPerformanceUser> userMailList = GeneralPerformanceHelper.GetMailInteractionUsers(_db);
            foreach (var item in userMailList)
            {
                string data_name = "Service Performance:Email Interaction (#):" + item.userId;
                var r_email_u = GeneralPerformanceHelper.BuildSubGrpRow(dataName: data_name, label: item.userName, dataFormat: "number0");
                output.Add(r_email_u);
            }
            GeneralPerformanceHelper.CloseSubGroup();

            ///////////Service Performance:Contact Attempts (#) ///////////////////////////////////////////////////////////////////
            var r_contact = GeneralPerformanceHelper.BuildRow("Service Performance:Contact Attempts (#)", dataFormat: "number0");
            GeneralPerformanceHelper.StartSubGroup(output, "Service Performance:Contact Attempts (#)", r_contact);
            List<GeneralPerformanceUser> userContactList = GeneralPerformanceHelper.GetContactAttemptsUsers(_db);
            foreach (var item in userContactList)
            {
                string data_name = "Service Performance:Contact Attempts (#):" + item.userId;
                var r_contact_u = GeneralPerformanceHelper.BuildSubGrpRow(dataName: data_name, label: item.userName, dataFormat: "number0");
                output.Add(r_contact_u);
            }
            GeneralPerformanceHelper.CloseSubGroup();

            //////////////////////////////////////////////////////////////////////////////
            output.Add(GeneralPerformanceHelper.BlankRow("Service Performance", "split1"));
            GeneralPerformanceHelper.CloseGroup();
        }
    }
}

