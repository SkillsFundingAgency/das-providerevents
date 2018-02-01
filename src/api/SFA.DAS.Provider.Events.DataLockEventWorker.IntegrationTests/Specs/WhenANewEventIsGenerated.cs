using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Types;
using EventSource = System.Diagnostics.Tracing.EventSource;

namespace SFA.DAS.Provider.Events.DataLockEventWorker.IntegrationTests.Specs
{
    public class WhenANewEventIsGenerated
    {
        private TestDataHelper _helper = new TestDataHelper();
        //private DataLockEventProcessor _processor = new DataLockEventProcessor();

        private void Setup()
        {
            _helper.Clean();
        }

        [Test]
        public void ThenItShouldBeWrittenToTheDatabase()
        {
            // Arrange
            Setup();

            var ukprn = 10000534;
            var commitmentId = 1;

            _helper.AddLearningProvider(ukprn);
            //_helper.AddFileDetails(ukprn);
            _helper.AddCommitment(commitmentId, ukprn, "Lrn-001", passedDataLock: false);
            _helper.AddIlrDataForCommitment(commitmentId, "Lrn-001");

            //_helper.CopyReferenceData();

            ////Act
            //_processor.Run();

            //TaskRunner.RunTask(eventsSource:
            //    context == TestFixtureContext.PeriodEnd
            //    ? EventSource.PeriodEnd
            //    : EventSource.Submission);


            // Assert
            var events = _helper.GetAllEvents();

            Assert.IsNotNull(events);
            Assert.AreEqual(1, events.Count);

            var @event = events[0];

            //Assert.AreEqual(1, @event.Id);
            Assert.AreEqual(ukprn, @event.Ukprn);
            //Assert.AreEqual(commitmentId, @event.CommitmentId);
            Assert.AreEqual(EventStatus.New, @event.Status);

            var eventErrors = @event.Errors;// _helper.GetAllEventErrors(@event.DataLockEventId);
            var eventPeriods = @event.Periods;// _helper.GetAllEventPeriods(@event.DataLockEventId);
            var eventCommitmentVersions = @event.Apprenticeships;// _helper.GetAllEventCommitmentVersions(@event.DataLockEventId);

            Assert.IsNotNull(eventErrors);
            Assert.IsNotNull(eventPeriods);
            Assert.IsNotNull(eventCommitmentVersions);

            Assert.AreEqual(1, eventErrors.Length);
            Assert.AreEqual(36, eventPeriods.Length);
            Assert.AreEqual(1, eventCommitmentVersions.Length);
        }

        [Test]
        public void ThenItShouldWriteMultipleEventsWhenErrorsForMultipleCommitmentsOnTheSamePriceEpisodeAreProduced()
        {
            // Arrange
            Setup();

            var ukprn = 10000534;
            var commitmentId1 = 1;
            var commitmentId2 = 2;

            _helper.AddLearningProvider(ukprn);
//            _helper.AddFileDetails(ukprn);
            _helper.AddCommitment(commitmentId1, ukprn, "Lrn-001", passedDataLock: false);
            _helper.AddCommitment(commitmentId2, ukprn, "Lrn-001", passedDataLock: false);
            _helper.AddIlrDataForCommitment(commitmentId1, "Lrn-001");

//            _helper.CopyReferenceData();

            // Act
            //_processor.Run();
            //TaskRunner.RunTask(eventsSource:
            //    context == TestFixtureContext.PeriodEnd
            //    ? EventSource.PeriodEnd
            //    : EventSource.Submission);

            // Assert
            var events = _helper.GetAllEvents();

            Assert.IsNotNull(events);
            Assert.AreEqual(2, events.Count);
            Assert.AreEqual(1, events.Count(x => x.ApprenticeshipId == commitmentId1), "More than 1 event for commitment 1");
            Assert.AreEqual(1, events.Count(x => x.ApprenticeshipId == commitmentId2), "More than 1 event for commitment 2");
        }

        [Explicit]
        [TestCase(1200, 30)]
        [TestCase(20000, 300)]
        public void ThenItShouldCompleteInAnAcceptableTime(int numberOfLearners, int expectedMaxElapsed)
        {
            // Arrange
            Setup();

            var ukprn = 10000534;

            _helper.AddLearningProvider(ukprn);
            //_helper.AddFileDetails(ukprn);
            for (var i = 1; i <= numberOfLearners; i++)
            {
                var learnRefNumber = $"Lrn-{i:0000}";
                _helper.AddCommitment(i, ukprn, learnRefNumber, passedDataLock: false);
                _helper.AddIlrDataForCommitment(i, learnRefNumber);
            }
            //_helper.CopyReferenceData();

            // Act
            var stopwatch = Stopwatch.StartNew();
            //_processor.Run();
            //TaskRunner.RunTask(eventsSource:
            //    context == TestFixtureContext.PeriodEnd
            //        ? EventSource.PeriodEnd
            //        : EventSource.Submission);
            stopwatch.Stop();

            // Assert
            Console.WriteLine($"Execution took {stopwatch.Elapsed.TotalSeconds:0.0}");
            Assert.IsTrue(stopwatch.Elapsed.TotalSeconds < expectedMaxElapsed, $"Expected to complete in less than {expectedMaxElapsed} seconds but took {stopwatch.Elapsed.TotalSeconds:0.0}");
        }

    }
}