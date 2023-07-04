using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Infra.Data.Dto.User
{
    public class UserRegisterDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UserDto
    {
        public int userid { get; set; }
        public string useraccount { get; set; }
        public string ms_useraccount { get; set; }
        public string username { get; set; }
        public string userpassword { get; set; }
        public int user_role { get; set; }
    }
}
