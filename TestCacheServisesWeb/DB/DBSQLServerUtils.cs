using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace TestCacheServisesWeb.DB
{
    public class DBSQLServerUtils
    {
        public static SqlConnection GetDBConnection(string datasource, string database, string username, string password)
        {
            // Data Source=TRAN-VMWARE\SQLEXPRESS;Initial Catalog=simplehr;Persist Security Info=True;User ID=sa;Password=12345
            // Data Source=MSI\SQLEXPRESS;Initial Catalog=TestDB;Integrated Security=True

            string connString = @"Data Source=" + datasource + ";Initial Catalog="
                       + database + ";Integrated Security=True";

            SqlConnection conn = new SqlConnection(connString);

            return conn;
        }
    }
}
