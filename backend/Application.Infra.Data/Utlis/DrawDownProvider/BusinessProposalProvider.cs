using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Report.Domain.Models.CRM;

namespace Report.Infra.Data.Utlis.DrawDownProvider
{
    public class BusinessProposalProvider
    {
        public static void BusinessProposal(DrawdownDBSet rawDataSet, List<DrawdownData> output)
        {
            DrawDownHelper.StartGroup(output, "Business Proposal", labelCss: "L_bold L_toggle");
            output.Add(DrawDownHelper.BuildRow("Business Proposal:Fast Loan", label: "Fast Loan", dataFormat: "number0", comment: "Business Proposal-Fast-Loan"));
            output.Add(DrawDownHelper.BuildRow("Business Proposal:Fast Biz +", label: "Fast Biz +", dataFormat: "number0", comment: "Business Proposal-Fast-Biz"));
            output.Add(DrawDownHelper.BuildRow("Business Proposal:Premium Business", label: "Premium Business", dataFormat: "number0", comment: "Business Proposal-Premium-Business"));
            output.Add(DrawDownHelper.BuildRow("Business Proposal:USB", label: "USB", dataFormat: "number0", comment: "Business Proposal-USB"));
            output.Add(DrawDownHelper.BlankRow("Business Proposal", "split1"));
            DrawDownHelper.CloseGroup();
        }
    }
}
