using System;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.DataLockEventWorker.AcceptanceTests.Specs
{
    [TestFixture]
    public class WhenPerformingInitialRun : DataLockProcessorTestBase
    {
        private const long Ukprn = 10000534;
        private const long CommitmentId = 1;
        private const string LearnerRefNumber = "Lrn-007";

        [Test]
        public void ThenItShouldCopyAllObjectsWithoutCreatingNewOnes()
        {
            //Arrange
            TestDataHelperDeds.AddProvider(Ukprn, DateTime.Today);
            TestDataHelperDeds.AddDataLock(new[] {CommitmentId}, Ukprn, LearnerRefNumber, priceEpisodeIdentifier: "3", errorCodesCsv: "E1");
            TestDataHelperDeds.AddDataLockEvent(Ukprn, DateTime.Today, EventStatus.Removed, 1, "1", 1, DateTime.Today.AddDays(-1), null);
            TestDataHelperDeds.AddDataLockEvent(Ukprn, DateTime.Today, EventStatus.Updated, 2, "2", 2, DateTime.Today.AddDays(-1), new[] {"E1", "E2"});

            //Act
            Act(() => TestDataHelperDataLockEventsDatabase.Count("select count(*) from [DataLockEvents].[ProcessorRun] where [IsInitialRun] = 1 and FinishTimeUtc is not null") > 0, pageSize: 1);

            // Assert
            var providers = TestDataHelperDataLockEventsDatabase.GetAllProviders();
            Assert.IsNotNull(providers);
            Assert.AreEqual(1, providers.Count);
            Assert.AreEqual(Ukprn, providers[0].Ukprn);
            Assert.AreEqual(DateTime.Today, providers[0].IlrSubmissionDateTime);
            Assert.IsNull(providers[0].HandledBy);
            Assert.IsFalse(providers[0].RequiresInitialImport);

            var dataLocks = TestDataHelperDataLockEventsDatabase.GetAllLastDataLocks();
            Assert.AreEqual(1, dataLocks.Count);
            Assert.AreEqual(Ukprn, dataLocks[0].Ukprn);
            Assert.AreEqual(LearnerRefNumber, dataLocks[0].LearnerReferenceNumber);
            Assert.AreEqual("3", dataLocks[0].PriceEpisodeIdentifier);
            Assert.AreEqual("[\"E1\"]", dataLocks[0].ErrorCodes);

            var events = TestDataHelperDataLockEventsDatabase.GetAllEvents();
            Assert.AreEqual(2, events.Count);

            var actualEvent = events[0];
            Assert.AreEqual(Ukprn, actualEvent.Ukprn);
            Assert.AreEqual("1", actualEvent.PriceEpisodeIdentifier);
            Assert.AreEqual("1", actualEvent.LearnRefNumber);
            Assert.AreEqual(1, actualEvent.CommitmentId);
            Assert.AreEqual(false, actualEvent.HasErrors);
            Assert.IsNull(actualEvent.ErrorCodes);
            Assert.AreEqual(EventStatus.Removed, (EventStatus)actualEvent.Status);

            actualEvent = events[1];
            Assert.AreEqual(Ukprn, actualEvent.Ukprn);
            Assert.AreEqual("2", actualEvent.PriceEpisodeIdentifier);
            Assert.AreEqual("2", actualEvent.LearnRefNumber);
            Assert.AreEqual(2, actualEvent.CommitmentId);
            Assert.AreEqual(true, actualEvent.HasErrors);
            Assert.AreEqual("[\"E1\",\"E2\"]", actualEvent.ErrorCodes);
            Assert.AreEqual(EventStatus.Updated, (EventStatus)actualEvent.Status);
        }
    }
}
