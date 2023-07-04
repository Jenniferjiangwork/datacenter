using Report.Domain.Models.Outreach;
using Microsoft.EntityFrameworkCore;
using Report.Domain.Models;
using Report.Domain.Models.CRM;
using Report.Domain.Models.Users;
using Report.Domain.Models.RMS;
using Report.Domain.Models.AM;

namespace Report.Infra.Data.Context
{
    public class DatacentreDBContext : DbContext
    {
        public DatacentreDBContext(DbContextOptions<DatacentreDBContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<ActivityLog> ActivityLog { get; set; }
        public DbSet<LeadGenMonitorData> LeadGenMonitorData { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<AdminUserTable> AdminUserTable { get; set; }
        public DbSet<AdminRole> AdminRole { get; set; }
        public DbSet<CloseLoanAnalysisData> CloseLoanAnalysisData { get; set; }
        public DbSet<PortfolioSummaryData> PortfolioSummaryData { get; set; }
        public DbSet<LoanPerformanceColumn> LoanPerformanceColumn { get; set; }
    }
}
