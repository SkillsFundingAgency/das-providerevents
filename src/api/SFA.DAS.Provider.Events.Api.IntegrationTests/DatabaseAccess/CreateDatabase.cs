using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Dapper;

namespace SFA.DAS.Provider.Events.Api.IntegrationTests.DatabaseAccess
{
    class CreateDatabase
    {
        private readonly DatabaseConnection _connection;

        public CreateDatabase(DatabaseConnection connection)
        {
            _connection = connection;
        }

        public async Task Create()
        {
            Debug.WriteLine("Creating tables");

            await _connection.RunScriptfile(Path.Combine("SetupScripts", "TableSetup"))
                .ConfigureAwait(false);
        }

        public async Task<bool> IsCreated()
        {
            using (var connection = DatabaseConnection.Connection())
            {
                // doesn't work so well when initialisation script fails mid way through
                // wrap creation script in transaction?
                const string sql = "SELECT (CASE WHEN OBJECT_ID('[Payments].[FundingAccountId]', 'U') IS NULL THEN 0 ELSE 1 END)";
                return await connection.ExecuteScalarAsync<int>(sql).ConfigureAwait(false) == 1;
            }
        }
        public async Task CreateSubmissionEvents()
        {
            Debug.WriteLine("Creating SubmissionEvents table");

            await _connection.RunScriptfile(Path.Combine("SetupScripts", "SubmissionEventsTableCreate"));
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
