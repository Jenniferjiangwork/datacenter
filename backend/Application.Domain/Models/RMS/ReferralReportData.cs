using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Domain.Models.RMS
{
    [Keyless]
    public class ReferralReportData
    {
        public string referrer_id { get; set; }
        public string referrer_name { get; set; }
        public int submit_no { get; set; }
        public int preapproved_no { get; set; }
        public int settled_no { get; set; }
        public decimal settled_amt { get; set; }
        public decimal coversion_no { get; set; }
       public decimal coversion_amt { get; set; }
    }

    [Keyless]
    public class ReferralReportDetail
    {
        public string referrer_id { get; set; }
        public DateTime settleDate { get; set; }
        public string LoanNo { get; set; }
        public string LoanStatus { get; set; }
        public decimal? cashamount { get; set; }
        public decimal? LoanBalance { get; set; }
        public decimal? TotalReceived { get; set; }
        public decimal? Arrears { get; set; }
    }
}
