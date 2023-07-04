using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Report.Domain.Models.CRM;

namespace Report.Infra.Data.Utlis.DrawDownProvider
{
    public class ToProvider
    {
        public static void To(DrawdownDBSet rawDataSet, List<DrawdownData> output)
        {
            var p_fl = DrawDownHelper.BuildRow("Business Proposal:Fast Loan");
            var p_fb = DrawDownHelper.BuildRow("Business Proposal:Fast Biz +");
            var p_p = DrawDownHelper.BuildRow("Business Proposal:Premium Business");
            var p_u = DrawDownHelper.BuildRow("Business Proposal:USB");

            var d_fl = DrawDownHelper.BuildRow("Draw Down:Fast Loan");
            var d_fb = DrawDownHelper.BuildRow("Draw Down:Fast Biz");
            var d_p = DrawDownHelper.BuildRow("Draw Down:Premium");
            var d_u = DrawDownHelper.BuildRow("Draw Down:USB");

            var c_fl = DrawDownHelper.BuildRow("Business Contract:Fast Loan");
            var c_fb = DrawDownHelper.BuildRow("Business Contract:Fast Biz +");
            var c_p = DrawDownHelper.BuildRow("Business Contract:Premium");
            var c_u = DrawDownHelper.BuildRow("Business Contract:USB");


            DrawDownHelper.StartGroup(output, "Proposal to Drawn-Down", label: "% Proposal to Drawn-Down", labelCss: "L_bold L_toggle");
            output.Add(DrawDownHelper.DivRow(d_fl, p_fl, 2, dataName: "Proposal to Drawn-Down:Fast Loan", comment: "Proposal to Drawn-Down-Fast-Loan", label: "Fast Loan", dataFormat: "percent0"));
            output.Add(DrawDownHelper.DivRow(d_fb, p_fb, 2, dataName: "Proposal to Drawn-Down:Fast Biz", comment: "Proposal to Drawn-Down-Fast-Biz", label: "Fast Biz +", dataFormat: "percent0"));
            output.Add(DrawDownHelper.DivRow(d_p, p_p, 2, dataName: "Proposal to Drawn-Down:Premium", comment: "Proposal to Drawn-Down-Premium", label: "Premium Business", dataFormat: "percent0"));
            output.Add(DrawDownHelper.DivRow(d_u, p_u, 2, dataName: "Proposal to Drawn-Down:USB", comment: "Proposal to Drawn-Down-USB", label: "USB", dataFormat: "percent0"));
            output.Add(DrawDownHelper.BlankRow("Proposal to Drawn-Down", "split1"));
            DrawDownHelper.CloseGroup();

            DrawDownHelper.StartGroup(output, "Proposal to Contract", label: "Proposal to Contract", labelCss: "L_bold L_toggle");
            output.Add(DrawDownHelper.DivRow(c_fl, p_fl, 2, dataName: "Proposal to Contract:Fast Loan", comment: "Proposal to Contract-Fast-Loan", label: "Fast Loan", dataFormat: "percent0"));
            output.Add(DrawDownHelper.DivRow(c_fb, p_fb, 2, dataName: "Proposal to Contract:Fast Biz", comment: "Proposal to Contract-Fast-Biz", label: "Fast Biz +", dataFormat: "percent0"));
            output.Add(DrawDownHelper.DivRow(c_p, p_p, 2, dataName: "Proposal to Contract:Premium", comment: "Proposal to Contract-Premium", label: "Premium Business", dataFormat: "percent0"));
            output.Add(DrawDownHelper.DivRow(c_u, p_u, 2, dataName: "Proposal to Contract:USB", comment: "Proposal to Contract-USB", label: "USB", dataFormat: "percent0"));
            output.Add(DrawDownHelper.BlankRow("Proposal to Drawn-Down", "split1"));
            DrawDownHelper.CloseGroup();

            DrawDownHelper.StartGroup(output, "Contract to Drawn-Down", label: "Contract to Drawn-Down", labelCss: "L_bold L_toggle");
            output.Add(DrawDownHelper.DivRow(d_fl, c_fl, 2, dataName: "Contract to Drawn-Down:Fast Loan", comment: "Contract to Drawn-Down-Fast-Loan", label: "Fast Loan", dataFormat: "percent0"));
            output.Add(DrawDownHelper.DivRow(d_fb, c_fb, 2, dataName: "Contract to Drawn-Down:Fast Biz", comment: "Contract to Drawn-Down-Fast-Biz", label: "Fast Biz +", dataFormat: "percent0"));
            output.Add(DrawDownHelper.DivRow(d_p, c_p, 2, dataName: "Contract to Drawn-Down:Premium", comment: "Contract to Drawn-Down-Premium", label: "Premium Business", dataFormat: "percent0"));
            output.Add(DrawDownHelper.DivRow(d_u, c_u, 2, dataName: "Contract to Drawn-Down:USB", comment: "Contract to Drawn-Down-USB", label: "USB", dataFormat: "percent0"));
            output.Add(DrawDownHelper.BlankRow("Contract to Drawn-Down", "split1"));
            DrawDownHelper.CloseGroup();
        }
    }
}
