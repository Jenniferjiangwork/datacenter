using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Domain.Models.CRM
{
    [Keyless]
    public class DrawdownData
    {
        public string data_name { get; set; }
        public string comment { get; set; }
        public string label { get; set; }
        public string labelCss { get; set; } = "";
        public string dataCss { get; set; } = "";
        public string dataFormat { get; set; } = "";
        public string section { get; set; }
        public List<DrawdownDBCell> values { get; set; } = new List<DrawdownDBCell>();
        public List<DrawdownDBQCell> qs { get; set; } = new List<DrawdownDBQCell>();
        public bool enableDetail { get; set; } = false;
    }

    [Keyless]
    public class DrawdownUpdatedDate
    {
        public DateTime? updatedDate { get; set; }
    }

    public class DrawdownDetailDTO
    {
        public string template { get; set; }
        public List<dynamic> detailData { get; set; }
    }

    public class QueryParam
    {
        public string template { get; set; }
        public string sp { get; set; }
        public object param { get; set; }
    }
    public class DrawdownDBCell
    {
        public int data_year { get; set; }
        public int data_month { get; set; }
        public decimal data_value { get; set; }
    }
    public class DrawdownDBQCell
    {
        public int data_year { get; set; }
        public int q { get; set; }
        public int inQ { get; set; }
        public decimal data_value { get; set; }
    }
    public class DrawdownDBSet
    {
        public List<DrawdownDBRow> rows { get; set; }
        public List<string> groups { get; set; } = new List<string>();
    }

    public class DrawdownDBRow
    {
        public string data_name { get; set; }
        public int data_year { get; set; }
        public int data_month { get; set; }
        public decimal data_value { get; set; }
    }
}
