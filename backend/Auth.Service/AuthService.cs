using Dapper;
using log4net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Report.Domain.Models.Common;
using Report.Domain.Models.Users;
using Report.Infra.Data.Context;
using Report.Infra.Data.Dto.User;
using Report.Infra.Data.Utlis;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Service
{
    public class AuthService : IAuthService
    {
        private readonly DatacentreDBContext _dataCentreContext;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IConfiguration _configuration;
        IDbConnection _db;

        public AuthService(DatacentreDBContext context, IConfiguration configuration)
        {
            _db = new DbFactory(Config.DatacentreDatabase).db;
            _dataCentreContext = context;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<AdminUser>> GetUserAccountDetail(int userId)
        {
            var serviceResponse = new ServiceResponse<AdminUser>();
            try
            {
                string sql = $@"SELECT au.userid, au.useraccount, au.username, au.userpassword, au.ms_useraccount, au.user_role, ar.rolename
                                FROM admin_user au
                                JOIN admin_role ar ON au.user_role = ar.roleid WHERE userid = {userId}";

                AdminUser rs = await _dataCentreContext.AdminUsers.FromSqlRaw($"{sql}").FirstOrDefaultAsync();
                rs.userpassword = CommonFunction.DecryptPassword(rs.userpassword);
                serviceResponse.Data = rs;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                log.Error(ex.Message);
                var result = await CommonFunction.SaveActivityLog<string, Exception>("GetUserAccountDetail", "GetUserAccountDetail", ex, _dataCentreContext);
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error Id: {result.Id}";
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<AdminLogin>> Login(AdminUserLoginDto adminUserLoginDto)
        {
            var serviceResponse = new ServiceResponse<AdminLogin>();
            try
            {
                string passwordEncrypt = CommonFunction.EncryptPassword(adminUserLoginDto.password);
                string sql = $@"SELECT au.userid, au.useraccount, au.username, au.userpassword, au.ms_useraccount, au.user_role, ar.rolename
                                FROM admin_user au
                                JOIN admin_role ar ON au.user_role = ar.roleid WHERE username = '{adminUserLoginDto.username}' and userpassword = '{passwordEncrypt}'";

                AdminLogin rs = await _db.QueryFirstOrDefaultAsync<AdminLogin>(sql);
                if (rs == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "User not found. Invalid username or password!";
                }
                else
                {
                    rs.accessToken = CreateToken(rs);
                }

                serviceResponse.Data = rs;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                log.Error(ex.Message);
                var result = await CommonFunction.SaveActivityLog<string, Exception>("Login", "Login", ex, _dataCentreContext);
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error Id: {result.Id}";
            }
            return serviceResponse;
        }

        private string CreateToken(AdminLogin adminLogin)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, adminLogin.userid.ToString()),
                new Claim(ClaimTypes.Name, adminLogin.username)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokendDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = System.DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokendDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
