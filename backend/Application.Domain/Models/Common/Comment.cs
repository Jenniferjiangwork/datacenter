using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Domain.Models.Common
{
    [Keyless]
    public class Comment
    {
        public string comments { get; set; }
        public string colors { get; set; }
    }
}
