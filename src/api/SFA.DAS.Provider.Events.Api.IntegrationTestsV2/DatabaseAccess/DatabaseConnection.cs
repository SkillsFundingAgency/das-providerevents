using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using Dapper;

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

        public static string BaseDirectory =>
            System.IO.Path.GetDirectoryName(typeof(DatabaseConnection).Assembly.Location);

        public async Task RunScriptfile(string relativePath)
        {
            var path = Path.Combine(BaseDirectory, $"{relativePath}.sql");
            var file = File.ReadAllText(path);

            var commands = file.Split(new[] {"GO"}, StringSplitOptions.RemoveEmptyEntries);
            using (var conn = Connection())
            {
                await conn.OpenAsync().ConfigureAwait(false);

                foreach (var command in commands)
                {
                    await conn.ExecuteAsync(command).ConfigureAwait(false);
                }
                conn.Close();
            }
        }
    }
}
