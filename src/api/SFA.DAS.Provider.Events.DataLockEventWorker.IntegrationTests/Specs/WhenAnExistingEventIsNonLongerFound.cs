//using System.Linq;
//using NUnit.Framework;
//using SFA.DAS.Provider.Events.DataLock.Domain;
//using SFA.DAS.Provider.Events.DataLock.IntegrationTests.Execution;
//using SFA.DAS.Provider.Events.DataLock.IntegrationTests.Helpers;

//namespace SFA.DAS.Provider.Events.DataLock.IntegrationTests.Specs
//{
//    public class WhenAnExistingEventIsNonLongerFound
//    {
//        private ITestDataHelper _helper;

//        private const long Ukprn = 10000534;
//        private const int CommitmentId = 1;
//        private const string LearnerRefNumber = "Lrn-001";
//        private const string PriceEpisodeIdentifier = "1-1-1-2017-04-01";

        
//        private void Setup(TestFixtureContext context)
//        {
//            _helper = TestDataHelper.Get(context);

//            _helper.Clean();

//            _helper.SetCurrentPeriodEnd();
//            _helper.AddLearningProvider(Ukprn);
//            _helper.AddFileDetails(Ukprn);
//            _helper.AddCommitment(CommitmentId, Ukprn, LearnerRefNumber, passedDataLock: false);
//            _helper.AddIlrDataForCommitment(CommitmentId, LearnerRefNumber);

//            _helper.AddDataLockEvent(Ukprn, LearnerRefNumber, passedDataLock: false, priceEpisodeIdentifier: PriceEpisodeIdentifier);

//            _helper.CopyReferenceData();
//        }

//        [TestCase(TestFixtureContext.Submission)]
//        [TestCase(TestFixtureContext.PeriodEnd)]
//        public void ThenItShouldWriteADeletionEvent(TestFixtureContext context)
//        {
//            // Arrange
//            Setup(context);

//            //Act
//            TaskRunner.RunTask(eventsSource:
//                context == TestFixtureContext.PeriodEnd
//                ? EventSource.PeriodEnd
//                : EventSource.Submission);

//            // Assert
//            var events = _helper.GetAllEvents();
//            var actualEvent = events?.SingleOrDefault(e => e.PriceEpisodeIdentifier == PriceEpisodeIdentifier);

//            Assert.IsNotNull(actualEvent);
//            Assert.AreEqual(EventStatus.Removed, actualEvent.Status);
//            Assert.AreEqual(PriceEpisodeIdentifier, actualEvent.PriceEpisodeIdentifier);
//            Assert.AreEqual(LearnerRefNumber, actualEvent.LearnRefnumber);
//        }

//        [TestCase(TestFixtureContext.Submission)]
//        [TestCase(TestFixtureContext.PeriodEnd)]
//        public void ThenItShouldNotWriteADeletionEventIfTheLastSeenStatusIsRemoved(TestFixtureContext context)
//        {
//            //Arrange
//            Setup(context);

//            //Act
//            TaskRunner.RunTask(eventsSource:
//                context == TestFixtureContext.PeriodEnd
//                ? EventSource.PeriodEnd
//                : EventSource.Submission);

//            // Assert
//            var events = _helper.GetAllEvents();
//            var actualEvent = events?.SingleOrDefault(e => e.PriceEpisodeIdentifier == PriceEpisodeIdentifier);

//            Assert.IsNotNull(actualEvent);
//            Assert.AreEqual(EventStatus.Removed, actualEvent.Status);
//            Assert.AreEqual(PriceEpisodeIdentifier, actualEvent.PriceEpisodeIdentifier);
//            Assert.AreEqual(LearnerRefNumber, actualEvent.LearnRefnumber);
//        }
//    }
//}
