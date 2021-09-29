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

            TestData.CollectionPeriod = AcademicYearHelper.GetRandomCollectionPeriod();

            TestData.AcademicYear = AcademicYearHelper.GetRandomAcademicYear();

            await new DatabaseSetup().PopulateAllData().ConfigureAwait(false);
        }
    }
}
