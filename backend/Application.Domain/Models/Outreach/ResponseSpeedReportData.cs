using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Domain.Models.Outreach
{
    [Keyless]
    public class ResponseSpeedReportData
    {
        public string outreachEvent { get; set; }

        public int appCount { get; set; }
    }
}
