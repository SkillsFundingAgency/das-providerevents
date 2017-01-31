using System;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Submission.IntegrationTests.Data;
using SFA.DAS.Provider.Events.Submission.IntegrationTests.Execution;

namespace SFA.DAS.Provider.Events.Submission.IntegrationTests.Specs
{
    public class WhenANewIlrFileIsSubmitted
    {
        [Test]
        public void ThenItShouldWriteANewSubmissionEventWithAllPropertiesPopulated()
        {
            // Arrange
            var testDataSet = TestDataSet.GetFirstSubmissionDataset();
            testDataSet.Store();

            // Act
            TaskRunner.RunTask();

            // Assert
            var events = SubmissionEventRepository.GetSubmissionEventsForProvider(testDataSet.LearningDeliveries[0].Ukprn);
            Assert.IsNotNull(events);
            Assert.AreEqual(1, events.Length);

            var newEvent = events[0];
            Assert.AreEqual(testDataSet.Providers[0].FileName, newEvent.IlrFileName);
            Assert.IsTrue(AreDateTimesLessThanASecondDifferent(testDataSet.Providers[0].SubmittedTime, newEvent.SubmittedDateTime));
            Assert.AreEqual(SubmissionEventsTask.ComponentVersion, newEvent.ComponentVersionNumber);
            Assert.AreEqual(testDataSet.Providers[0].Ukprn, newEvent.Ukprn);
            Assert.AreEqual(testDataSet.LearningDeliveries[0].Uln, newEvent.Uln);
            Assert.AreEqual(testDataSet.LearningDeliveries[0].LearnRefNumber, newEvent.LearnRefNumber);
            Assert.AreEqual(testDataSet.LearningDeliveries[0].AimSeqNumber, newEvent.AimSeqNumber);
            Assert.AreEqual(testDataSet.PriceEpisodes[0].PriceEpisodeIdentifier, newEvent.PriceEpisodeIdentifier);
            Assert.AreEqual(testDataSet.LearningDeliveries[0].StdCode, newEvent.StandardCode);
            Assert.AreEqual(testDataSet.LearningDeliveries[0].ProgType, newEvent.ProgrammeType);
            Assert.AreEqual(testDataSet.LearningDeliveries[0].FworkCode, newEvent.FrameworkCode);
            Assert.AreEqual(testDataSet.LearningDeliveries[0].PwayCode, newEvent.PathwayCode);
            Assert.AreEqual(testDataSet.LearningDeliveries[0].LearnStartDate, newEvent.ActualStartDate);
            Assert.AreEqual(testDataSet.LearningDeliveries[0].LearnPlanEndDate, newEvent.PlannedEndDate);
            Assert.AreEqual(testDataSet.LearningDeliveries[0].LearnActEndDate, newEvent.ActualEndDate);
            Assert.AreEqual(testDataSet.PriceEpisodes[0].Tnp1, newEvent.OnProgrammeTotalPrice);
            Assert.AreEqual(testDataSet.PriceEpisodes[0].Tnp2, newEvent.CompletionTotalPrice);
            Assert.AreEqual(testDataSet.LearningDeliveries[0].NiNumber, newEvent.NiNumber);
        }


        private bool AreDateTimesLessThanASecondDifferent(DateTime expected, DateTime actual)
        {
            return Math.Abs((expected - actual).TotalSeconds) < 1;
        }
    }
}
