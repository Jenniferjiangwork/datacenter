using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Report.Domain.Models.CRM;

namespace Report.Infra.Data.Utlis.DrawDownProvider
{
    public class BusinessApplicationFlowProvider
    {
        public static void BusinessApplication(DrawdownDBSet rawDataSet, List<DrawdownData> output)
        {
            //////////////////////////////////////////All Application Flow
            ///All Application(%) & (No.)
            DrawDownHelper.StartGroup(output, "All Application (%) & (No.)", labelCss: "L_bold L_toggle");

            ///////////////////////////1 Started Form > Submit Form
            ///1  Start Form > Submit App Form - 'Business Application Flow:Submit Form'
            var r_start_form = output.Find(r => r.data_name == "Business Form:Total");
            var r_submit_app_form = DrawDownHelper.BuildRow("Business Application Flow:Submit Form", dataFormat: "number0");
            var r_1 = DrawDownHelper.DivRow(r_start_form, r_submit_app_form, 3,
                dataName: "All Application Flow:Start Form > Submit App Form", dataFormat: "percent1");
            output.Add(r_1);

            ///////////////////////////2 Pass iDecision > Finish Form
            ///// 2 Submit Form > Pre-approved  -  'Business Application Flow:Pass iDecision'
            var r_pass_idecision = DrawDownHelper.BuildRow("Business Application Flow:Pass iDecision", dataFormat: "number0");
            var r_2 = DrawDownHelper.DivRow(r_pass_idecision, r_start_form, 2,
                dataName: "All Application Flow:Submit Form > Pre-approved", dataFormat: "percent0");
            output.Add(r_2);

            //////////////////////////3 Bank Retrieve > Pass iDecision
            ///3 Bank Retrieve > Pass iDecision  -- 'Business Application Flow:Bank Retrieve'
            var r_bank_retrieve = DrawDownHelper.BuildRow("Business Application Flow:Bank Retrieve", dataFormat: "number0");
            var r_3 = DrawDownHelper.DivRow(r_bank_retrieve, r_pass_idecision, 2,
                dataName: "All Application Flow:Bank Retrieve > Pass iDecision", dataFormat: "percent0");
            output.Add(r_3);

            /////////////////////////
            ///4 Pre-Approved > Final Verification  ---- 'Business Application Flow:Final Verification Pre-Approved'
            var r_4_pre_approved = DrawDownHelper.BuildRow("Business Application Flow:Pre-Approved", dataFormat: "number0");


            var r_pre_approved = output.Find(r => r.data_name == "Business Form:Total Pre-Approved");  // this changed into propose
            //////////////////////// 5 Final Verification > Pre Approved  
            ///5 Final Verification > Proposal -  "Business Application Flow:Final Verification"
            var r_final_verification = DrawDownHelper.BuildRow("Business Application Flow:Final Verification", dataFormat: "number0");
            var r_4 = DrawDownHelper.DivRow(r_final_verification, r_4_pre_approved, 2,
                dataName: "All Application Flow:Pre-Approved > Final Verification", dataFormat: "percent0");

            var r_5 = DrawDownHelper.DivRow(r_final_verification, r_pre_approved, 2,
                dataName: "All Application Flow:Final Verification > Proposal", dataFormat: "percent0");
            output.Add(r_4);
            output.Add(r_5);

            ////////////////////////6 Proposal > Final Verification  HIDE
            /// 6 Proposal > Final Verification -- Business Application Flow:Proposal
            var r_proposal = DrawDownHelper.BuildRow("Business Application Flow:Proposal", dataFormat: "number0");

            /////////////////////////7 Contract > Proposal
            ///7 Proposal > Contract
            var r_contract = DrawDownHelper.BuildRow("Business Application Flow:Contract", dataFormat: "number0");
            var r_7 = DrawDownHelper.DivRow(r_contract, r_proposal, 2,
                dataName: "All Application Flow:Proposal > Contract", dataFormat: "percent0");
            output.Add(r_7);

            /////////////////////////
            ///8 Contract > Settlement  
            var r_8_settlement = DrawDownHelper.BuildRow("Business Application Flow:Settlement", dataFormat: "number0");
            var r_8 = DrawDownHelper.DivRow(r_8_settlement, r_contract, 2,
                dataName: "All Application Flow:Contract > Settlement", dataFormat: "percent0");
            output.Add(r_8);

            ////////////////////////////Draw-down > Proposal
            ///9 Draw-down > Proposal
            var r_drawdown = DrawDownHelper.BuildRow("Business Application Flow:1st Draw-down no", dataFormat: "number0");
            var r_9 = DrawDownHelper.DivRow(r_drawdown, r_proposal, 2,
                dataName: "All Application Flow:Draw-down > Proposal", dataFormat: "percent0");
            output.Add(r_9);

            ////////////////////////////////////// 10 Pre-Approved Duration(days) Avg
            ///10 Pre-Approved Duration(days) Avg
            var r_10 = DrawDownHelper.BuildRow("Business Application Flow:Pre-Approved Duration(days) Avg", dataFormat: "number1");
            output.Add(r_10);

            //////////////////////////////////////11 Pre-Approved Duration (days) Medium
            ///11 Pre-Approved Duration (days) Medium
            var r_11 = DrawDownHelper.BuildRow("Business Application Flow:Pre-Approved Duration (days) Medium", dataFormat: "number0");
            output.Add(r_11);

            ///////////////////////////////////////Rescue > Submit Form
            ///12 Unsubmit Form > Rescue
            var r_rescue_form = DrawDownHelper.BuildRow("Business Application Flow:Rescue", dataFormat: "number0");
            var r_12 = DrawDownHelper.DivRow(r_rescue_form, r_submit_app_form, 3,
                dataName: "All Application Flow:Unsubmit Form > Rescue", dataFormat: "percent1");
            output.Add(r_12);

            ///////////////////////// 4 All Docs > Pre Approved
            /// 13 Pre-Approved > All Doc
            /// // "Business Form:Total Pre-Approved"

            var r_all_doc = DrawDownHelper.BuildRow("Business Application Flow:All Docs", dataFormat: "number0");
            var r_13 = DrawDownHelper.DivRow(r_all_doc, r_pre_approved, 2,
                dataName: "All Application Flow:Pre-Approved > All Doc", dataFormat: "percent0");
            output.Add(r_13);

            output.Add(DrawDownHelper.BlankRow("All Application (%) & (No.)", "split1"));
            DrawDownHelper.CloseGroup();
        }
    }
}
