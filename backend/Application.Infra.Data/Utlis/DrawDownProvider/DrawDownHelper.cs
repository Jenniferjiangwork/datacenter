using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Report.Domain.Models.CRM;

namespace Report.Infra.Data.Utlis.DrawDownProvider
{
    public class DrawDownHelper
    {
        private static DrawdownDBSet dataSet = null;
        public static DrawdownDBSet LoadDbSet(bool force, IDbConnection _db)
        {
            if (dataSet == null || force)
            {
                string sql = @"SELECT * FROM dc_rpt_drawdown ORDER BY data_name, data_year, data_month";
                dataSet = new DrawdownDBSet
                {
                    rows = _db.Query<DrawdownDBRow>(sql).ToList()
                };
            }
            return dataSet;
        }
        public static void StartGroup(List<DrawdownData> output, string groupName, DrawdownData row)
        {
            dataSet.groups.Add(groupName);
            if (row != null)
            {
                row.section = string.Join(":", dataSet.groups.ToArray()).Replace("$", "").Replace("+", "");
            }
            output.Add(row);
        }

        public static void StartGroup(List<DrawdownData> output, string groupName, string label = "", string labelCss = "L_bold", string comment = "")
        {
            dataSet.groups.Add(groupName);
            var data_name = string.Join("-", dataSet.groups.ToArray());
            label = label == "" ? groupName.Split(new char[] { ':' }).Last() : label;

            output.Add(
                new DrawdownData
                {
                    data_name = data_name,
                    label = label,
                    labelCss = labelCss,
                    dataCss = "",
                    dataFormat = "",
                    comment = comment == "" ? data_name.Replace("$", "").Replace("+", "") : comment,
                    section = string.Join(":", dataSet.groups.ToArray()).Replace("$", "").Replace("+", "")
                });
        }

        public static DrawdownData BuildRow(
            string dataName, string comment = "", string labelCss = "", string dataCss = "", string dataFormat = "", string label = "", bool enableDetail = false)
        {
            DrawdownData result = new DrawdownData
            {
                label = String.IsNullOrEmpty(label) ? dataName.Split(new char[] { ':' }).Last() : label,
                labelCss = labelCss,
                dataCss = dataCss,
                dataFormat = dataFormat,
                data_name = dataName,
                comment = comment == "" ? dataName.Replace("$", "").Replace("+", "") : comment,
                section = string.Join(":", dataSet.groups.ToArray()).Replace("$", "").Replace("+", ""),
                enableDetail = enableDetail
            };
            List<DrawdownDBRow> set = dataSet.rows.Where(r => r.data_name == dataName).ToList();
            foreach (var r in set)
            {
                result.values.Add(
                    new DrawdownDBCell
                    {
                        data_month = r.data_month,
                        data_year = r.data_year,
                        data_value = r.data_value
                    });
            }
            BuildQ(result);
            return result;
        }

        public static DrawdownData DivRow(DrawdownData row1, DrawdownData row2, int bit = 4,
            string dataName = "", string comment = "", string label = "", string labelCss = "", string dataCss = "", string dataFormat = "")
        {
            DrawdownData result = new DrawdownData
            {
                data_name = dataName,
                comment = comment == "" ? dataName.Replace("$", "").Replace("+", "") : comment,
                dataCss = dataCss,
                dataFormat = dataFormat,
                label = label == "" ? dataName.Split(new char[] { ':' }).Last() : label,
                labelCss = labelCss,
                section = string.Join(":", dataSet.groups.ToArray()).Replace("$", "").Replace("+", "")
            };
            foreach (var cell in row1.values)
            {
                var resultCell = result.values.Find(r => r.data_year == cell.data_year && r.data_month == cell.data_month);
                if (resultCell == null)
                {
                    resultCell = new DrawdownDBCell
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

        public static DrawdownData BlankRow(string groupName, string rowName, string section = "")
        {
            return new DrawdownData
            {
                data_name = $"{groupName}:{rowName}",
                label = "",
                labelCss = "",
                dataCss = "",
                dataFormat = "",
                comment = $"{groupName}:{rowName}",
                section = string.Join(":", dataSet.groups.ToArray()).Replace("$", "").Replace("+", "")
            };
        }

        public static void CloseGroup()
        {
            dataSet.groups.RemoveAt(dataSet.groups.Count - 1);
        }

        public static DrawdownData SumRow(DrawdownData row1, DrawdownData row2,
            string dataName = "", string comment = "", string label = "", string labelCss = "", string dataCss = "", string dataFormat = "")
        {
            DrawdownData result = new DrawdownData
            {
                data_name = dataName,
                comment = comment == "" ? dataName.Replace("$", "").Replace("+", "") : comment,
                dataCss = dataCss,
                dataFormat = dataFormat,
                label = label == "" ? dataName.Split(new char[] { ':' }).Last() : label,
                labelCss = labelCss,
                section = string.Join(":", dataSet.groups.ToArray()).Replace("$", "").Replace("+", "")
            };
            foreach (var cell in row1.values)
            {
                var resultCell = result.values.Find(r => r.data_year == cell.data_year && r.data_month == cell.data_month);
                if (resultCell == null)
                {
                    resultCell = new DrawdownDBCell
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
                    resultCell = new DrawdownDBCell
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

        public static void BuildQ(DrawdownData row)
        {
            foreach (var cell in row.values)
            {
                int qNum = (cell.data_month - 1) / 3 + 1;
                var qCell = row.qs.Find(q => q.data_year == cell.data_year && q.q == qNum);
                if (qCell == null)
                {
                    qCell = new DrawdownDBQCell { data_year = cell.data_year, q = qNum, inQ = 0 };
                    row.qs.Add(qCell);
                }
                qCell.inQ++;
                qCell.data_value += cell.data_value;
            }
        }
    }
}
