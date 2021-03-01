using System;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.IntegrationTestsV2.DatabaseAccess;

namespace SFA.DAS.Provider.Events.Api.IntegrationTestsV2
{
    [SetUpFixture]
    public class BeforeAllTests
    {
        [OneTimeSetUp]
        public async Task Setup()
        {
            TestData.EmployerAccountId = new Random().Next(100000000, 999999999);

            TestData.CollectionPeriod = (byte)new Random().Next(1, 14);

            var years = new short[] { 1617, 1718, 1819, 1920, 2021 };
            var randomYear = new Random().Next(0, 4);
            TestData.AcademicYear = years[randomYear];

            await new DatabaseSetup().PopulateAllData().ConfigureAwait(false);
        }
    }
}
