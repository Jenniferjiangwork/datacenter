using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Report.Domain.Models.Outreach
{

    [Keyless]
    public class MonthlyEventReportData
    {
        public int year { get; set; }

        public int month { get; set; }


        public string eventType { get; set; }

        public string version { get; set; }

        public int templateId { get; set; }

        public int sendOut { get; set; }

        public Decimal unsub { get; set; }

        public int bounced { get; set; }

        public int opened { get; set; }

        public int clicked { get; set; }

        public int apps { get; set; }

        public Decimal loans { get; set; }

        public Decimal totalCashAmount { get; set; }

        public int waiting { get; set; }

        public Decimal preApproved { get; set; }


    }


}
