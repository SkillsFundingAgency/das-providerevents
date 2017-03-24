using NUnit.Framework;
using SFA.DAS.Provider.Events.DataLock.Domain;
using SFA.DAS.Provider.Events.DataLock.IntegrationTests.Execution;
using SFA.DAS.Provider.Events.DataLock.IntegrationTests.Helpers;

namespace SFA.DAS.Provider.Events.DataLock.IntegrationTests.Specs
{
    public class WhenAnExistingEventIsFound
    {
        [SetUp]
        public void Arrange()
        {
            TestDataHelper.Clean();
        }

        [Test]
        public void ThenNoNewEventsShouldBeWrittenIfNothingChangedInASubmissionRun()
        {
            // Arrange
            var ukprn = 10000534;
            var commitmentId = 1;

            TestDataHelper.AddLearningProvider(ukprn);
            TestDataHelper.AddFileDetails(ukprn);
            TestDataHelper.AddCommitment(commitmentId, ukprn, "Lrn-001", passedDataLock: false);
            TestDataHelper.AddIlrDataForCommitment(commitmentId, "Lrn-001");

            TestDataHelper.AddDataLockEvent(ukprn, "Lrn-001", passedDataLock: false);

            TestDataHelper.SubmissionCopyReferenceData();

            // Act
            TaskRunner.RunTask();

            // Assert
            var events = TestDataHelper.GetAllEvents();

            Assert.IsNotNull(events);
            Assert.AreEqual(0, events.Length);
        }

        [Test]
        public void ThenNoNewEventsShouldBeWrittenIfNothingChangedInAPeriodEndRun()
        {
            // Arrange
            var ukprn = 10000534;
            var commitmentId = 1;

            TestDataHelper.PeriodEndAddLearningProvider(ukprn);
            TestDataHelper.PeriodEndAddCommitment(commitmentId, ukprn, "Lrn-001", passedDataLock: false);
            TestDataHelper.PeriodEndAddIlrDataForCommitment(commitmentId, "Lrn-001");

            TestDataHelper.AddDataLockEvent(ukprn, "Lrn-001", passedDataLock: false);

            TestDataHelper.PeriodEndCopyReferenceData();

            // Act
            TaskRunner.RunTask(eventsSource: EventSource.PeriodEnd);

            // Assert
            var events = TestDataHelper.GetAllEvents(false);

            Assert.IsNotNull(events);
            Assert.AreEqual(0, events.Length);
        }

        [Test]
        public void ThenANewEventShouldBeWrittenIfSomethingChangedInASubmissionRun()
        {
            // Arrange
            var ukprn = 10000534;
            var commitmentId = 1;

            TestDataHelper.AddLearningProvider(ukprn);
            TestDataHelper.AddFileDetails(ukprn);
            TestDataHelper.AddCommitment(commitmentId, ukprn, "Lrn-001", passedDataLock: false);
            TestDataHelper.AddIlrDataForCommitment(commitmentId, "Lrn-001");

            TestDataHelper.AddDataLockEvent(ukprn, "Lrn-001");

            TestDataHelper.SubmissionCopyReferenceData();

            // Act
            TaskRunner.RunTask();

            // Assert
            var events = TestDataHelper.GetAllEvents();

            Assert.IsNotNull(events);
            Assert.AreEqual(1, events.Length);

            var @event = events[0];

            Assert.AreEqual(2, @event.Id);
            Assert.AreEqual(ukprn, @event.Ukprn);
            Assert.AreEqual(commitmentId, @event.CommitmentId);

            var eventErrors = TestDataHelper.GetAllEventErrors(@event.Id);
            var eventPeriods = TestDataHelper.GetAllEventPeriods(@event.Id);
            var eventCommitmentVersions = TestDataHelper.GetAllEventCommitmentVersions(@event.Id);

            Assert.IsNotNull(eventErrors);
            Assert.IsNotNull(eventPeriods);
            Assert.IsNotNull(eventCommitmentVersions);

            Assert.AreEqual(1, eventErrors.Length);
            Assert.AreEqual(84, eventPeriods.Length);
            Assert.AreEqual(1, eventCommitmentVersions.Length);
        }

        [Test]
        public void ThenANewEventShouldBeWrittenIfSomethingChangedInAPeriodEndRun()
        {
            // Arrange
            var ukprn = 10000534;
            var commitmentId = 1;

            TestDataHelper.PeriodEndAddLearningProvider(ukprn);
            TestDataHelper.PeriodEndAddCommitment(commitmentId, ukprn, "Lrn-001", passedDataLock: false);
            TestDataHelper.PeriodEndAddIlrDataForCommitment(commitmentId, "Lrn-001");

            TestDataHelper.AddDataLockEvent(ukprn, "Lrn-001");

            TestDataHelper.PeriodEndCopyReferenceData();

            // Act
            TaskRunner.RunTask(eventsSource: EventSource.PeriodEnd);

            // Assert
            var events = TestDataHelper.GetAllEvents(false);

            Assert.IsNotNull(events);
            Assert.AreEqual(1, events.Length);

            var @event = events[0];

            Assert.AreEqual(2, @event.Id);
            Assert.AreEqual(ukprn, @event.Ukprn);
            Assert.AreEqual(commitmentId, @event.CommitmentId);

            var eventErrors = TestDataHelper.GetAllEventErrors(@event.Id, false);
            var eventPeriods = TestDataHelper.GetAllEventPeriods(@event.Id, false);
            var eventCommitmentVersions = TestDataHelper.GetAllEventCommitmentVersions(@event.Id, false);

            Assert.IsNotNull(eventErrors);
            Assert.IsNotNull(eventPeriods);
            Assert.IsNotNull(eventCommitmentVersions);

            Assert.AreEqual(1, eventErrors.Length);
            Assert.AreEqual(84, eventPeriods.Length);
            Assert.AreEqual(1, eventCommitmentVersions.Length);
        }
    }
}