using Report.Domain.Models.Outreach;
using Microsoft.EntityFrameworkCore;
using Report.Domain.Models.RMS;
using Report.Domain.Models.CRM;

namespace Report.Infra.Data.Context
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {

        }

        public DbSet<ResponseSpeedReportData> ResponseSpeedReportData { get; set; }
        public DbSet<ApplicationMonitorData> ApplicationMonitorData { get; set; }
        public DbSet<ReferralReportData> ReferralReportData { get; set; }
        public DbSet<LeadGenMonitorBarData> LeadGenMonitorBarData { get; set; }
        public DbSet<GeneralPerformanceUpdatedDate> GeneralPerfUpdatedDate_mfapp { get; set; }
        public DbSet<DrawdownUpdatedDate> DrawdownUpdatedDate_mfapp { get; set; }
    }
}
