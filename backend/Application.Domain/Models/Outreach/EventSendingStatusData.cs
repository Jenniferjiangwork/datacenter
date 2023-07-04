using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Report.Domain.Models.Outreach
{
    [Keyless]
    public class EventSendingStatusData
    {
        public string eventType { get; set; }
        public int waiting { get; set; }

        public int today { get; set; }


        public int yesterday { get; set; }

        public int towdaysago { get; set; }

        public int week { get; set; }

        [NotMapped]
        public int? gmailWaiting { get; set; }

        [NotMapped]
        public int? otherWaiting { get; set; }

    }

}
