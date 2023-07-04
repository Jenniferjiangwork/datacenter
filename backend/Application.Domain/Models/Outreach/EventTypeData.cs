using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Domain.Models.Outreach
{
    [Keyless]
    public class EventTypeData
    {
        public string eventType { get; set; }
        public int dailyLimitAll { get; set; }
        public int dailyLimit { get; set; }
        public int dailyLimit2 { get; set; }
        public string versions { get; set; }
        public string versionids { get; set; }
    }
}
