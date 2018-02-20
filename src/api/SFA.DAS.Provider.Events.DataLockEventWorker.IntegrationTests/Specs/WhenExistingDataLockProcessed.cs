using System;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.DataLockEventWorker.AcceptanceTests.Specs
{
    public class WhenExistingDataLockProcessed : DataLockProcessorTestBase
    {
        private const long Ukprn = 10000534;
        private const long CommitmentId = 1;
        private const string LearnerRefNumber = "Lrn-007";
        private const string PriceEpisodeIdentifier = "1-1-1-2017-04-07";

        [TestCase(true)]
        [TestCase(false)]
        public void ThenNoNewEventsShouldBeWrittenIfNothingChanged(bool passedDataLock)
        {
            //Arrange
            TestDataHelperDeds.AddProvider(Ukprn, DateTime.Today);
            TestDataHelperDeds.AddDataLock(new[] {CommitmentId}, Ukprn, LearnerRefNumber, priceEpisodeIdentifier: PriceEpisodeIdentifier, errorCodesCsv: passedDataLock ? null : "E1");

            TestDataHelperDataLockEventsDatabase.AddProvider(Ukprn, DateTime.Today.AddDays(-1));
            TestDataHelperDataLockEventsDatabase.AddDataLock(new[] {CommitmentId}, Ukprn, LearnerRefNumber, priceEpisodeIdentifier: PriceEpisodeIdentifier, errorCodesCsv: passedDataLock ? null : "E1");

            //Act
            Act(() => TestDataHelperDataLockEventsDatabase.Count("select count(*) from [DataLockEvents].[Provider] where Ukprn = @ukprn and IlrSubmissionDateTime = @date", new {ukprn = Ukprn, date = DateTime.Today}) > 0);

            // Assert
            var events = TestDataHelperDataLockEventsDatabase.GetAllEvents();
            var actualEvent = events?.SingleOrDefault(e => e.PriceEpisodeIdentifier == PriceEpisodeIdentifier);

            Assert.IsNull(actualEvent);
        }

        [Test]
        public void ThenANewEventShouldBeWrittenIfSomethingChanged()
        {
            //Arrange
            TestDataHelperDeds.AddProvider(Ukprn, DateTime.Today);
            TestDataHelperDeds.AddDataLock(new[] {CommitmentId}, Ukprn, LearnerRefNumber, priceEpisodeIdentifier: PriceEpisodeIdentifier);

            TestDataHelperDataLockEventsDatabase.AddProvider(Ukprn, DateTime.Today.AddDays(-1));
            TestDataHelperDataLockEventsDatabase.AddDataLock(new[] {CommitmentId}, Ukprn, LearnerRefNumber, priceEpisodeIdentifier: PriceEpisodeIdentifier, errorCodesCsv: "E1");

            //Act
            Act(() => TestDataHelperDataLockEventsDatabase.Count("select count(*) from [DataLockEvents].[Provider] where Ukprn = @ukprn and IlrSubmissionDateTime = @date", new {ukprn = Ukprn, date = DateTime.Today}) > 0);

            // Assert
            var events = TestDataHelperDataLockEventsDatabase.GetAllEvents();
            var actualEvent = events?.SingleOrDefault(e => e.PriceEpisodeIdentifier == PriceEpisodeIdentifier);

            Assert.IsNotNull(actualEvent);
            Assert.AreEqual(Ukprn, actualEvent.Ukprn);
            Assert.AreEqual(PriceEpisodeIdentifier, actualEvent.PriceEpisodeIdentifier);
            Assert.AreEqual(LearnerRefNumber, actualEvent.LearnRefNumber);
            Assert.AreEqual(CommitmentId, actualEvent.CommitmentId);
            Assert.AreEqual(false, actualEvent.HasErrors);
            Assert.AreEqual(null, actualEvent.ErrorCodes);
            Assert.AreEqual(EventStatus.Updated, (EventStatus)actualEvent.Status);
        }

        
        [Test]
        public void ThenANewEventShouldBeEmittedIfItIsNoLongerRemoved()
        {
            //Arrange
            TestDataHelperDeds.AddProvider(Ukprn, DateTime.Today);
            TestDataHelperDeds.AddDataLock(new[] {CommitmentId}, Ukprn, LearnerRefNumber, priceEpisodeIdentifier: PriceEpisodeIdentifier, errorCodesCsv: "E1");

            TestDataHelperDataLockEventsDatabase.AddProvider(Ukprn, DateTime.Today.AddDays(-1));
            TestDataHelperDataLockEventsDatabase.AddDataLock(new[] {CommitmentId}, Ukprn, LearnerRefNumber, priceEpisodeIdentifier: PriceEpisodeIdentifier, errorCodesCsv: "E1", deletedTime: DateTime.UtcNow.AddDays(-1));

            //Act
            Act(() => TestDataHelperDataLockEventsDatabase.Count("select count(*) from [DataLockEvents].[Provider] where Ukprn = @ukprn and IlrSubmissionDateTime = @date", new {ukprn = Ukprn, date = DateTime.Today}) > 0);

            // Assert
            var events = TestDataHelperDataLockEventsDatabase.GetAllEvents();
            var actualEvent = events?.SingleOrDefault(e => e.PriceEpisodeIdentifier == PriceEpisodeIdentifier);

            Assert.IsNotNull(actualEvent);
            Assert.AreEqual(Ukprn, actualEvent.Ukprn);
            Assert.AreEqual(PriceEpisodeIdentifier, actualEvent.PriceEpisodeIdentifier);
            Assert.AreEqual(LearnerRefNumber, actualEvent.LearnRefNumber);
            Assert.AreEqual(CommitmentId, actualEvent.CommitmentId);
            Assert.AreEqual(true, actualEvent.HasErrors);
            Assert.AreEqual("[\"E1\"]", actualEvent.ErrorCodes);
            Assert.AreEqual(EventStatus.New, (EventStatus)actualEvent.Status);
        }
    }
}