using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Report.Infra.Data.Context;
using OurreachReport.Service;
using Report.API.Log4Net;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using CRMReport.Service;
using Report.Infra.Data.Utlis;
using RMSReport.Service;
using Common.Service;
using AMReport.Service;
using Users.Service;
using Auth.Service;
using System.IO;

namespace Report.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add CORS:
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policyBuilder => policyBuilder
                        .WithOrigins("http://localhost:3000", "http://crm.finbotic.com.au:8077")
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });
            SetConfigurations();

            var mailConnection = Configuration.GetConnectionString("MailDatabase");

            var appConnection = Configuration.GetConnectionString("AppDatabase");

            var datacentreConnection = Configuration.GetConnectionString("DatacentreDatabase");

            var crmConnection = Configuration.GetConnectionString("CrmDatabase");

            var serverVersion = ServerVersion.AutoDetect(mailConnection);

            services.AddDbContext<MailDBContext>(options => options.UseMySql(mailConnection, serverVersion), ServiceLifetime.Transient);

            services.AddDbContext<AppDBContext>(options => options.UseMySql(appConnection, serverVersion), ServiceLifetime.Transient);

            services.AddDbContext<CrmDBContext>(options => options.UseMySql(crmConnection, serverVersion), ServiceLifetime.Transient);

            services.AddDbContext<DatacentreDBContext>(options => options.UseMySql(datacentreConnection, serverVersion), ServiceLifetime.Transient);

            services.AddControllers();

            //Swagger config with jwt auth
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DataCenterReport", Version = "v1" });
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    // 3. JWT config
                    Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            // 2. JWT config
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
            services.AddScoped<IOutReachService, OutReachService>();
            services.AddScoped<IDrawDownService, DrawDownService>();
            services.AddScoped<ICourtesyCallService, CourtesyCallService>();
            services.AddScoped<IPortfolioService, PortfolioService>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IApplicationMonitorService, ApplicationMonitorService>();
            services.AddScoped<IGeneralPerformanceService,GeneralPerformanceService>();
            services.AddScoped<ILeadGenMonitorService,LeadGenMonitorService>();
            services.AddScoped<IReferralDefaultReportService,ReferralDefaultReportService>();
            services.AddScoped<IReferralReportService,ReferralReportService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IReportNoteService, ReportNoteService>();
            services.AddScoped<ICloseLoanAnalysisService, CloseLoanAnalysisService>();
            services.AddScoped<IPortfolioSummaryService, PortfolioSummaryService>();
            services.AddScoped<ILoanPerformanceService, LoanPerformanceService>();
            services.AddScoped<ILenderService, LenderService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddLog4Net();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Report.API v1"));
            }

            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404 && !Path.HasExtension(context.Request.Path.Value))
                {
                    context.Request.Path = "/index.html";
                    await next();
                }
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("CorsPolicy");
            // 1. JWT config
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        #region Other Methods
        private void SetConfigurations()
        {
            Config.DatacentreDatabase = Configuration.GetConnectionString("DatacentreDatabase");
            Config.CrmDatabase = Configuration.GetConnectionString("CrmDatabase");
            Config.MfappDatabase = Configuration.GetConnectionString("AppDatabase");
        }
        #endregion
    }
}
