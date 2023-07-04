using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Domain.Models.RMS
{
    [Keyless]
    public class LeadGenMonitorData
    {
        public int data_year { get; set; }
        public int data_month { get; set; }
        public int data_day { get; set; }
        public string data_name { get; set; }
        public int data_value { get; set; }
    }

    [Keyless]
    public class LeadGenMonitorBarData
    {
        public string Category { get; set; }
        public int Submitted_no { get; set; }
        public int PreApproved_no { get; set; }
    }
}
