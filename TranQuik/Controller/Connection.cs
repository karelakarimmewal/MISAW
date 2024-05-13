using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using TranQuik.Configuration;

namespace TranQuik.Controller
{
    internal class Connection
    {
        public static IDbConnection GetLocalDbConnection()
        {
            // Construct connection string for local MySQL database
            string connectionString = $"Server={DatabaseSettings.LocalDbServer};" +
                                       $"Port={DatabaseSettings.LocalDbPort};" +
                                       $"Database={DatabaseSettings.LocalDbName};" +
                                       $"Uid={DatabaseSettings.LocalDbUser};" +
                                       $"Pwd={DatabaseSettings.LocalDbPassword};";

            return new MySqlConnection(connectionString);
        }

        public static IDbConnection GetCloudDbConnection()
        {
            // Construct connection string for cloud SQL Server database
            string connectionString = $"Server={DatabaseSettings.CloudDbServer};" +
                                       $"Database={DatabaseSettings.CloudDbName};" +
                                       $"User Id={DatabaseSettings.CloudDbUser};" +
                                       $"Password={DatabaseSettings.CloudDbPassword};";

            return new SqlConnection(connectionString);
        }
    }
}
