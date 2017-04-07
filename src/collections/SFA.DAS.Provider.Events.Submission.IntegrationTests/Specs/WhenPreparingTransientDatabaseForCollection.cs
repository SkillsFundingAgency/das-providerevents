using System;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Submission.IntegrationTests.Data.Entities;
using SFA.DAS.Provider.Events.Submission.IntegrationTests.TestContext;

namespace SFA.DAS.Provider.Events.Submission.IntegrationTests.Specs
{
    public class WhenPreparingTransientDatabaseForCollection
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
        public void ThenItShouldCopyReferenceDataFromDedsToTransient()
        {
            using (var dedsConnection = new SqlConnection(GlobalTestContext.Current.DedsDatabaseConnectionString))
            using (var transConnection = new SqlConnection(GlobalTestContext.Current.TransientDatabaseConnectionString))
            {
                // Arrange
                CleanDeds(dedsConnection);
                CleanTransient(transConnection);

                PopulateProvider(transConnection);
                PopulateLastSeen(dedsConnection);
                
                // Act
                transConnection.RunDbSetupSqlScriptFile("dml\\01 submissions.populate.submissions.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);

                // Assert
                AssertLastSeen(transConnection);
            }
        }


        private void CleanDeds(SqlConnection dedsConnection)
        {
            dedsConnection.Execute("DELETE FROM Submissions.LastSeenVersion");
        }
        private void CleanTransient(SqlConnection transConnection)
        {
            transConnection.Execute("DELETE FROM Submissions.LastSeenVersion");
        }

        private void PopulateProvider(SqlConnection transConnection)
        {
            var command = "INSERT INTO Valid.LearningProvider VALUES (@Ukprn)";

            transConnection.Execute(command, new { Ukprn });
        }
        private void PopulateLastSeen(SqlConnection dedsConnection)
        {
            var command = "INSERT INTO Submissions.LastSeenVersion " +
                      "(IlrFileName,FileDateTime,SubmittedDateTime,ComponentVersionNumber,UKPRN,ULN,LearnRefNumber,AimSeqNumber,PriceEpisodeIdentifier," +
                      "StandardCode,ActualStartDate,PlannedEndDate,OnProgrammeTotalPrice,CompletionTotalPrice,AcademicYear) " +
                      "VALUES " +
                      "(@IlrFileName,@FileDateTime,@SubmittedDateTime,@ComponentVersionNumber,@UKPRN,@ULN,@LearnRefNumber,@AimSeqNumber,@PriceEpisodeIdentifier," +
                      "@StandardCode,@ActualStartDate,@PlannedEndDate,@OnProgrammeTotalPrice,@CompletionTotalPrice,@AcademicYear)";

            dedsConnection.Execute(command, new
            {
                IlrFileName = $"ILR-{Ukprn}-{AcademicYear}-{FilePrepDate.AddDays(-1).ToString("yyyyMMdd-HHmmss")}-01.xml",
                FileDateTime = FilePrepDate.AddDays(-1),
                SubmittedDateTime = SubmissionTime.AddDays(-1),
                ComponentVersionNumber = 1,
                UKPRN = Ukprn,
                ULN = Uln,
                LearnRefNumber = "1",
                AimSeqNumber = 1,
                PriceEpisodeIdentifier = $"00-34-01/{StartDate.ToString("MM/yyyy")}",
                StandardCode = 34,
                ActualStartDate = StartDate,
                PlannedEndDate = EndDate,
                OnProgrammeTotalPrice = OnProgPrice * 0.8m,
                CompletionTotalPrice = EndpointPrice * 0.8m,
                AcademicYear = AcademicYear
            });
        }

        
        private void AssertLastSeen(SqlConnection transConnection)
        {
            var lastSeen = transConnection.Query<LastSeenVersionEntity>("SELECT * FROM Submissions.LastSeenVersion").ToArray();

            Assert.AreEqual(1, lastSeen.Length);

            Assert.AreEqual($"ILR-{Ukprn}-{AcademicYear}-{FilePrepDate.AddDays(-1).ToString("yyyyMMdd-HHmmss")}-01.xml", lastSeen[0].IlrFileName);
            Assert.AreEqual(FilePrepDate.AddDays(-1), lastSeen[0].FileDateTime);
            Assert.AreEqual(SubmissionTime.AddDays(-1), lastSeen[0].SubmittedDateTime);
            Assert.AreEqual(1, lastSeen[0].ComponentVersionNumber);
            Assert.AreEqual(Ukprn, lastSeen[0].Ukprn);
            Assert.AreEqual(Uln, lastSeen[0].Uln);
            Assert.AreEqual("1", lastSeen[0].LearnRefNumber);
            Assert.AreEqual(1, lastSeen[0].AimSeqNumber);
            Assert.AreEqual($"00-34-01/{StartDate.ToString("MM/yyyy")}", lastSeen[0].PriceEpisodeIdentifier);
            Assert.AreEqual(34, lastSeen[0].StandardCode);
            Assert.AreEqual(StartDate, lastSeen[0].ActualStartDate);
            Assert.AreEqual(EndDate, lastSeen[0].PlannedEndDate);
            Assert.AreEqual(OnProgPrice * 0.8m, lastSeen[0].OnProgrammeTotalPrice);
            Assert.AreEqual(EndpointPrice * 0.8m, lastSeen[0].CompletionTotalPrice);
            Assert.AreEqual(AcademicYear, lastSeen[0].AcademicYear);
        }
    }
}
