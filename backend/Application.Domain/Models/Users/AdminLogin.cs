using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Domain.Models.Users
{
    [Keyless]
    public class AdminLogin
    {
        public int userid { get; set; }
        public string useraccount { get; set; }
        public string ms_useraccount { get; set; }
        public string username { get; set; }
        public int user_role { get; set; }
        public string rolename { get; set; }
        public string accessToken { get; set; }

    }
}
