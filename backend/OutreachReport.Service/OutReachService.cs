using Report.Domain.Models.Outreach;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Report.Infra.Data.Context;
using System.Net;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using log4net;
using Report.Domain.Models;
using Report.Domain.Models.Common;

namespace OurreachReport.Service
{
    public class OutReachService : IOutReachService
    {
        private readonly MailDBContext _mailDBContext;
        private readonly AppDBContext _appDBContext;
        private readonly IConfiguration _config;

        private readonly DatacentreDBContext _dataCentreContext;


        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public OutReachService(DatacentreDBContext context, MailDBContext mailDBContext, AppDBContext appDBContext, IConfiguration config)
        {
            _dataCentreContext = context;
            _mailDBContext = mailDBContext;
            _appDBContext = appDBContext;
            _config = config;
        }

        public async Task<ServiceResponse<List<DailyEventReportData>>> GetDailyEvent(string? date, string? from, string? to)
        {
            var serviceResponse = new ServiceResponse<List<DailyEventReportData>>();

            if (date != null)
            {
                from = Convert.ToDateTime(date).ToString("yyyy-M-d");
                to = Convert.ToDateTime(date).AddDays(1).ToString("yyyy-M-d");
            }

            try
            {
                string condition = "1=1";

                if (from != null)
                {
                    condition += $@" and outreach_detail.sent_at >= '{from}'";

                }
                if (to != null)
                {
                    condition += $@" and outreach_detail.sent_at <= '{to}'";

                }
                if (condition != "1=1")
                {
                    condition = $@"({ condition})";

                }

                string sql = $@"
						 select
							root.event_type as `eventType`,
							root.SendDate as createdDate ,
							root.send_out as sendOut,
							root.unsub as unsub,
							ifnull(bounce.bounced, 0) as bounced,
							ifnull(` OPEN`.opened, 0) as opened,
							ifnull(click.clicked, 0) as clicked,
							ifnull(apps, 0) as apps,
							ifnull(loans, 0) as loans,
							ifnull(total_cash_amount, 0) as totalCashAmount,
							'' as version,
							ifnull(conversion.pre_approved, 0) as preApproved
						from
							(
							select
								outreach.`event_type`,
								DATE(outreach_detail.sent_at) as SendDate,
								count(distinct outreach_detail.email) as send_out ,
								sum(if(mail_blacklist.id is null, 0, 1)) as unsub
							from
								outreach
							join outreach_detail on
								outreach.id = outreach_detail.outreach_id
								and outreach_detail.sent_at>'1970-01-01'
							left join mail.mail_blacklist on
								mail_blacklist.email = outreach_detail.Email
								and reason = 'unsub'
							where
								{condition}
							group by
								event_type,
								SendDate) as root
						left join(
							select
								outreach.`event_type` ,
								DATE(outreach_detail.sent_at) as SendDate,
								count(distinct mail_event.email) as bounced
							from
								mail_event
							join outreach on
								outreach.mail_id = mail_event.mail_id
							join outreach_detail on
								outreach.id = outreach_detail.outreach_id
								and outreach_detail.sent_at>'1970-01-01'
								and outreach_detail.Email = mail_event.email
							where
								`event` in ('hard_bounce', 'hard_bounce')
									and {condition}
								group by
									event_type,
									SendDate) as bounce on
							bounce.event_type = root.event_type
							and bounce.SendDate = root.SendDate
						left join(
							select
								outreach.`event_type` ,
								DATE(outreach_detail.sent_at) as SendDate,
								count(distinct mail_event.email) as opened
							from
								mail_event
							join outreach on
								outreach.mail_id = mail_event.mail_id
							join outreach_detail on
								outreach.id = outreach_detail.outreach_id
								and outreach_detail.sent_at>'1970-01-01'
								and outreach_detail.Email = mail_event.email
							where
								`event` in ('open')
									and {condition}
								group by
									event_type,
									SendDate) as ` OPEN` on
							` OPEN`.event_type = root.event_type
							and ` OPEN`.SendDate = root.SendDate
						left join(
							select
								outreach.`event_type` ,
								DATE(outreach_detail.sent_at) as SendDate,
								count(distinct mail_event.email) as clicked
							from
								mail_event
							join outreach on
								outreach.mail_id = mail_event.mail_id
							join outreach_detail on
								outreach.id = outreach_detail.outreach_id
								and outreach_detail.sent_at>'1970-01-01'
								and outreach_detail.Email = mail_event.email
							where
								`event` in ('click')
									and {condition}
								group by
									event_type,
									SendDate) as `click` on
							`click`.event_type = root.event_type
							and `click`.SendDate = root.SendDate
						left join(
							select
								outreach.`event_type` ,
								DATE(outreach_detail.sent_at) as SendDate,
								count(distinct outreach_conversion.app_id ) as apps,
								sum( if(loan_id>0, 1, 0)) as loans,
								sum(loan_amount) as total_cash_amount,
								sum( if(mc_final_status = 'Pass' or mc_final_status = 'Manual Respond', 1, 0)) as pre_approved
							from
								outreach
							join outreach_detail on
								outreach.id = outreach_detail.outreach_id
								and outreach_detail.sent_at>'1970-01-01'
							join outreach_conversion on
								outreach_conversion.detail_id = outreach_detail.id
							join mfapp.crm_application on
								mfapp.crm_application.app_id = outreach_conversion.app_id
							where
								{condition}
							group by
								event_type,
								SendDate) as `conversion` on
							conversion.event_type = root.event_type
							and `conversion`.SendDate = root.SendDate
						order by
							root.`event_type`,
							root.SendDate
						 ";

                sql = Regex.Replace(sql, @"\t|\n|\r", " ");


                List<DailyEventReportData> rs = await _mailDBContext.DailyEventReportData.FromSqlRaw($"{sql}").ToListAsync();

                if (date != null)
                {
                    List<DailyEventReportData> summaryInfo = rs.GroupBy(d => new { d.createdDate })
                .Select(groupData => new DailyEventReportData
                {
                    sendOut = groupData.Sum(d => d.sendOut),
                    bounced = groupData.Sum(d => d.bounced),
                    opened = groupData.Sum(d => d.opened),
                    clicked = groupData.Sum(d => d.clicked),
                    unsub = groupData.Sum(d => d.unsub),
                    apps = groupData.Sum(d => d.apps),
                    preApproved = groupData.Sum(d => d.preApproved),
                    totalCashAmount = groupData.Sum(d => d.totalCashAmount),
                    loans = groupData.Sum(d => d.loans),
                    eventType = "Summary",
                    version = "",
                }).ToList();

                    rs.AddRange(summaryInfo);
                }
                serviceResponse.Data = rs;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                log.Error(ex.Message);
                var result = await SaveActivityLog<string, Exception>("GetDailyEvent", "GetDailyEvent", ex);
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error Id: {result.Id}";
            }
            return serviceResponse;

        }

        public async Task<ServiceResponse<List<MonthlyEventReportData>>> GetMonthlyEvent(string from, string to)
        {

            var serviceResponse = new ServiceResponse<List<MonthlyEventReportData>>();
            try
            {
                string condition = "1=1";

                if (from != null)
                {
                    condition += $@" and outreach_detail.sent_at >= '{from}'";

                }
                if (to != null)
                {
                    condition += $@" and outreach_detail.sent_at <= '{to}'";

                }
                if (condition != "1=1")
                {
                    condition = $@"({ condition})";

                }

                string sql = $@"select root.event as eventType, root.templateId,root.year,root.month,root.send_out as sendOut,root.unsub, ifnull(bounce.bounced,0) as bounced, ifnull(`open`.opened,0) as opened, ifnull(click.clicked,0) as clicked,
							ifnull(apps,0) as apps, ifnull(loans,0) as loans, ifnull(total_cash_amount,0) as totalCashAmount,
							outreach_parameter.description as version, ifnull(conversion.pre_approved,0) as preApproved,
							ifnull(waiting.waiting,0) as waiting
						from (select 
								outreach.`event_type` as `event`, 
								outreach_detail.template_id as `templateId`, 
								Year(outreach_detail.sent_at) as `year`, 
								Month(outreach_detail.sent_at) as `month`,
								count(distinct outreach_detail.email) as send_out ,
								sum(if(mail_blacklist.id is null,0,1)) as unsub
							from outreach
							join outreach_detail  ON outreach.id = outreach_detail.outreach_id  and outreach_detail.sent_at>'1970-01-01'
							left join mail.mail_blacklist on mail_blacklist.email=outreach_detail.Email and reason='unsub'
							where {condition}
							group by `event`,outreach_detail.template_id, `year`, `month`) as root
						left join (
							select 
								outreach.`event_type` as `event_type`, 
								outreach_detail.template_id as `templateId`, 
								Year(outreach_detail.sent_at) as `year`, 
								Month(outreach_detail.sent_at) as `month`,
								count(distinct mail_event.email) as bounced 
							from outreach
							join outreach_detail  ON outreach.id = outreach_detail.outreach_id and outreach_detail.sent_at>'1970-01-01' 
							join mail_event ON outreach.mail_id = mail_event.mail_id and mail_event.email = outreach_detail.email          
							where `event` in ('hard_bounce', 'hard_bounce') and {condition}
							group by `event`,outreach_detail.template_id, `year`, `month`) as bounce on 
								bounce.`event_type` = root.`event` 
								and bounce.`templateId` = root.`templateId` 
								and bounce.`year` = root.`year` 
								and bounce.`month` = root.`month`
						left join (
							select 
								outreach.`event_type` as `event_type`, 
								outreach_detail.template_id as `templateId`, 
								Year(outreach_detail.sent_at) as `year`, 
								Month(outreach_detail.sent_at) as `month`,
								count(distinct mail_event.email) as opened  
							from outreach
							join outreach_detail  ON outreach.id = outreach_detail.outreach_id and outreach_detail.sent_at>'1970-01-01'           
							join mail_event ON outreach.mail_id = mail_event.mail_id and mail_event.email = outreach_detail.email  
							where `event` in ('open') and {condition}
							group by `event`,outreach_detail.template_id, `year`, `month`) as `open` on
								`open`.`event_type` = root.`event` 
								and `open`.`templateId` = root.`templateId` 
								and `open`.`year` = root.`year` 
								and `open`.`month` = root.`month`

						left join (
							select 
								outreach.`event_type` as `event_type`, 
								outreach_detail.template_id as `templateId`, 
								Year(outreach_detail.sent_at) as `year`, 
								Month(outreach_detail.sent_at) as `month`,
								count(distinct mail_event.email) as clicked 
							from outreach
							join outreach_detail  ON outreach.id = outreach_detail.outreach_id and outreach_detail.sent_at>'1970-01-01'           
							join mail_event ON outreach.mail_id = mail_event.mail_id and mail_event.email = outreach_detail.email
							where `event` in ('click') and {condition}
							group by `event`,outreach_detail.template_id, `year`, `month`) as `click` on
								`click`.`event_type` = root.`event` 
								and `click`.`templateId` = root.`templateId` 
								and `click`.`year` = root.`year` 
								and `click`.`month` = root.`month`
						left join (
							select 
								outreach.`event_type` as `event_type`, 
								outreach_detail.template_id as `templateId`, 
								Year(outreach_detail.sent_at) as `year`, 
								Month(outreach_detail.sent_at) as `month`,
								count( distinct outreach_conversion.app_id ) as apps,
								sum( if(loan_id>0,1,0)) as loans,
								sum( loan_amount ) as total_cash_amount,
								sum( if(mc_final_status = 'Pass' or mc_final_status = 'Manual Respond', 1, 0)) as pre_approved
							from outreach
							join outreach_detail  ON outreach.id = outreach_detail.outreach_id and outreach_detail.sent_at>'1970-01-01'
							join outreach_conversion ON outreach_conversion.detail_id = outreach_detail.id
							join mfapp.crm_application on mfapp.crm_application.app_id = outreach_conversion.app_id
							where {condition}
							group by `event_type`,outreach_detail.template_id, `year`, `month`) as `conversion` on
								`conversion`.`event_type` = root.`event` 
								and `conversion`.`templateId` = root.`templateId` 
								and `conversion`.`year` = root.`year` 
								and `conversion`.`month` = root.`month`
						left join outreach_parameter on root.`templateId` = outreach_parameter.id
						left join (
							SELECT 
								outreach.`event_type` as `event_type`, 
								outreach_detail.template_id as `templateId`, 
								Year(outreach.createdDate) as `year`, 
								Month(outreach.createdDate) as `month`,
								count(outreach_detail.id) as `waiting`
							FROM outreach_detail
							join outreach_parameter on outreach_detail.template_id = outreach_parameter.id
							join outreach on outreach.id = outreach_detail.outreach_id
									and outreach.createdDate >= DATE_ADD(curdate(), INTERVAL -1*`day_range` DAY)
							where sent_at<'1970-01-01' and {condition}
							group by `event_type`,outreach_detail.template_id, `year`, `month`
						) as `waiting` on `waiting`.`event_type` = root.`event` 
								and `waiting`.`templateId` = root.`templateId` 
								and `waiting`.`year` = root.`year` 
								and `waiting`.`month` = root.`month`
						order by 1,3,4,2";


                sql = Regex.Replace(sql, @"\t|\n|\r", " ");


                List<MonthlyEventReportData> rs = await _mailDBContext.MonthlyEventReportData.FromSqlRaw($"{sql}").ToListAsync();



                List<MonthlyEventReportData> summaryInfo = rs.GroupBy(d => new { d.year, d.month })
                                .Select(groupData => new MonthlyEventReportData
                                {
                                    sendOut = groupData.Sum(d => d.sendOut),
                                    bounced = groupData.Sum(d => d.bounced),
                                    opened = groupData.Sum(d => d.opened),
                                    clicked = groupData.Sum(d => d.clicked),
                                    unsub = groupData.Sum(d => d.unsub),
                                    apps = groupData.Sum(d => d.apps),
                                    preApproved = groupData.Sum(d => d.preApproved),
                                    waiting = groupData.Sum(d => d.waiting),
                                    totalCashAmount = groupData.Sum(d => d.totalCashAmount),
                                    loans = groupData.Sum(d => d.loans),
                                    eventType = "Summary",
                                    version = "",
                                    year = groupData.Max(d => d.year),
                                    month = groupData.Max(d => d.month),
                                    templateId = 0,
                                }).ToList();

                summaryInfo.AddRange(rs);

                serviceResponse.Data = summaryInfo;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                log.Error(ex.Message);
                var result = await SaveActivityLog<string, Exception>("GetMonthlyEvent", "GetMonthlyEvent", ex);
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error Id: {result.Id}";
            }

            return serviceResponse;

        }

        public async Task<ServiceResponse<List<EventSendingStatusData>>> GetEventSendingStatus(string from, string to)
        {
            var serviceResponse = new ServiceResponse<List<EventSendingStatusData>>();
            try
            {
                string condition = "1=1";

                if (from != null)
                {
                    condition += $@" and outreach_detail.sent_at >= '{from}'";

                }
                if (to != null)
                {
                    condition += $@" and outreach_detail.sent_at <= '{to}'";

                }
                if (condition != "1=1")
                {
                    condition = $@"({ condition})";

                }

                string sql = $@"SELECT 
							outreach.`event_type` as `eventType`, 
							sum(if(sent_at<'1970-01-01' and outreach.createdDate >= DATE_ADD(curdate(), INTERVAL -1*`day_range` DAY), 1,0)) as `waiting`,
							sum(if(sent_at>=curdate(), 1,0)) as `today`,
							sum(if(sent_at>=curdate()-interval 1 day and sent_at<curdate(), 1,0)) as `yesterday`,
							sum(if(sent_at>=curdate()-interval 2 day and sent_at<curdate()-interval 1 day, 1,0)) as 'towdaysago',
							sum(if(sent_at>=curdate()-interval 6 day,1,0))  as 'week'
						FROM outreach_detail
						join outreach_parameter on outreach_detail.template_id = outreach_parameter.id
						join outreach on outreach.id = outreach_detail.outreach_id
						where {condition}
							and (
								(sent_at<'1970-01-01' and outreach.createdDate >= DATE_ADD(curdate(), INTERVAL -1*`day_range` DAY))
								or sent_at>=curdate()-interval 6 day
							)
						group by `eventType`";

                sql = Regex.Replace(sql, @"\t|\n|\r", " ");



                List<EventSendingStatusData> rs = await _mailDBContext.EventSendingStatusData.FromSqlRaw($"{sql}").ToListAsync();

                List<OutreachWaitingData> records = null;

                using (WebClient client = new WebClient())
                {

                    string s = client.DownloadString(_config["URLBases:LiveCRMBase"] + "/form_view_maxfinance/Outreach_waiting.php");
                    records = JsonConvert.DeserializeObject<List<OutreachWaitingData>>(s);

                }


                EventSendingStatusData summaryInfo = new EventSendingStatusData
                {
                    today = rs.Sum(d => d.today),
                    yesterday = rs.Sum(d => d.yesterday),
                    towdaysago = rs.Sum(d => d.towdaysago),
                    week = rs.Sum(d => d.week),
                    gmailWaiting = records.Sum(r => r.gmail_waiting),
                    otherWaiting = records.Sum(r => r.other_waiting),
                    waiting = records.Sum(r => r.all_waiting),
                    eventType = "Summary"

                };

                rs.Insert(0, summaryInfo);

                foreach (EventSendingStatusData e in rs)
                {
                    foreach (OutreachWaitingData o in records)
                    {
                        if (e.eventType == o.event_type)
                        {
                            e.waiting = o.all_waiting;
                            e.gmailWaiting = o.gmail_waiting;
                            e.otherWaiting = o.other_waiting;
                        }
                    }
                }

                serviceResponse.Data = rs;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                log.Error(ex.Message);
                var result = await SaveActivityLog<string, Exception>("GetEventSendingStatus", "GetEventSendingStatus", ex);
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error Id: {result.Id}";
            }
            return serviceResponse;
        }


        public async Task<ServiceResponse<List<ApplicationOfDateData>>> GetOutreachApps(string eventType, string template, string date, int year, int month)
        {
            eventType = eventType == "Summary" ? "" : eventType;
            var serviceResponse = new ServiceResponse<List<ApplicationOfDateData>>();
            try
            {
                string from = null;
                string to = null;

                if (date != null)
                {
                    from = date;
                    to = date;
                }
                else
                {
                    from = new DateTime(year, month, 1).ToString("yyyy-M-d");
                    to = new DateTime(year, month, DateTime.DaysInMonth(year, month)).ToString("yyyy-M-d");
                }
                string sql = $@"select a.app_id as appId, a.DesiredAmount as desiredAmount, a.CreatedDate as createdDate, ac.FirstName as firstname, ac.Surname as surname, ac.Mobile as mobile, oc.crm_app_id as crmAppId, oc.loan_id as loanId, oc.loan_amount as loanAmount, a.mc_final_status as mcFinalStatus,come.detail
							from outreach_conversion as oc
							join mfapp.crm_application as a on a.app_id=oc.app_id
							join mfapp.crm_application_comesfrom as come on a.app_id=come.app_id
							join mfapp.crm_contact as ac on ac.app_id=a.app_id and NumberFromAppinfo=1
							join outreach_detail as od on oc.detail_id=od.id and (od.template_id={template} or {template}<=0)
							join outreach as o on 
								od.outreach_id=o.id 
								and o.CreatedDate >= '{from}' 
								and o.CreatedDate <= '{to} '
								and (o.event_type= '{eventType}' or '{eventType}'='')";

                sql = Regex.Replace(sql, @"\t|\n|\r", " ");



                List<ApplicationOfDateData> rs = await _mailDBContext.ApplicationOfDateData.FromSqlRaw($"{sql}").ToListAsync();

                serviceResponse.Data = rs;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                log.Error(ex.Message);
                var result = await SaveActivityLog<string, Exception>("GetOutreachApps", "GetOutreachApps", ex);
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error Id: {result.Id}";
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<QueueStatusData>>> GetOutreachMailQueueStatus()
        {
            var serviceResponse = new ServiceResponse<List<QueueStatusData>>();
            try
            {
                string sql = $@"SELECT year(created_at) as `year`, month(created_at) as `month`, date(created_at) as `date`,
							sum(if(status=0,1,0)) as waiting,
							sum(if(status=1,1,0)) as sending,
							sum(if(status=4,1,0)) as failed,
							sum(if(status=2,1,0)) as sent,
							max(mailclass) as maxMailClass
								FROM email_queue where 
									mailclass like '%OutreachEmail' 
								and created_at>=curdate() - interval 5 day
							group by `year`, `month`, `date`";

                List<QueueStatusData> rs = await _mailDBContext.QueueStatusData.FromSqlRaw($"{sql}").ToListAsync();

                serviceResponse.Data = rs;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                log.Error(ex.Message);
                var result = await SaveActivityLog<string, Exception>("GetOutreachMailQueueStatus", "GetOutreachMailQueueStatus", ex);
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error Id: {result.Id}";
            }
            return serviceResponse;
        }


        public async Task<ServiceResponse<List<EventTypeData>>> GetEventType()
        {
            var serviceResponse = new ServiceResponse<List<EventTypeData>>();
            try
            {

                string sql = $@"SELECT event_type as eventType,  max(daily_limit_all) as dailyLimitAll,  max(daily_limit) as dailyLimit, 
							max(daily_limit2) as dailyLimit2, group_concat(description)  as versions, group_concat(id) as versionids
							FROM mail.outreach_parameter where 	active=1 group by event_type";

                sql = Regex.Replace(sql, @"\t|\n|\r", " ");



                List<EventTypeData> rs = await _mailDBContext.EventTypeData.FromSqlRaw($"{sql}").ToListAsync();

                serviceResponse.Data = rs;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                log.Error(ex.Message);
                var result = await SaveActivityLog<string, Exception>("GetEventType", "GetEventType", ex);
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error Id: {result.Id}";
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<MonthlyDomainReportData>>> GetMonthlyDomain(string from, string to, string eventType, string template)
        {
            var serviceResponse = new ServiceResponse<List<MonthlyDomainReportData>>();
            try
            {

                string condition = "1=1";
                string join = "";
                string joinCondition = "";

                if (from != null)
                {
                    condition += $@" and outreach_detail.sent_at >= '{from}'";

                }
                if (to != null)
                {
                    condition += $@" and outreach_detail.sent_at <= '{to}'";

                }
                if (eventType != null)
                {
                    condition += $@" and outreach.event_type =  '{eventType}'";

                }
                if (template != null)
                {
                    join += $@" join (select id, `template` from outreach_parameter ) as op on outreach_detail.template_id = op.id and op.`template` = '{template}' ";
                    joinCondition = $@" outreach_parameter.`template` = '{template}' ";
                }
                else
                {
                    join += $@" ";
                    joinCondition = "1=1";
                }
                if (condition != "1=1")
                {
                    condition = $@"({ condition})";

                }


                string sql = $@"select root.domain,root.year, root.month,root.send_out as sendOut,root.unsub, ifnull(bounce.bounced,0) as bounced, ifnull(`open`.opened,0) as opened, ifnull(click.clicked,0) as clicked,
                    ifnull(apps,0) as apps, ifnull(loans,0) as loans, ifnull(total_cash_amount,0) as totalCashAmount,
                    ifnull(conversion.pre_approved,0) as preApproved,
                    ifnull(waiting.waiting,0) as waiting
                from (select 
                        case 
                            when outreach_detail.Email like '%@gmail.com' then 'Gmail'
                            when outreach_detail.Email like '%@hotmail.com' then 'HotMail'
                            else 'Others'
                        end as domain,
                        Year(outreach_detail.sent_at) as `year`, 
                        Month(outreach_detail.sent_at) as `month`,
                        count(distinct outreach_detail.email) as send_out ,
                        sum(if(mail_blacklist.id is null,0,1)) as unsub
                    from outreach
                    join outreach_detail  ON outreach.id = outreach_detail.outreach_id  and outreach_detail.sent_at>'1970-01-01'
                    {join}
                    left join mail.mail_blacklist on mail_blacklist.email=outreach_detail.Email and reason='unsub'
                    where {condition}
                    group by domain, `year`, `month`) as root
                left join (
                    select 
                        case 
                            when outreach_detail.Email like '%@gmail.com' then 'Gmail'
                            when outreach_detail.Email like '%@hotmail.com' then 'HotMail'
                            else 'Others'
                        end as domain,
                        Year(outreach_detail.sent_at) as `year`, 
                        Month(outreach_detail.sent_at) as `month`,
                        count(distinct mail_event.email) as bounced 
                    from outreach
                    join outreach_detail  ON outreach.id = outreach_detail.outreach_id and outreach_detail.sent_at>'1970-01-01' 
                    {join}
                    join mail_event ON outreach.mail_id = mail_event.mail_id and mail_event.email = outreach_detail.email          
                    where `event` in ('hard_bounce', 'hard_bounce') and {condition}
                    group by domain, `year`, `month`) as bounce on 
                        bounce.domain = root.domain 
                        and bounce.`year` = root.`year` 
                        and bounce.`month` = root.`month`
                left join (
                    select 
                        case 
                            when outreach_detail.Email like '%@gmail.com' then 'Gmail'
                            when outreach_detail.Email like '%@hotmail.com' then 'HotMail'
                            else 'Others'
                        end as domain,
                        Year(outreach_detail.sent_at) as `year`, 
                        Month(outreach_detail.sent_at) as `month`,
                        count(distinct mail_event.email) as opened  
                    from outreach
                    join outreach_detail  ON outreach.id = outreach_detail.outreach_id and outreach_detail.sent_at>'1970-01-01'           
                    {join}
                    join mail_event ON outreach.mail_id = mail_event.mail_id and mail_event.email = outreach_detail.email  
                    where `event` in ('open') and {condition}
                    group by domain, `year`, `month`) as `open` on
                        `open`.domain = root.domain  
                        and `open`.`year` = root.`year` 
                        and `open`.`month` = root.`month`

                left join (
                    select 
                        case 
                            when outreach_detail.Email like '%@gmail.com' then 'Gmail'
                            when outreach_detail.Email like '%@hotmail.com' then 'HotMail'
                            else 'Others'
                        end as domain,
                        Year(outreach_detail.sent_at) as `year`, 
                        Month(outreach_detail.sent_at) as `month`,
                        count(distinct mail_event.email) as clicked 
                    from outreach
                    join outreach_detail  ON outreach.id = outreach_detail.outreach_id and outreach_detail.sent_at>'1970-01-01'           
                    {join}
                    join mail_event ON outreach.mail_id = mail_event.mail_id and mail_event.email = outreach_detail.email
                    where `event` in ('click') and {condition}
                    group by domain, `year`, `month`) as `click` on
                        `click`.domain = root.domain  
                        and `click`.`year` = root.`year` 
                        and `click`.`month` = root.`month`
                left join (
                    select 
                        case 
                            when outreach_detail.Email like '%@gmail.com' then 'Gmail'
                            when outreach_detail.Email like '%@hotmail.com' then 'HotMail'
                            else 'Others'
                        end as domain, 
                        Year(outreach_detail.sent_at) as `year`, 
                        Month(outreach_detail.sent_at) as `month`,
                        count( distinct outreach_conversion.app_id ) as apps,
                        sum( if(loan_id>0,1,0)) as loans,
                        sum( loan_amount ) as total_cash_amount,
                        sum( if(mc_final_status = 'Pass' or mc_final_status = 'Manual Respond', 1, 0)) as pre_approved
                    from outreach
                    join outreach_detail  ON outreach.id = outreach_detail.outreach_id and outreach_detail.sent_at>'1970-01-01'
                    {join}
                    join outreach_conversion ON outreach_conversion.detail_id = outreach_detail.id
                    join mfapp.crm_application on mfapp.crm_application.app_id = outreach_conversion.app_id
                    where {condition}
                    group by domain, `year`, `month`) as `conversion` on
                        `conversion`.domain = root.domain 
                        and `conversion`.`year` = root.`year` 
                        and `conversion`.`month` = root.`month`
                left join (
                    SELECT 
                        case 
                            when outreach_detail.Email like '%@gmail.com' then 'Gmail'
                            when outreach_detail.Email like '%@hotmail.com' then 'HotMail'
                            else 'Others'
                        end as domain, 
                        Year(outreach.createdDate) as `year`, 
                        Month(outreach.createdDate) as `month`,
                        count(outreach_detail.id) as `waiting`
                    FROM outreach_detail
                    join (select id, `template`, `day_range` from outreach_parameter) as outreach_parameter on outreach_detail.template_id = outreach_parameter.id and {joinCondition}
                    join outreach on outreach.id = outreach_detail.outreach_id
                            and outreach.createdDate >= DATE_ADD(curdate(), INTERVAL -1*`day_range` DAY)
                    where sent_at<'1970-01-01' and {condition}
                    group by domain, `year`, `month`
                ) as `waiting` on `waiting`.domain = root.domain 
                        and `waiting`.`year` = root.`year` 
                        and `waiting`.`month` = root.`month`
                order by 1,2,3";


                sql = Regex.Replace(sql, @"\t|\n|\r", " ");


                List<MonthlyDomainReportData> rs = await _mailDBContext.MonthlyDomainReportData.FromSqlRaw($"{sql}").ToListAsync();



                List<MonthlyDomainReportData> summaryInfo = rs.GroupBy(d => new { d.year, d.month })
                                .Select(groupData => new MonthlyDomainReportData
                                {
                                    sendOut = groupData.Sum(d => d.sendOut),
                                    bounced = groupData.Sum(d => d.bounced),
                                    opened = groupData.Sum(d => d.opened),
                                    clicked = groupData.Sum(d => d.clicked),
                                    unsub = groupData.Sum(d => d.unsub),
                                    apps = groupData.Sum(d => d.apps),
                                    preApproved = groupData.Sum(d => d.preApproved),
                                    waiting = groupData.Sum(d => d.waiting),
                                    totalCashAmount = groupData.Sum(d => d.totalCashAmount),
                                    loans = groupData.Sum(d => d.loans),
                                    domain = "Summary",
                                    year = groupData.Max(d => d.year),
                                    month = groupData.Max(d => d.month),

                                }).ToList();

                summaryInfo.AddRange(rs);

                serviceResponse.Data = summaryInfo;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                log.Error(ex.Message);
                var result = await SaveActivityLog<string, Exception>("GetMonthlyDomain", "GetMonthlyDomain", ex);
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error Id: {result.Id}";
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<ResponseSpeedReportData>>> GetResponseSpeed(string from, string to, int days)
        {
            var serviceResponse = new ServiceResponse<List<ResponseSpeedReportData>>();
            try
            {
                string condition = "1=1";

                if (from != null)
                {
                    condition += $@" and outreach_detail.sent_at >= '{from}'";

                }
                if (to != null)
                {
                    condition += $@" and outreach_detail.sent_at <= '{to}'";

                }
                if (condition != "1=1")
                {
                    condition = $@"({ condition})";

                }

                string sql = $@"select outreach_event as outreachEvent, count(*) as appCount
                            from 
                                (
                                    SELECT crm_application.app_id, detail, substring_index(detail,'_',-1) as mid, CreatedDate
                                    FROM crm_application_comesfrom 
                                    join crm_application on crm_application.app_id = crm_application_comesfrom.app_id
                                    where `source`='email' and detail like 'Outreach%'
                                ) as p
                            join mail.outreach_detail on outreach_detail.id = p.mid and DATEDIFF(CreatedDate,sent_at)<={days} and {condition}
                            group by outreach_event";


                List<ResponseSpeedReportData> rs = await _appDBContext.ResponseSpeedReportData.FromSqlRaw($"{sql}").ToListAsync();

                serviceResponse.Data = rs;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                log.Error(ex.Message);
                var result = await SaveActivityLog<string, Exception>("GetResponseSpeed", "GetResponseSpeed", ex);
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error Id: {result.Id}";
            }
            return serviceResponse;
        }

        private async Task<ActivityLog> SaveActivityLog<A, B>(string activity, A payload, B error)
        {
            var activityLog = new ActivityLog()
            {
                Activity = activity,
                Payload = JsonConvert.SerializeObject(payload),
                Error = JsonConvert.SerializeObject(error)
            };

            _dataCentreContext.ActivityLog.Add(activityLog);
            await _dataCentreContext.SaveChangesAsync();

            return activityLog;
        }


    }
}


