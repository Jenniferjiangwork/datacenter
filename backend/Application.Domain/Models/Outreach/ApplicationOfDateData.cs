using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Domain.Models.Outreach
{
    [Keyless]
    public class ApplicationOfDateData
    {
        public int appId { get; set; }

        public Decimal desiredAmount { get; set; }
        public DateTime createdDate { get; set; }
        public string firstname { get; set; }
        public string surname { get; set; }
        public string mobile { get; set; }
        public int crmAppId { get; set; }

        public int loanId { get; set; }

        public Decimal loanAmount { get; set; }

        public string mcFinalStatus { get; set; }

        public string detail { get; set; }
    }
}
