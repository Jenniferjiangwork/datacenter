using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Domain.Models.CRM
{
    public class CourtesyCallDBSet
    {
        public List<CourtesyCallDBRow> rows { get; set; }
        public List<string> operators { get; set; }
    }
    public class CourtesyCallDBRow
    {
        public string data_operator { get; set; }
        public int data_year { get; set; }
        public int data_month { get; set; }
        public string data_name { get; set; }
        public decimal data_value { get; set; }
    }
    public class CourtesyCallRow
    {
        public string Operator { get; set; }
        public decimal Value { get; set; }

    }
    public class CourtesyCall
    {
        public string Name { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public List<CourtesyCallRow> Data { get; set; }
    }

    public class CourtesyCallOutput
    {
        public List<string> Operators { get; set; }
        public List<CourtesyCall> List { get; set; }
    }
}
