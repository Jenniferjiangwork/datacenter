using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Domain.Models.Outreach
{
    public class OutreachWaitingData
    {
        public string id { set; get; }
        public string event_type { set; get; }

        public int all_waiting { set; get; }
        public int other_waiting { set; get; }
        public int gmail_waiting { set; get; }
        public string created_at { set; get; }
    }
}
