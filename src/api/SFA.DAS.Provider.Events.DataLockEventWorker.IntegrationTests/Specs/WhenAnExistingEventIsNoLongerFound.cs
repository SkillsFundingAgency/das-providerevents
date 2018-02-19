using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.DataLockEventWorker.AcceptanceTests.Specs
{
    [TestFixture]
    public class WhenAnExistingEventIsNoLongerFound : DataLockProcessorTestBase
    {
        private const long Ukprn = 10000534;
        private const long CommitmentId = 1;
        private const string LearnerRefNumber = "Lrn-001";
        private const string PriceEpisodeIdentifier = "1-1-1-2017-04-01";

        [Test]
        public void ThenItShouldWriteADeletionEvent()
        {
            // Arrange
            TestDataHelperDeds.AddProvider(Ukprn, DateTime.Today);
            TestDataHelperDataLockEventsDatabase.AddDataLock(new[] {CommitmentId}, Ukprn, LearnerRefNumber, priceEpisodeIdentifier: PriceEpisodeIdentifier, errorCodesCsv: "E1");

            //Act
            Act(() => TestDataHelperDataLockEventsDatabase.Count("select count(*) from [DataLockEvents].[DataLockEvent] where [Status] = @status", new {status = (int) EventStatus.Removed}) > 0);

            // Assert
            var events = TestDataHelperDataLockEventsDatabase.GetAllEvents();
            var actualEvent = events?.SingleOrDefault(e => e.PriceEpisodeIdentifier == PriceEpisodeIdentifier);

            Assert.IsNotNull(actualEvent);
            Assert.AreEqual(EventStatus.Removed, (EventStatus)actualEvent.Status);
            Assert.AreEqual(PriceEpisodeIdentifier, actualEvent.PriceEpisodeIdentifier);
            Assert.AreEqual(LearnerRefNumber, actualEvent.LearnRefNumber);
        }

        [Test]
        public void ThenItShouldNotWriteADeletionEventIfTheLastSeenStatusIsRemoved()
        {
            //Arrange
            TestDataHelperDeds.AddProvider(Ukprn, DateTime.Today);

            TestDataHelperDataLockEventsDatabase.AddProvider(Ukprn, DateTime.Today.AddDays(-1));
            TestDataHelperDataLockEventsDatabase.AddDataLock(new[] {CommitmentId}, Ukprn, LearnerRefNumber, priceEpisodeIdentifier: PriceEpisodeIdentifier, errorCodesCsv: "E1");

            //Act
            Act(() => TestDataHelperDataLockEventsDatabase.Count("select count(*) from [DataLockEvents].[Provider] where Ukprn = @ukprn and IlrSubmissionDateTime = @date", new {ukprn = Ukprn, date = DateTime.Today}) > 0);

            // Assert
            var events = TestDataHelperDataLockEventsDatabase.GetAllEvents();
            var actualEvent = events?.SingleOrDefault(e => e.PriceEpisodeIdentifier == PriceEpisodeIdentifier);

            Assert.IsNotNull(actualEvent);
            Assert.AreEqual(EventStatus.Removed, (EventStatus)actualEvent.Status);
            Assert.AreEqual(PriceEpisodeIdentifier, actualEvent.PriceEpisodeIdentifier);
            Assert.AreEqual(LearnerRefNumber, actualEvent.LearnRefNumber);
        }
    }
}
