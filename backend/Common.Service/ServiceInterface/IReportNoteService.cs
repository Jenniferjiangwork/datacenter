using Report.Domain.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Service
{
    public interface IReportNoteService
    {
        Task<ServiceResponse<string>> GetReportNote(string reportname, string company);
        Task<ServiceResponse<int>> SaveReportNote(string reportname, string reportnote, string company);
    }
}
