using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Infra.Data.Utlis
{
    public class DbFactory
    {
        IDbConnection _db;

        public DbFactory(string connectionString)
        {
            _db = new MySqlConnection(connectionString);
        }

        public IDbConnection db
        {
            get { return _db; }
        }
    }
}

