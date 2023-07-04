using Report.Domain.Models.Common;
using Report.Domain.Models.Outreach;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OurreachReport.Service
{
    public interface IOutReachService
    {
        Task<ServiceResponse<List<DailyEventReportData>>> GetDailyEvent(string? date, string? from, string? to);
        Task<ServiceResponse<List<MonthlyEventReportData>>> GetMonthlyEvent(string from, string to);
        Task<ServiceResponse<List<QueueStatusData>>> GetOutreachMailQueueStatus();
        Task<ServiceResponse<List<EventSendingStatusData>>> GetEventSendingStatus(string from, string to);
        Task<ServiceResponse<List<ApplicationOfDateData>>> GetOutreachApps(string eventType, string template, string date, int year, int month);
        Task<ServiceResponse<List<EventTypeData>>> GetEventType();
        Task<ServiceResponse<List<MonthlyDomainReportData>>> GetMonthlyDomain(string from, string to, string eventType, string template);
        Task<ServiceResponse<List<ResponseSpeedReportData>>> GetResponseSpeed(string from, string to, int days);
    }
}
