using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Domain.Models.RMS
{
    [Keyless]
    public class ReferralDefaultData
    {
        public string referrer_id { get; set; }
        public string company_name { get; set; }
        public decimal cash_amt { get; set; }
        public decimal total_rec_amt { get; set; }
        public int repaying_no { get; set; }
        public int default_no { get; set; }
        public int recovery_no { get; set; }
        public int closed_no { get; set; }
        public int settled_no { get; set; }
        public int writtenoff_no { get; set; }
        public int total_no { get; set; }

    }

    [Keyless]
    public class ReferralDefaultDetail
    {
        public string referrer_id { get; set; }
        public DateTime settleDate { get; set; }
        public string LoanNo { get; set; }
        public string LoanStatus { get; set; }
        public decimal cashamount { get; set; }
        public decimal LoanBalance { get; set; }
        public decimal TotalReceived { get; set; }
        public decimal Arrears { get; set; }
    }
}

