using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Domain.Models.RMS
{
    [Keyless]
    public class ApplicationMonitorData
    {
        public int app_id { get; set; }
        public DateTime created_date { get; set; }
        public DateTime lastedit_date { get; set; }
        public string applicant_name { get; set; }
        public string referer { get; set; }
        public decimal desired_loan_amt { get; set; }
        public string app_status { get; set; }
        public int offerred_loan_amt { get; set; }
        public string account_mgr { get; set; }
    }
}
