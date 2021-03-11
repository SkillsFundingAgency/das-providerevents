using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Dapper;

namespace SFA.DAS.Provider.Events.Api.IntegrationTests.DatabaseAccess
{
    internal class CreateDatabase
    {
        private readonly DatabaseConnection _connection;

        public CreateDatabase(DatabaseConnection connection)
        {
            _connection = connection;
        }

        public async Task CreateSubmissionEvents()
        {
            Debug.WriteLine("Creating SubmissionEvents table");

            await DatabaseConnection.RunScriptfile(Path.Combine("SetupScripts", "SubmissionEventsTableCreate"));
        }

        public async Task<bool> IsSubmissionEventsCreated()
        {
            using (var connection = DatabaseConnection.Connection())
            {
                const string sql = "SELECT (CASE WHEN OBJECT_ID('[Submissions].[SubmissionEvents]', 'U') IS NULL THEN 0 ELSE 1 END)";
                return await connection.ExecuteScalarAsync<int>(sql) == 1;
            }
        }
    }
}
