using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Domain.Models.RMS
{
    [Keyless]
    public class GeneralPerformanceData
    {
        public string data_name { get; set; }
        public string comment { get; set; }
        public string label { get; set; }
        public string labelCss { get; set; } = "";
        public string dataCss { get; set; } = "";
        public string dataFormat { get; set; } = "";
        public string section { get; set; }
        public List<GeneralPerformanceDBCell> values { get; set; } = new List<GeneralPerformanceDBCell>();
        public List<GeneralPerformanceDBQCell> qs { get; set; } = new List<GeneralPerformanceDBQCell>();
        public bool enableDetail { get; set; } = false;
        public bool isGrpRow { get; set; } = false;
        public bool isSubGrpRow { get; set; } = false;
    }

    [Keyless]
    public class GeneralPerformanceDBSet
    {
        public List<GeneralPerformanceDBRow> rows { get; set; }
        public List<string> groups { get; set; } = new List<string>();

        public List<string> subGroups { get; set; } = new List<string>();
    }

    [Keyless]
    public class GeneralPerformanceDBRow
    {
        public string data_name { get; set; }
        public int data_year { get; set; }
        public int data_month { get; set; }
        public decimal data_value { get; set; }
    }

    [Keyless]
    public class GeneralPerformanceUpdatedDate
    {
        public DateTime updatedDate { get; set; }
    }
    public class GeneralPerformanceDBCell
    {
        public int data_year { get; set; }
        public int data_month { get; set; }
        public decimal data_value { get; set; }
    }
    public class GeneralPerformanceDBQCell
    {
        public int data_year { get; set; }
        public int q { get; set; }
        public int inQ { get; set; }
        public decimal data_value { get; set; }
    }

    public class GeneralPerformanceDetailDTO
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

    public class GeneralPerformanceUser
    {
        public string userId { get; set; }
        public string userName { get; set; }
    }
}

