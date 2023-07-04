using Report.Domain.Models.Outreach;
using Microsoft.EntityFrameworkCore;

namespace Report.Infra.Data.Context
{
    public class MailDBContext : DbContext
    {
        public MailDBContext(DbContextOptions<MailDBContext> options) : base(options)
        {

        }
        public DbSet<DailyEventReportData> DailyEventReportData { get; set; }
        public DbSet<MonthlyEventReportData> MonthlyEventReportData { get; set; }
        public DbSet<QueueStatusData> QueueStatusData { get; set; }

        public DbSet<EventSendingStatusData> EventSendingStatusData { get; set; }
        public DbSet<ApplicationOfDateData> ApplicationOfDateData { get; set; }
        public DbSet<EventTypeData> EventTypeData { get; set; }
        public DbSet<MonthlyDomainReportData> MonthlyDomainReportData { get; set; }
    }
}
