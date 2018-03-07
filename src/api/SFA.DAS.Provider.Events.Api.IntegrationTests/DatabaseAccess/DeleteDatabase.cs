using System.IO;
using System.Threading.Tasks;

namespace SFA.DAS.Provider.Events.Api.IntegrationTests.DatabaseAccess
{
    class DeleteDatabase
    {
        private readonly DatabaseConnection _connection;

        public DeleteDatabase(DatabaseConnection connection)
        {
            _connection = connection;
        }

        public async Task Delete()
        {
            await _connection.RunScriptfile(Path.Combine("SetupScripts", "TableTeardown"))
                .ConfigureAwait(false);
        }
    }
}
