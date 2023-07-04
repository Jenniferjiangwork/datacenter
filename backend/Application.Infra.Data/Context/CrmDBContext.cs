using Microsoft.EntityFrameworkCore;
using Report.Domain.Models.CRM;
using Report.Domain.Models.RMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Infra.Data.Context
{
    public class CrmDBContext : DbContext
    {
        public CrmDBContext(DbContextOptions<CrmDBContext> options) : base(options)
        {

        }

        public DbSet<ReferralReportDetail> ReferralReportDetail { get; set; }
        public DbSet<ReferralDefaultData> ReferralDefaultData { get; set; }
        public DbSet<ReferralDefaultDetail> ReferralDefaultDetail { get; set; }
        public DbSet<GeneralPerformanceUpdatedDate> GeneralPerfUpdatedDate_crm { get; set; }
        public DbSet<DrawdownUpdatedDate> DrawdownUpdatedDate_crm { get; set; }
        public DbSet<LenderData> LenderData { get; set; }
    }
}
