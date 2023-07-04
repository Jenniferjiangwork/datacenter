using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.Domain.Models.Outreach
{
    [Keyless]
    public class DailyEventReportData
    {
        public string eventType { get; set; }

        public DateTime createdDate { get; set; }


        public int sendOut { get; set; }

        public Decimal unsub { get; set; }

        public int bounced { get; set; }

        public int opened { get; set; }

        public int clicked { get; set; }

        public int apps { get; set; }

        public Decimal loans { get; set; }

        public Decimal totalCashAmount { get; set; }

        public string version { get; set; }

        public Decimal preApproved { get; set; }
    }

}
