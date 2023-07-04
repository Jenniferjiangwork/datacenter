using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Infra.Data.Utlis
{
    public class DbHelper
    {
        public static string DBConnectionString { get; set; }

        static DbHelper()
        {
            DBConnectionString = Config.DatacentreDatabase;
        }

        public static DbConnection DbConnection
        {
            get
            {
                var con = new MySqlConnection(DBConnectionString);
                con.Open();
                return con;
            }
        }

        //Set param and it's value to dbparameter
        public static DbParameter SetParameter(string name, object value)
        {
            DbParameter dataParameter = new MySqlParameter();
            dataParameter.ParameterName = name;
            dataParameter.Value = value;
            return dataParameter;
        }

        public static DataSet ExecuteDataset(string commandString, DbParameter[] commandParameters)
        {
            DbConnection cn = new MySqlConnection(DBConnectionString);
            DbCommand cmd = new MySqlCommand(commandString, cn as MySqlConnection);

            foreach (DbParameter parm in commandParameters)
            {
                cmd.Parameters.Add(parm);
            }

            DbDataAdapter da = new MySqlDataAdapter();
            DataSet ds = new DataSet();

            try
            {
                cn.Open();
                da.SelectCommand = cmd;
                da.Fill(ds);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
            return ds;
        }

        //over load Excute sql store_Procedure with param and return dataset
        public static DataTable ExecuteDatasetSP(string spName, DbParameter[] commandParameters)
        {
            DbConnection cn = new MySqlConnection(DBConnectionString);
            DbCommand cmd = new MySqlCommand(spName, cn as MySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure; //StoredProcedure

            foreach (DbParameter parm in commandParameters)
            {
                cmd.Parameters.Add(parm);
            }

            DbDataAdapter da = new MySqlDataAdapter();
            DataTable ds = new DataTable();

            try
            {
                cn.Open();
                da.SelectCommand = cmd;
                da.Fill(ds);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
            return ds;
        }

    }
}
