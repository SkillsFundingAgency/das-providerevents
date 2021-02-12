using System.Configuration;
using System.Data.SqlClient;

namespace SFA.DAS.Provider.Events.Api.IntegrationTestsV2.DatabaseAccess
{
    public class DatabaseConnection
    {
        public static SqlConnection Connection()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
            var connection = new SqlConnection(connectionString);
            return connection;
        }
    }
}
