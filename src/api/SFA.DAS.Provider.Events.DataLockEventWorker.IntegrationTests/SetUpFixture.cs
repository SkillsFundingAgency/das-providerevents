using NUnit.Framework;

namespace SFA.DAS.Provider.Events.DataLockEventWorker.AcceptanceTests
{
    [SetUpFixture]
    public class SetUpFixture
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestDataHelperDeds.CreateDatabase();
            //TestDataHelperDataLockEventsDatabase.CreateDatabase();
        }
    }
}
