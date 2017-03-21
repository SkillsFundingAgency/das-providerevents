using System;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Submission.IntegrationTests.Data;
using SFA.DAS.Provider.Events.Submission.IntegrationTests.Execution;

namespace SFA.DAS.Provider.Events.Submission.IntegrationTests.Specs
{
    public class WhenAnUpdatedIlrFileIsSubmitted
    {
        [Test]
        public void ThenItShouldWriteANewSubmissionEventWithOnlyUpdatedPropertiesPopulated()
        {
            // Arrange
            var testDataSet = TestDataSet.GetUpdatedSubmissionDataSet();
            testDataSet.Clean();
            testDataSet.Store();

            // Act
            TaskRunner.RunTask();

            // Assert
            var events = SubmissionEventRepository.GetSubmissionEventsForProvider(testDataSet.LearningDeliveries[0].Ukprn);
            Assert.IsNotNull(events);
            Assert.AreEqual(1, events.Length);

            var newEvent = events[0];
            Assert.AreEqual(testDataSet.FileDetails[0].FileName, newEvent.IlrFileName);
            Assert.IsTrue(AreDateTimesLessThanASecondDifferent(testDataSet.FileDetails[0].SubmittedTime, newEvent.SubmittedDateTime));
            Assert.AreEqual(SubmissionEventsTask.ComponentVersion, newEvent.ComponentVersionNumber);
            Assert.AreEqual(testDataSet.Providers[0].Ukprn, newEvent.Ukprn);
            Assert.AreEqual(testDataSet.Learners[0].Uln, newEvent.Uln);
            Assert.AreEqual(testDataSet.LearningDeliveries[0].LearnRefNumber, newEvent.LearnRefNumber);
            Assert.AreEqual(testDataSet.LearningDeliveries[0].AimSeqNumber, newEvent.AimSeqNumber);
            Assert.AreEqual(testDataSet.PriceEpisodes[0].PriceEpisodeIdentifier, newEvent.PriceEpisodeIdentifier);
            Assert.AreEqual(testDataSet.LearningDeliveries[0].StdCode, newEvent.StandardCode);
            Assert.IsNull(newEvent.ProgrammeType);
            Assert.IsNull(newEvent.FrameworkCode);
            Assert.IsNull(newEvent.PathwayCode);
            Assert.IsNull(newEvent.ActualStartDate);
            Assert.IsNull(newEvent.PlannedEndDate);
            Assert.IsNull(newEvent.ActualEndDate);
            Assert.AreEqual(testDataSet.PriceEpisodes[0].Tnp1, newEvent.OnProgrammeTotalPrice);
            Assert.IsNull(newEvent.CompletionTotalPrice);
            Assert.IsNull(newEvent.NiNumber);
        }

        [Test]
        public void ThenItShouldWriteIlrDetailsToLastSeenVersion()
        {
            // Arrange
            var testDataSet = TestDataSet.GetUpdatedSubmissionDataSet();
            testDataSet.Clean();
            testDataSet.Store();

            // Act
            TaskRunner.RunTask();

            // Assert
            var lastSeenVersions = LastSeenVersionRepository.GetLastestVersionsForProvider(testDataSet.Providers[0].Ukprn);
            Assert.IsNotNull(lastSeenVersions);
            Assert.AreEqual(1, lastSeenVersions.Length);

            var version = lastSeenVersions[0];
            Assert.AreEqual(testDataSet.FileDetails[0].FileName, version.IlrFileName);
            Assert.IsTrue(AreDateTimesLessThanASecondDifferent(testDataSet.FileDetails[0].SubmittedTime, version.SubmittedDateTime));
            Assert.AreEqual(testDataSet.Providers[0].Ukprn, version.Ukprn);
            Assert.AreEqual(testDataSet.Learners[0].Uln, version.Uln);
            Assert.AreEqual(testDataSet.LearningDeliveries[0].LearnRefNumber, version.LearnRefNumber);
            Assert.AreEqual(testDataSet.LearningDeliveries[0].AimSeqNumber, version.AimSeqNumber);
            Assert.AreEqual(testDataSet.PriceEpisodes[0].PriceEpisodeIdentifier, version.PriceEpisodeIdentifier);
            Assert.AreEqual(testDataSet.LearningDeliveries[0].StdCode, version.StandardCode);
            Assert.AreEqual(testDataSet.LearningDeliveries[0].ProgType, version.ProgrammeType);
            Assert.AreEqual(testDataSet.LearningDeliveries[0].FworkCode, version.FrameworkCode);
            Assert.AreEqual(testDataSet.LearningDeliveries[0].PwayCode, version.PathwayCode);
            Assert.AreEqual(testDataSet.LearningDeliveries[0].LearnStartDate, version.ActualStartDate);
            Assert.AreEqual(testDataSet.LearningDeliveries[0].LearnPlanEndDate, version.PlannedEndDate);
            Assert.AreEqual(testDataSet.LearningDeliveries[0].LearnActEndDate, version.ActualEndDate);
            Assert.AreEqual(testDataSet.PriceEpisodes[0].Tnp1, version.OnProgrammeTotalPrice);
            Assert.AreEqual(testDataSet.PriceEpisodes[0].Tnp2, version.CompletionTotalPrice);
            Assert.AreEqual(testDataSet.Learners[0].NiNumber, version.NiNumber);
        }


        private bool AreDateTimesLessThanASecondDifferent(DateTime expected, DateTime actual)
        {
            return Math.Abs((expected - actual).TotalSeconds) < 1;
        }
    }
}
