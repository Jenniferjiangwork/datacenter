using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Report.Domain.Models.CRM;

namespace Report.Infra.Data.Utlis.DrawDownProvider
{
    public class BusinessApplicationFlowPPTYProvider
    {
        public static void BusinessApplicationFlowPPTY(DrawdownDBSet rawDataSet, List<DrawdownData> output)
        {
            DrawDownHelper.StartGroup(output, "Business Application Flow(PPTY)", labelCss: "L_bold L_toggle");
            var r_finish_form = output.Find(r => r.data_name == "Business Form:Total");
            var r_submit_form = DrawDownHelper.BuildRow("Business Application Flow:Submit Form(PPTY)", dataFormat: "number0");
            var r_finish_submit = DrawDownHelper.DivRow(r_finish_form, r_submit_form, 3,
                dataName: "Business Application Flow:Started Form > Submit Form(PPTY)", dataFormat: "percent1");
            output.Add(r_finish_submit);

            var r_pass_idecision = DrawDownHelper.BuildRow("Business Application Flow:Pass iDecision(PPTY)", dataFormat: "number0");
            var r_pass_idecision_finish = DrawDownHelper.DivRow(r_pass_idecision, r_finish_form, 2,
                dataName: "Business Application Flow:Pass iDecision > Finish Form(PPTY)", dataFormat: "percent0");
            output.Add(r_pass_idecision_finish);
            var r_bank = DrawDownHelper.BuildRow("Business Application Flow:Bank Retrieve(PPTY)", dataFormat: "number0");
            var r_bank_pass_idecision = DrawDownHelper.DivRow(r_bank, r_pass_idecision, 2,
                dataName: "Business Application Flow:Bank Retrieve > Pass iDecision(PPTY)", dataFormat: "percent0");
            output.Add(r_bank_pass_idecision);
            var r_pre_approved = output.Find(r => r.data_name == "Business Form:Total Pre-Approved");
            var r_all_doc = DrawDownHelper.BuildRow("Business Application Flow:All Docs(PPTY)", dataFormat: "number0");
            var r_all_doc_pre_approve = DrawDownHelper.DivRow(r_all_doc, r_pre_approved, 2,
                dataName: "Business Application Flow:All Docs > Pre Approved(PPTY)", dataFormat: "percent0");
            output.Add(r_all_doc_pre_approve);
            var r_final_verification = DrawDownHelper.BuildRow("Business Application Flow:Final Verification(PPTY)", dataFormat: "number0");
            var r_final_verification_pre_approve = DrawDownHelper.DivRow(r_final_verification, r_pre_approved, 2,
                dataName: "Business Application Flow:Final Verification > Pre Approved(PPTY)", dataFormat: "percent0");
            output.Add(r_final_verification_pre_approve);
            var r_proposal = DrawDownHelper.BuildRow("Business Application Flow:Proposal(PPTY)", dataFormat: "number0");
            var r_proposal_final_verification = DrawDownHelper.DivRow(r_proposal, r_final_verification, 2,
                dataName: "Business Application Flow:Proposal > Final Verification(PPTY)", dataFormat: "percent0");
            output.Add(r_proposal_final_verification);

            var r_contract = DrawDownHelper.BuildRow("Business Application Flow:Contract(PPTY)", dataFormat: "number0");
            var r_drawdown_contract_proposal = DrawDownHelper.DivRow(r_contract, r_proposal, 2,
                dataName: "Business Application Flow:Contract > Proposal(PPTY)", dataFormat: "percent0");
            output.Add(r_drawdown_contract_proposal);


            var r_drawdown = DrawDownHelper.BuildRow("Business Application Flow:1st Draw-down no(PPTY)", dataFormat: "number0");
            var r_drawdown_final_verification = DrawDownHelper.DivRow(r_drawdown, r_proposal, 2,
                dataName: "Business Application Flow:Draw-down > Proposal(PPTY)", dataFormat: "percent0");
            output.Add(r_drawdown_final_verification);

            output.Add(DrawDownHelper.BuildRow("Business Application Flow:Pre-Approved Duration(days) Avg(PPTY)", dataFormat: "number1"));
            output.Add(DrawDownHelper.BuildRow("Business Application Flow:Pre-Approved Duration (days) Medium(PPTY)", dataFormat: "number0"));

            var r_rescue_form = DrawDownHelper.BuildRow("Business Application Flow:Rescue(PPTY)", dataFormat: "number0");
            var r_rescue_submit = DrawDownHelper.DivRow(r_rescue_form, r_submit_form, 3,
                dataName: "Business Application Flow:Rescue > Submit Form(PPTY)", dataFormat: "percent1");
            output.Add(r_rescue_submit);

            output.Add(DrawDownHelper.BlankRow("Business Application Flow", "split1"));
            DrawDownHelper.CloseGroup();
        }
    }
}
