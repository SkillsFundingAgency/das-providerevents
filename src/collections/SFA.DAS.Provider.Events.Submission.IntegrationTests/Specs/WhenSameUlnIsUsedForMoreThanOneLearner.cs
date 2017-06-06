using Dapper;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Submission.Domain;
using SFA.DAS.Provider.Events.Submission.IntegrationTests.Data;
using SFA.DAS.Provider.Events.Submission.IntegrationTests.Execution;
using SFA.DAS.Provider.Events.Submission.IntegrationTests.Helpers;
using SFA.DAS.Provider.Events.Submission.IntegrationTests.TestContext;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Provider.Events.Submission.IntegrationTests.Specs
{
    public class WhenSameUlnIsUsedForMoreThanOneLearner
    {

        private const long Ukprn = 123456;
        private const long Uln = 98563;
        private const string NiNumber = "AB123456A";
        private const string AcademicYear = "1718";
        private readonly DateTime FilePrepDate = new DateTime(2017, 9, 2, 12, 37, 26);
        private readonly DateTime SubmissionTime = new DateTime(2017, 9, 2, 12, 54, 56);
        private readonly DateTime StartDate = new DateTime(2017, 9, 1);
        private readonly DateTime EndDate = new DateTime(2019, 9, 15);
        private const decimal OnProgPrice = 12000m;
        private const decimal EndpointPrice = 3000m;

        [Test]
        public void ThenItShouldNotThrowException()
        {
            var testDataSet = TestDataSet.GetFirstSubmissionDataset();
            testDataSet.Clean();
            testDataSet.Store();

            using (var transientConnection = new SqlConnection(GlobalTestContext.Current.TransientDatabaseConnectionString))
            {
                // Arrange
                var ilrDetails = new IlrDetails {

                    IlrFileName = $"ILR-{Ukprn}-{AcademicYear}-{FilePrepDate.AddDays(-1).ToString("yyyyMMdd-HHmmss")}-01.xml",
                    FileDateTime = FilePrepDate.AddDays(-1),
                    SubmittedDateTime = SubmissionTime.AddDays(-1),
                    ComponentVersionNumber = 1,
                    Ukprn = testDataSet.Learners[0].Ukprn,
                    Uln = testDataSet.Learners[0].Uln,
                    LearnRefNumber = "1",
                    AimSeqNumber = 1,
                    PriceEpisodeIdentifier = testDataSet.PriceEpisodes[0].PriceEpisodeIdentifier,
                    StandardCode = 34,
                    ActualStartDate = StartDate,
                    PlannedEndDate = EndDate,
                    OnProgrammeTotalPrice = OnProgPrice * 0.8m,
                    CompletionTotalPrice = EndpointPrice * 0.8m,
                    AcademicYear = AcademicYear
                };
                TestDataHelper.PopulateLastSeen(transientConnection, ilrDetails);

                ilrDetails.LearnRefNumber = "2";
                TestDataHelper.PopulateLastSeen(transientConnection, ilrDetails);

            }

            // Act
            TaskRunner.RunTask();
            
        }

      
    }
}
