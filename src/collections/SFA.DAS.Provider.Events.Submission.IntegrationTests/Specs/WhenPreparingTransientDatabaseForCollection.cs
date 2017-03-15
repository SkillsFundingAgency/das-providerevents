using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private const int CommitmentId = 741;

        [Test]
        public void ThenItShouldCopyReferenceDataFromDedsToTransient()
        {
            using (var dedsConnection = new SqlConnection(GlobalTestContext.Current.DedsDatabaseConnectionString))
            using (var transConnection = new SqlConnection(GlobalTestContext.Current.TransientDatabaseConnectionString))
            {
                // Arrange
                CleanDeds(dedsConnection);
                CleanTransient(transConnection);

                PopulateLastSeen(dedsConnection);
                PopulateProvider(dedsConnection);
                PopulateFileDetails(dedsConnection);
                PopulateLearningDeliveries(dedsConnection);
                PopulateLearners(dedsConnection);
                PopulatePriceEpisodes(dedsConnection);
                PopulateEmploymentStatus(dedsConnection);
                PopulatePriceEpisodeMatch(dedsConnection);

                // Act
                transConnection.RunDbSetupSqlScriptFile("dml\\01 submissions.populate.reference.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);
                transConnection.RunDbSetupSqlScriptFile("dml\\02 submissions.populate.submissions.sql", GlobalTestContext.Current.DedsDatabaseNameBracketed);

                // Assert
                AssertProviders(transConnection);
                AssertLearningDeliveries(transConnection);
                AssertPriceEpisodes(transConnection);
            }
        }


        private void CleanDeds(SqlConnection dedsConnection)
        {
            dedsConnection.Execute("DELETE FROM Submissions.LastSeenVersion");
            dedsConnection.Execute("DELETE FROM Valid.LearningProvider");
            dedsConnection.Execute("DELETE FROM dbo.FileDetails");
            dedsConnection.Execute("DELETE FROM Valid.LearningDelivery");
            dedsConnection.Execute("DELETE FROM Valid.Learner");
            dedsConnection.Execute("DELETE FROM Rulebase.AEC_ApprenticeshipPriceEpisode");
            dedsConnection.Execute("DELETE FROM Valid.LearnerEmploymentStatus");
            dedsConnection.Execute("DELETE FROM DataLock.PriceEpisodeMatch");
        }
        private void CleanTransient(SqlConnection transConnection)
        {
            transConnection.Execute("DELETE FROM Reference.LearningDeliveries");
            transConnection.Execute("DELETE FROM Reference.PriceEpisodes");
            transConnection.Execute("DELETE FROM Reference.Providers");
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
                IlrFileName = $"ILR-{Ukprn}-{AcademicYear}-{FilePrepDate.AddDays(-1).ToString("yyyyMMdd-HHmmss")}-01",
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
        private void PopulateProvider(SqlConnection dedsConnection)
        {
            var command = "INSERT INTO Valid.LearningProvider (UKPRN) VALUES (@Ukprn)";

            dedsConnection.Execute(command, new { Ukprn });
        }
        private void PopulateFileDetails(SqlConnection dedsConnection)
        {
            var command = "INSERT INTO dbo.FileDetails " +
                          "(UKPRN,FileName,SubmittedTime) " +
                          "VALUES " +
                          "(@Ukprn,@FileName,@SubmittedTime)";

            dedsConnection.Execute(command, new
            {
                Ukprn = Ukprn,
                FileName = $"ILR-{Ukprn}-{AcademicYear}-{FilePrepDate.ToString("yyyyMMdd-HHmmss")}-01",
                SubmittedTime = SubmissionTime
            });
        }
        private void PopulateLearningDeliveries(SqlConnection dedsConnection)
        {
            var command = "INSERT INTO Valid.LearningDelivery " +
                          "(UKPRN,LearnRefNumber,AimSeqNumber,StdCode,LearnStartDate,LearnPlanEndDate," +
                          "LearnAimRef,AimType,FundModel) " +
                          "VALUES " +
                          "(@UKPRN,@LearnRefNumber,@AimSeqNumber,@StdCode,@LearnStartDate,@LearnPlanEndDate," +
                          "'123',1,1)";

            dedsConnection.Execute(command, new
            {
                UKPRN = Ukprn,
                LearnRefNumber = "1",
                AimSeqNumber = 1,
                StdCode = 34,
                LearnStartDate = StartDate,
                LearnPlanEndDate = EndDate
            });
        }
        private void PopulateLearners(SqlConnection dedsConnection)
        {
            var command = "INSERT INTO Valid.Learner " +
                          "(UKPRN,ULN,NINumber,LearnRefNumber,Ethnicity,Sex,LLDDHealthProb) " +
                          "VALUES " +
                          "(@UKPRN,@ULN,@NINumber,'1',1,'U',1)";

            dedsConnection.Execute(command, new { Ukprn, Uln, NiNumber });
        }
        private void PopulatePriceEpisodes(SqlConnection dedsConnection)
        {
            var command = "INSERT INTO Rulebase.AEC_ApprenticeshipPriceEpisode " +
                          "(PriceEpisodeIdentifier,UKPRN,LearnRefNumber,PriceEpisodeAimSeqNumber,EpisodeEffectiveTNPStartDate,TNP1,TNP2,TNP3,TNP4) " +
                          "VALUES " +
                          "(@PriceEpisodeIdentifier,@UKPRN,@LearnRefNumber,@PriceEpisodeAimSeqNumber,@EpisodeEffectiveTNPStartDate,@TNP1,@TNP2,0,0)";

            dedsConnection.Execute(command, new
            {
                PriceEpisodeIdentifier = $"00-34-01/{StartDate.ToString("MM/yyyy")}",
                Ukprn = Ukprn,
                LearnRefNumber = "1",
                PriceEpisodeAimSeqNumber = 1,
                EpisodeEffectiveTNPStartDate = StartDate,
                TNP1 = OnProgPrice,
                TNP2 = EndpointPrice
            });
        }
        private void PopulateEmploymentStatus(SqlConnection dedsConnection)
        {
            var command = "INSERT INTO Valid.LearnerEmploymentStatus " +
                          "(UKPRN,LearnRefNumber,DateEmpStatApp) " +
                          "VALUES " +
                          "(@UKPRN,@LearnRefNumber,@DateEmpStatApp)";

            dedsConnection.Execute(command, new
            {
                UKPRN = Ukprn,
                LearnRefNumber = "1",
                DateEmpStatApp = StartDate
            });
        }
        private void PopulatePriceEpisodeMatch(SqlConnection dedsConnection)
        {
            var command = "INSERT INTO DataLock.PriceEpisodeMatch " +
                          "(Ukprn,PriceEpisodeIdentifier,LearnRefNumber,AimSeqNumber,CommitmentId,CollectionPeriodName,CollectionPeriodMonth,CollectionPeriodYear) " +
                          "VALUES" +
                          "(@Ukprn,@PriceEpisodeIdentifier,@LearnRefNumber,@AimSeqNumber,@CommitmentId,@CollectionPeriodName,@CollectionPeriodMonth,@CollectionPeriodYear)";

            dedsConnection.Execute(command, new
            {
                Ukprn = Ukprn,
                PriceEpisodeIdentifier = $"00-34-01/{StartDate.ToString("MM/yyyy")}",
                LearnRefNumber = "1",
                AimSeqNumber = 1,
                CommitmentId = CommitmentId,
                CollectionPeriodName = "R02",
                CollectionPeriodMonth = 9,
                CollectionPeriodYear = 2017
            });
        }

        private void AssertProviders(SqlConnection transConnection)
        {
            var providers = transConnection.Query<ProviderEntity>("SELECT Ukprn, IlrFilename [FileName], SubmittedTime FROM Reference.Providers").ToArray();

            Assert.AreEqual(1, providers.Length);
            Assert.AreEqual(Ukprn, providers[0].Ukprn);
            Assert.AreEqual($"ILR-{Ukprn}-{AcademicYear}-{FilePrepDate.ToString("yyyyMMdd-HHmmss")}-01", providers[0].FileName);
            Assert.AreEqual(SubmissionTime, providers[0].SubmittedTime);
        }
        private void AssertLearningDeliveries(SqlConnection transConnection)
        {
            var learningDeliveries = transConnection.Query<LearningDeliveryEntity>("SELECT * FROM Reference.LearningDeliveries").ToArray();

            Assert.AreEqual(1, learningDeliveries.Length);
            Assert.AreEqual(Ukprn, learningDeliveries[0].Ukprn);
            Assert.AreEqual("1", learningDeliveries[0].LearnRefNumber);
            Assert.AreEqual(1, learningDeliveries[0].AimSeqNumber);
            Assert.AreEqual(Uln, learningDeliveries[0].Uln);
            Assert.AreEqual(NiNumber, learningDeliveries[0].NiNumber);
            Assert.AreEqual(34, learningDeliveries[0].StdCode);
            Assert.AreEqual(StartDate, learningDeliveries[0].LearnStartDate);
            Assert.AreEqual(EndDate, learningDeliveries[0].LearnPlanEndDate);
            Assert.IsNull(learningDeliveries[0].LearnActEndDate);
        }
        private void AssertPriceEpisodes(SqlConnection transConnection)
        {
            var priceEpisodes = transConnection.Query<PriceEpisodeEntity>("SELECT * FROM Reference.PriceEpisodes").ToArray();

            Assert.AreEqual(1, priceEpisodes.Length);
            Assert.AreEqual($"00-34-01/{StartDate.ToString("MM/yyyy")}", priceEpisodes[0].PriceEpisodeIdentifier);
            Assert.AreEqual(Ukprn, priceEpisodes[0].Ukprn);
            Assert.AreEqual("1", priceEpisodes[0].LearnRefNumber);
            Assert.AreEqual(1, priceEpisodes[0].PriceEpisodeAimSeqNumber);
            Assert.AreEqual(StartDate, priceEpisodes[0].EpisodeEffectiveTnpStartDate);
            Assert.AreEqual(OnProgPrice, priceEpisodes[0].Tnp1);
            Assert.AreEqual(EndpointPrice, priceEpisodes[0].Tnp2);
            Assert.AreEqual(0, priceEpisodes[0].Tnp3);
            Assert.AreEqual(0, priceEpisodes[0].Tnp4);
            Assert.AreEqual(CommitmentId, priceEpisodes[0].CommitmentId);
        }
    }
}
