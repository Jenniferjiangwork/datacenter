using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Report.Domain.Models;
using Report.Infra.Data.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Report.Infra.Data.Utlis
{
    public class CommonFunction
    {
        public static string getPath(string catelog, string subcate, IConfiguration _config)
        {
            string path = string.Empty;
            string cpath = _config["CommentSection:Path"];
            string commentpath = _config["CommentSection:CommentPath"];

            if (!Directory.Exists(cpath))
                Directory.CreateDirectory(cpath);

            if (!Directory.Exists(commentpath))
                Directory.CreateDirectory(commentpath);

            path = commentpath + "\\" + catelog;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (!string.IsNullOrEmpty(subcate))
            {
                path = path + "\\" + subcate;
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }

            return path + "\\";
        }

        public static async Task<ActivityLog> SaveActivityLog<A, B>(string activity, A payload, B error, DatacentreDBContext _dataCentreContext)
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

        public static string EncryptPassword(string password)
        {
            byte[] encryptPasswd;
            UTF8Encoding UTF8 = new UTF8Encoding();
            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes("password"));
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;
            byte[] DataToEncrypt = UTF8.GetBytes(password);
            ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
            encryptPasswd = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            return Convert.ToBase64String(encryptPasswd);
        }
           
        public static string DecryptPassword(string encryptPswd)
        {
            byte[] decryptPasswd;
            UTF8Encoding UTF8 = new UTF8Encoding();
            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes("password"));
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;
            byte[] DataToDecrypt = Convert.FromBase64String(encryptPswd);
            ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
            decryptPasswd = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
            return UTF8.GetString(decryptPasswd);
        }
    }
}
