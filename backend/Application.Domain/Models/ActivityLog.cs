using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Domain.Models
{

    [Table("activity_log")]
    public class ActivityLog
    {
        public int Id { get; set; }

        public string Activity { get; set; }
        public string Payload { get; set; }
        public string Error { get; set; }
    }
}
