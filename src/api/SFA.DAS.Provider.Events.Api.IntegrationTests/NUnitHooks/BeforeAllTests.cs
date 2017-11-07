using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.IntegrationTests.DatabaseAccess;
using SFA.DAS.Provider.Events.Api.IntegrationTests.Setup;

namespace SFA.DAS.Provider.Events.Api.IntegrationTests
{
    [SetUpFixture]
    public class BeforeAllTests
    {
        [OneTimeSetUp]
        public async Task Setup()
        {
            var create = new CreateDatabase(new DatabaseConnection());
            var populate = new PopulateTables(new DatabaseConnection());
            var setup = new DatabaseSetup(create, populate);
            await setup.PopulateTestData().ConfigureAwait(false);
        }
    }
}
