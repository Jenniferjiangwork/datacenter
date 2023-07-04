using log4net;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Report.Domain.Models;
using Report.Domain.Models.Common;
using Report.Infra.Data.Context;
using Report.Infra.Data.Utlis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common.Service
{
    public class CommentService : ICommentService
    {
        private readonly IConfiguration _config;
        private readonly DatacentreDBContext _dataCentreContext;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public CommentService(DatacentreDBContext dataCentreContext, IConfiguration config)
        {
            _config = config;
            _dataCentreContext = dataCentreContext;
        }

        public async Task<ServiceResponse<string>> GetCommentColor(string catelog, string subcate)
        {
            var serviceResponse = new ServiceResponse<string>();
            try
            {
                var comments = new Dictionary<string, object>();
                var colors = new Dictionary<string, object>();
                string path = CommonFunction.getPath(catelog, subcate, _config);
                foreach (string filename in Directory.GetFiles(path, "*.txt"))
                {
                    string commentname = filename.Replace(path, "").Replace(".txt", "");
                    comments[commentname] = JsonConvert.DeserializeObject(File.ReadAllText(filename));
                }

                foreach (string filename in Directory.GetFiles(path, "*.color"))
                {
                    string colorname = filename.Replace(path, "").Replace(".color", "");
                    colors[colorname] = JsonConvert.DeserializeObject(File.ReadAllText(filename));
                }
                var result = JsonConvert.SerializeObject(new
                {
                    comments = comments,
                    colors = colors
                });
                serviceResponse.Data = result;               
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                log.Error(ex.Message);
                var result = await CommonFunction.SaveActivityLog<string, Exception>("GetCommentColor", "GetCommentColor", ex, _dataCentreContext);
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error Id: {result.Id}";
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<bool>> SaveComment(string catelog, string subcate, string key, string content)
        {
            var serviceResponse = new ServiceResponse<bool>();
            try
            {
                string filename = CommonFunction.getPath(catelog, subcate, _config) + key + ".txt";
                File.WriteAllText(filename, JsonConvert.SerializeObject(content));
                serviceResponse.Data =  true;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                log.Error(ex.Message);
                var result = await CommonFunction.SaveActivityLog<string, Exception>("SaveComment", "SaveComment", ex, _dataCentreContext);
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error Id: {result.Id}";
            }
            return serviceResponse;
        }
        public async Task<ServiceResponse<bool>> RemoveComment(string catelog, string subcate, string key)
        {
            var serviceResponse = new ServiceResponse<bool>();
            try
            {
                string filename = CommonFunction.getPath(catelog, subcate, _config) + key + ".txt";
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                    serviceResponse.Data = true;
                } 
                else
                {
                    serviceResponse.Data = false;
                }              
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                log.Error(ex.Message);
                var result = await CommonFunction.SaveActivityLog<string, Exception>("RemoveComment", "RemoveComment", ex, _dataCentreContext);
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error Id: {result.Id}";
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<bool>> SaveColor(string catelog, string subcate, string key, string color)
        {
            var serviceResponse = new ServiceResponse<bool>();
            try
            {
                string filename = CommonFunction.getPath(catelog, subcate, _config) + key + ".color";
                File.WriteAllText(filename, JsonConvert.SerializeObject(color));
                serviceResponse.Data = true;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                log.Error(ex.Message);
                var result = await CommonFunction.SaveActivityLog<string, Exception>("SaveColor", "SaveColor", ex, _dataCentreContext);
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error Id: {result.Id}";
            }
            return serviceResponse;
        }
        public async Task<ServiceResponse<bool>> RemoveColor(string catelog, string subcate, string key)
        {
            var serviceResponse = new ServiceResponse<bool>();
            try
            {
                string filename = CommonFunction.getPath(catelog, subcate, _config) + key + ".color";
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                    serviceResponse.Data = true;
                }
                else
                {
                    serviceResponse.Data = false;
                }
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                log.Error(ex.Message);
                var result = await CommonFunction.SaveActivityLog<string, Exception>("RemoveColor", "RemoveColor", ex, _dataCentreContext);
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error Id: {result.Id}";
            }
            return serviceResponse;
        }
    }
}

