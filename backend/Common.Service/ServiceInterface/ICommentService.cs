using Report.Domain.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Service
{
    public interface ICommentService
    {
        Task<ServiceResponse<string>> GetCommentColor(string catelog, string subcate);
        Task<ServiceResponse<bool>> SaveComment(string catelog, string subcate, string key, string content);
        Task<ServiceResponse<bool>> RemoveComment(string catelog, string subcate, string key);
        Task<ServiceResponse<bool>> SaveColor(string catelog, string subcate, string key, string color);
        Task<ServiceResponse<bool>> RemoveColor(string catelog, string subcate, string key);
    }
}
