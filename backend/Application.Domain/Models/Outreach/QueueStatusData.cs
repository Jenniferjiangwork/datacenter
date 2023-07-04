using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Report.Domain.Models.Outreach
{
    [Keyless]
    public class QueueStatusData
    {
        public int year { get; set; }

        public int month { get; set; }


        public DateTime date { get; set; }

        public int waiting { get; set; }

        public int sending { get; set; }

        public int failed { get; set; }

        public int sent { get; set; }

        public string maxMailClass { get; set; }
    }

}
