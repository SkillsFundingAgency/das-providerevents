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
            
            var year = (short) new Random().Next(2016, 2021);
            
            var yearPart1 = year % 100;
            var yearPart2 = year / 100;

            //2020
            if(yearPart1 == yearPart2)
            {
                TestData.AcademicYear = 2021;
            }      //2021
            else if (yearPart1 > 20)
            {
                TestData.AcademicYear = year;
            }//<=2019
            else
            {
                TestData.AcademicYear = (short)(((yearPart2 - (yearPart2 - yearPart1 + 1)) * 100) + yearPart1) ;
            }

            
            await new DatabaseSetup().PopulateAllData().ConfigureAwait(false);
        }
    }
}
