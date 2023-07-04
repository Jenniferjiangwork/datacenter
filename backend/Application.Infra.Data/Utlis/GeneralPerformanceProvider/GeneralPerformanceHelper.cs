using Dapper;
using Report.Domain.Models.RMS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Infra.Data.Utlis.GeneralPerformanceProvider
{
    public class GeneralPerformanceHelper
    {
        private static GeneralPerformanceDBSet generalPerformanceDBSet = null;

        public static GeneralPerformanceDBSet LoadDbSet(bool force, IDbConnection _db)
        {
            if (generalPerformanceDBSet == null || force)
            {
                string sql = $@"select * 
                    from dc_rpt_generalperf
                    order by data_name, data_year, data_month";

                generalPerformanceDBSet = new GeneralPerformanceDBSet
                {
                    rows = _db.Query<GeneralPerformanceDBRow>(sql).ToList()
                };

            }
            return generalPerformanceDBSet;
        }

        public static void StartGroup(List<GeneralPerformanceData> output, string groupName, string label = "", string labelCss = "L_bold", string comment = "")
        {
            generalPerformanceDBSet.groups.Add(groupName);
            var data_name = string.Join("-", generalPerformanceDBSet.groups.ToArray());
            label = label == "" ? groupName.Split(new char[] { ':' }).Last() : label;

            output.Add(
                new GeneralPerformanceData
                {
                    data_name = data_name,
                    label = label,
                    labelCss = labelCss,
                    dataCss = "",
                    dataFormat = "",
                    isGrpRow = true,
                    comment = comment == "" ? data_name.Replace("$", "").Replace("+", "") : comment,
                    section = string.Join(":", generalPerformanceDBSet.groups.ToArray()).Replace("$", "").Replace("+", "")
                }); ;
        }

        public static GeneralPerformanceData BuildRow(
         string dataName, string comment = "", string labelCss = "", string dataCss = "", string dataFormat = "", string label = "", bool enableDetail = false)
        {
            GeneralPerformanceData result = new GeneralPerformanceData
            {
                label = String.IsNullOrEmpty(label) ? dataName.Split(new char[] { ':' }).Last() : label,
                labelCss = labelCss,
                dataCss = dataCss,
                dataFormat = dataFormat,
                data_name = dataName,
                comment = comment == "" ? dataName.Replace("$", "").Replace("+", "") : comment,
                section = string.Join(":", generalPerformanceDBSet.groups.ToArray()).Replace("$", "").Replace("+", ""),
                enableDetail = enableDetail
            };
            List<GeneralPerformanceDBRow> set = generalPerformanceDBSet.rows.Where(r => r.data_name == dataName).ToList();
            foreach (var r in set)
            {
                result.values.Add(
                    new GeneralPerformanceDBCell
                    {
                        data_month = r.data_month,
                        data_year = r.data_year,
                        data_value = r.data_value
                    });
            }
            BuildQ(result);
            return result;
        }

        public static GeneralPerformanceData DivRow(GeneralPerformanceData row1, GeneralPerformanceData row2, int bit = 4,
           string dataName = "", string comment = "", string label = "", string labelCss = "", string dataCss = "", string dataFormat = "")
        {
            GeneralPerformanceData result = new GeneralPerformanceData
            {
                data_name = dataName,
                comment = comment == "" ? dataName.Replace("$", "").Replace("+", "") : comment,
                dataCss = dataCss,
                dataFormat = dataFormat,
                label = label == "" ? dataName.Split(new char[] { ':' }).Last() : label,
                labelCss = labelCss,
                section = string.Join(":", generalPerformanceDBSet.groups.ToArray()).Replace("$", "").Replace("+", "")
            };
            foreach (var cell in row1.values)
            {
                var resultCell = result.values.Find(r => r.data_year == cell.data_year && r.data_month == cell.data_month);
                if (resultCell == null)
                {
                    resultCell = new GeneralPerformanceDBCell
                    {
                        data_year = cell.data_year,
                        data_month = cell.data_month,
                        data_value = 0
                    };
                    result.values.Add(resultCell);
                }
                var cell2 = row2.values.Find(r => r.data_year == cell.data_year && r.data_month == cell.data_month);
                if (cell2 != null && cell2.data_value > 0)
                {
                    resultCell.data_value = Math.Round(cell.data_value / cell2.data_value, bit, 0);
                }
            }
            BuildQ(result);
            return result;
        }
        public static GeneralPerformanceData BlankRow(string groupName, string rowName, string section = "")
        {
            return new GeneralPerformanceData
            {
                data_name = $"{groupName}:{rowName}",
                label = "",
                labelCss = "",
                dataCss = "",
                dataFormat = "",
                comment = $"{groupName}:{rowName}",
                section = string.Join(":", generalPerformanceDBSet.groups.ToArray()).Replace("$", "").Replace("+", "")
            };
        }
        public static GeneralPerformanceData BuildSubGrpRow(
                string dataName, string comment = "", string labelCss = "", string dataCss = "", string dataFormat = "", string label = "", bool enableDetail = false)
        {
            GeneralPerformanceData result = new GeneralPerformanceData
            {
                label = String.IsNullOrEmpty(label) ? dataName.Split(new char[] { ':' }).Last() : label,
                labelCss = labelCss,
                dataCss = dataCss,
                dataFormat = dataFormat,
                data_name = dataName,
                comment = comment == "" ? dataName.Replace("$", "").Replace("+", "") : comment,
                section = string.Join(":", generalPerformanceDBSet.subGroups.ToArray()).Replace("$", "").Replace("+", ""),
                enableDetail = enableDetail
            };
            List<GeneralPerformanceDBRow> set = generalPerformanceDBSet.rows.Where(r => r.data_name == dataName).ToList();
            foreach (var r in set)
            {
                result.values.Add(
                    new GeneralPerformanceDBCell
                    {
                        data_month = r.data_month,
                        data_year = r.data_year,
                        data_value = r.data_value
                    });
            }
            BuildQ(result);
            return result;
        }
 
        public static void CloseGroup()
        {
            generalPerformanceDBSet.groups.RemoveAt(generalPerformanceDBSet.groups.Count - 1);
        }
        public static GeneralPerformanceData SumRow(GeneralPerformanceData row1, GeneralPerformanceData row2,
            string dataName = "", string comment = "", string label = "", string labelCss = "", string dataCss = "", string dataFormat = "")
        {
            GeneralPerformanceData result = new GeneralPerformanceData
            {
                data_name = dataName,
                comment = comment == "" ? dataName.Replace("$", "").Replace("+", "") : comment,
                dataCss = dataCss,
                dataFormat = dataFormat,
                label = label == "" ? dataName.Split(new char[] { ':' }).Last() : label,
                labelCss = labelCss,
                section = string.Join(":", generalPerformanceDBSet.groups.ToArray()).Replace("$", "").Replace("+", "")
            };
            foreach (var cell in row1.values)
            {
                var resultCell = result.values.Find(r => r.data_year == cell.data_year && r.data_month == cell.data_month);
                if (resultCell == null)
                {
                    resultCell = new GeneralPerformanceDBCell
                    {
                        data_year = cell.data_year,
                        data_month = cell.data_month,
                        data_value = 0
                    };
                    result.values.Add(resultCell);
                }
            }
            foreach (var cell in row2.values)
            {
                var resultCell = result.values.Find(r => r.data_year == cell.data_year && r.data_month == cell.data_month);
                if (resultCell == null)
                {
                    resultCell = new GeneralPerformanceDBCell
                    {
                        data_year = cell.data_year,
                        data_month = cell.data_month,
                        data_value = 0
                    };
                    result.values.Add(resultCell);
                }
            }
            foreach (var cell in result.values)
            {
                var cell1 = row1.values.Find(r => r.data_year == cell.data_year && r.data_month == cell.data_month);
                var cell2 = row2.values.Find(r => r.data_year == cell.data_year && r.data_month == cell.data_month);
                cell.data_value = (cell1 != null ? cell1.data_value : 0) + (cell2 != null ? cell2.data_value : 0);
            }
            BuildQ(result);
            return result;
        }
        public static void StartSubGroup(List<GeneralPerformanceData> output, string subGroupName, GeneralPerformanceData row)
        {
            generalPerformanceDBSet.subGroups.Add(subGroupName);
            if (row != null)
            {
                row.section = string.Join(":", generalPerformanceDBSet.subGroups.ToArray()).Replace("$", "").Replace("+", "");
                row.isSubGrpRow = true;
            }
            output.Add(row);
        }

        public static List<GeneralPerformanceUser> GetInbondUsers(IDbConnection _db)
        {
            var userList = new List<GeneralPerformanceUser>();
            string sql = $@"select userid, username from dc_rpt_generalperf_user where usertype = 'Inbond' ";
            userList = _db.Query<GeneralPerformanceUser>(sql).ToList();
            return userList;
        }

        public static List<GeneralPerformanceUser> GetOutbondUsers(IDbConnection _db)
        {
            List<GeneralPerformanceUser> userList = null;         
            string sql = $@"select userid, username from dc_rpt_generalperf_user where usertype = 'Outbond' ";
            userList = _db.Query<GeneralPerformanceUser>(sql).ToList();        
            return userList;
        }
        public static List<GeneralPerformanceUser> GetMailInteractionUsers(IDbConnection _db)
        {
            List<GeneralPerformanceUser> userList = null;
            string sql = $@"select userid, username from dc_rpt_generalperf_user where usertype = 'Email' ";
            userList = _db.Query<GeneralPerformanceUser>(sql).ToList();
            return userList;
        }

        public static List<GeneralPerformanceUser> GetContactAttemptsUsers(IDbConnection _db)
        {
            List<GeneralPerformanceUser> userList = null;
            string sql = @"select userid, username from dc_rpt_generalperf_user where usertype = 'Contact' ";
            userList = _db.Query<GeneralPerformanceUser>(sql).ToList();
            return userList;
        }

        public static void CloseSubGroup()
        {
            generalPerformanceDBSet.subGroups.RemoveAt(generalPerformanceDBSet.subGroups.Count - 1);
        }

        public static void BuildQ(GeneralPerformanceData row)
        {
            foreach (var cell in row.values)
            {
                int qNum = (cell.data_month - 1) / 3 + 1;
                var qCell = row.qs.Find(q => q.data_year == cell.data_year && q.q == qNum);
                if (qCell == null)
                {
                    qCell = new GeneralPerformanceDBQCell { data_year = cell.data_year, q = qNum, inQ = 0 };
                    row.qs.Add(qCell);
                }
                qCell.inQ++;
                qCell.data_value += cell.data_value;
            }
        }
    }
}
