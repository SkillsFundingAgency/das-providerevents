using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.IntegrationTestsV2.Setup;
using SFA.DAS.Provider.Events.Api.IntegrationTestsV2.DatabaseAccess;

namespace SFA.DAS.Provider.Events.Api.IntegrationTestsV2
{
    [SetUpFixture]
    public class BeforeAllTests
    {
        [OneTimeSetUp]
        public async Task Setup()
        {
            var create = new CreateDatabase(new DatabaseConnection());
            if (!await create.IsCreated())
            {
                await create.Create();
            }

            var populate = new PopulateTables();

            var setup = new DatabaseSetup(populate);
            await setup.PopulateTestData().ConfigureAwait(false);
        }
    }
}
