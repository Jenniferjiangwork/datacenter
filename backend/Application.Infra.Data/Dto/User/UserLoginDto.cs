using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Infra.Data.Dto.User
{
    public class UserLoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class AdminUserLoginDto
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}
