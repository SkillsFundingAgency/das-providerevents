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

            // as we've modified the SubmissionsEvents table, we need to enable the
            // seamless upgrade of the table on the first run after fetching the new code.
            // previously, the tests didn't populate the table, so we check for that.
            // this way we handle (a) first run with empty database,
            // (b) first run after table modification & (c) subsequent runs
            if (!await create.IsSubmissionEventsCreated()
                || !await PopulateTables.IsSubmissionEventsTablePopulated())
            {
                await create.CreateSubmissionEvents();
            }

            var setup = new DatabaseSetup();
            await setup.PopulateTestData().ConfigureAwait(false);
        }
    }
}
