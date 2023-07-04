using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Domain.Models.Users
{
    [Table("admin_user")]
    public class AdminUserTable
    {
        [Key]
        public int userid { get; set; }
        public string useraccount { get; set; }
        public string username { get; set; }
        public string userpassword { get; set; }
        public string ms_useraccount { get; set; }
        public int user_role { get; set; }
    }

    [Keyless]
    public class AdminUser
    {
        public int userid { get; set; }
        public string useraccount { get; set; }
        public string username { get; set; }
        public string userpassword { get; set; }
        public string ms_useraccount { get; set; }
        public int user_role { get; set; }
        public string rolename { get; set; }
    }

    [Keyless]
    public class AdminRole
    {
        public int roleid { get; set; }
        public string rolename { get; set; }
    }
}
