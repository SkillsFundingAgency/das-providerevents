using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.DataLockEventWorker.AcceptanceTests.Specs
{
    [TestFixture]
    public class WhenANewDataLockIsGenerated : DataLockProcessorTestBase
    {

        [Test]
        public void ThenItShouldBeWrittenToTheDatabase()
        {
            // Arrange
            var ukprn = 10000534;
            var commitmentId = 1;

            TestDataHelperDeds.AddProvider(ukprn, DateTime.Today);
            TestDataHelperDeds.AddDataLock(new long[]{commitmentId}, ukprn, "Lrn-001", errorCodesCsv: "E1");

            Act(() => TestDataHelperDataLockEventsDatabase.Count("select count(*) from [DataLockEvents].[Provider] where Ukprn = @ukprn and IlrSubmissionDateTime = @date", new {ukprn, date = DateTime.Today}) > 0);

            // Assert
            var events = TestDataHelperDataLockEventsDatabase.GetAllEvents();

            Assert.IsNotNull(events);
            Assert.AreEqual(1, events.Count);

            var @event = events[0];

            Assert.AreEqual(ukprn, @event.Ukprn);
            Assert.AreEqual(EventStatus.New, (EventStatus)@event.Status);

            var eventErrors = @event.ErrorCodes;
            Assert.IsNotNull(eventErrors);
            Assert.AreEqual("[\"E1\"]", eventErrors);
        }

        [Test]
        public void ThenItShouldWriteMultipleEventsWhenErrorsForMultipleCommitmentsOnTheSamePriceEpisodeAreProduced()
        {
            // Arrange
            var ukprn = 10000534;
            var commitmentId1 = 1;
            var commitmentId2 = 2;

            TestDataHelperDeds.AddProvider(ukprn, DateTime.Today);
            TestDataHelperDeds.AddDataLock(new long[]{commitmentId1, commitmentId2}, ukprn, "Lrn-001", errorCodesCsv: "E1");

            Act(() => TestDataHelperDataLockEventsDatabase.Count("select count(*) from [DataLockEvents].[Provider] where Ukprn = @ukprn and IlrSubmissionDateTime = @date", new {ukprn, date = DateTime.Today}) > 0);

            // Assert
            var events = TestDataHelperDataLockEventsDatabase.GetAllEvents();

            Assert.IsNotNull(events);
            Assert.AreEqual(2, events.Count);
            Assert.AreEqual(1, events.Count(x => x.ApprenticeshipId == commitmentId1), "More than 1 event for commitment 1");
            Assert.AreEqual(1, events.Count(x => x.ApprenticeshipId == commitmentId2), "More than 1 event for commitment 2");
        }

        //[Explicit]
        [TestCase(1200, 30)]
        //[TestCase(20000, 300)]
        public void ThenItShouldCompleteInAnAcceptableTime(int numberOfLearners, int expectedMaxElapsed)
        {
            // Arrange
            var ukprn = 10000534;

            TestDataHelperDeds.AddProvider(ukprn, DateTime.Today);

            for (var i = 1; i <= numberOfLearners; i++)
            {
                var learnRefNumber = $"Lrn-{i:0000}";
                TestDataHelperDeds.AddDataLock(new long[]{i}, ukprn, learnRefNumber, errorCodesCsv: "E1");
            }

            // Act
            var stopwatch = Stopwatch.StartNew();
            Act(() =>
            {
                var actualCount = TestDataHelperDataLockEventsDatabase.Count("select count(*) from [DataLockEvents].[DataLockEvent] where Ukprn = @ukprn", new {ukprn});
                return actualCount >= numberOfLearners;
            }, 500);

            stopwatch.Stop();

            // Assert
            Console.WriteLine($"Execution took {stopwatch.Elapsed.TotalSeconds:0.0}");
            Assert.IsTrue(stopwatch.Elapsed.TotalSeconds < expectedMaxElapsed, $"Expected to complete in less than {expectedMaxElapsed} seconds but took {stopwatch.Elapsed.TotalSeconds:0.0}");
        }

    }
}