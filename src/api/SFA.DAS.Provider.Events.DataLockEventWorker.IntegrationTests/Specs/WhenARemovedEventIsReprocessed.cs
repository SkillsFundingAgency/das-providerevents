//using System.Linq;
//using NUnit.Framework;
//using SFA.DAS.Provider.Events.DataLock.Domain;
//using SFA.DAS.Provider.Events.DataLock.IntegrationTests.Execution;
//using SFA.DAS.Provider.Events.DataLock.IntegrationTests.Helpers;

//namespace SFA.DAS.Provider.Events.DataLock.IntegrationTests.Specs
//{
//    public class WhenARemovedEventIsReprocessed
//    {
//        private const long Ukprn = 10000534;
//        private const string LearnerRefNumber = "Lrn-001";
//        private const string PriceEpisodeIdentifier = "Ep 1";

//        [TestCase(TestFixtureContext.Submission)]
//        [TestCase(TestFixtureContext.PeriodEnd)]
//        public void ThenANewEventShouldNotBeCreated(TestFixtureContext context)
//        {
//            //Arrange
//            var helper = TestDataHelper.Get(context);

//            helper.Clean();

//            helper.SetCurrentPeriodEnd();
//            helper.AddLearningProvider(Ukprn);

//            helper.AddDataLockEvent(Ukprn, LearnerRefNumber, priceEpisodeIdentifier: PriceEpisodeIdentifier, status: EventStatus.New);
//            helper.AddDataLockEvent(Ukprn, LearnerRefNumber, priceEpisodeIdentifier: PriceEpisodeIdentifier, status: EventStatus.Removed);

//            helper.CopyReferenceData();

//            //Act
//            TaskRunner.RunTask(eventsSource:
//                context == TestFixtureContext.PeriodEnd
//                ? EventSource.PeriodEnd
//                : EventSource.Submission);

//            //Assert
//            var actualEvents = helper.GetAllEvents();

//            Assert.IsNull(actualEvents.SingleOrDefault(e => e.PriceEpisodeIdentifier == PriceEpisodeIdentifier));
//        }
//    }
//}