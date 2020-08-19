using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace TestCacheServisesWeb.DB
{
    public class DBUtils
    {
        public static SqlConnection GetDBConnection()
        {
            string datasource = @"MSI\SQLEXPRESS";
            string database = "Northwind";
            string username = "sa";
            string password = "1234";

            return DBSQLServerUtils.GetDBConnection(datasource, database, username, password);
        }
    }
}
