using Report.Domain.Models.Common;
using Report.Domain.Models.Users;
using Report.Infra.Data.Dto.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Service
{
    public interface IAuthService
    {
        Task<ServiceResponse<AdminUser>> GetUserAccountDetail(int userId);
        Task<ServiceResponse<AdminLogin>> Login(AdminUserLoginDto adminUserLoginDto);
    }
}
