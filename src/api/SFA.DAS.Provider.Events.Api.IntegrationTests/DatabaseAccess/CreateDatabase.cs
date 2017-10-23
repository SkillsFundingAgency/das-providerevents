using System.IO;
using System.Threading.Tasks;

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
            await _connection.RunScriptfile(Path.Combine("SetupScripts", "TableSetup"))
                .ConfigureAwait(false);
        }
    }
}
