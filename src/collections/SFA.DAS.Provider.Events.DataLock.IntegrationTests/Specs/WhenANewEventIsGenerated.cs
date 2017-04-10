using NUnit.Framework;
using SFA.DAS.Provider.Events.DataLock.Domain;
using SFA.DAS.Provider.Events.DataLock.IntegrationTests.Execution;
using SFA.DAS.Provider.Events.DataLock.IntegrationTests.Helpers;

namespace SFA.DAS.Provider.Events.DataLock.IntegrationTests.Specs
{
    public class WhenANewEventIsGenerated
    {
        [SetUp]
        public void Arrange()
        {
            TestDataHelper.Clean();
        }

        [Test]
        public void ThenItShouldBeWrittenToTheDatabaseInASubmissionRun()
        {
            // Arrange
            var ukprn = 10000534;
            var commitmentId = 1;

            TestDataHelper.AddLearningProvider(ukprn);
            TestDataHelper.AddFileDetails(ukprn);
            TestDataHelper.AddCommitment(commitmentId, ukprn, "Lrn-001", passedDataLock: false);
            TestDataHelper.AddIlrDataForCommitment(commitmentId, "Lrn-001");

            TestDataHelper.SubmissionCopyReferenceData();

            // Act
            TaskRunner.RunTask();

            // Assert
            var events = TestDataHelper.GetAllEvents();

            Assert.IsNotNull(events);
            Assert.AreEqual(1, events.Length);

            var @event = events[0];

            //Assert.AreEqual(1, @event.Id);
            Assert.AreEqual(ukprn, @event.Ukprn);
            Assert.AreEqual(commitmentId, @event.CommitmentId);

            var eventErrors = TestDataHelper.GetAllEventErrors(@event.DataLockEventId);
            var eventPeriods = TestDataHelper.GetAllEventPeriods(@event.DataLockEventId);
            var eventCommitmentVersions = TestDataHelper.GetAllEventCommitmentVersions(@event.DataLockEventId);

            Assert.IsNotNull(eventErrors);
            Assert.IsNotNull(eventPeriods);
            Assert.IsNotNull(eventCommitmentVersions);

            Assert.AreEqual(1, eventErrors.Length);
            Assert.AreEqual(180, eventPeriods.Length);
            Assert.AreEqual(1, eventCommitmentVersions.Length);
        }

        [Test]
        public void ThenItShouldBeWrittenToTheDatabaseInAPeriodEndRun()
        {
            // Arrange
            var ukprn = 10000534;
            var commitmentId = 1;

            TestDataHelper.PeriodEndAddLearningProvider(ukprn);
            TestDataHelper.PeriodEndAddCommitment(commitmentId, ukprn, "Lrn-001", passedDataLock: false);
            TestDataHelper.PeriodEndAddIlrDataForCommitment(commitmentId, "Lrn-001");

            TestDataHelper.PeriodEndCopyReferenceData();

            // Act
            TaskRunner.RunTask(eventsSource: EventSource.PeriodEnd);

            // Assert
            var events = TestDataHelper.GetAllEvents(false);

            Assert.IsNotNull(events);
            Assert.AreEqual(1, events.Length);

            var @event = events[0];

            //Assert.AreEqual(1, @event.Id);
            Assert.AreEqual(ukprn, @event.Ukprn);
            Assert.AreEqual(commitmentId, @event.CommitmentId);

            var eventErrors = TestDataHelper.GetAllEventErrors(@event.DataLockEventId, false);
            var eventPeriods = TestDataHelper.GetAllEventPeriods(@event.DataLockEventId, false);
            var eventCommitmentVersions = TestDataHelper.GetAllEventCommitmentVersions(@event.DataLockEventId, false);

            Assert.IsNotNull(eventErrors);
            Assert.IsNotNull(eventPeriods);
            Assert.IsNotNull(eventCommitmentVersions);

            Assert.AreEqual(1, eventErrors.Length);
            Assert.AreEqual(180, eventPeriods.Length);
            Assert.AreEqual(1, eventCommitmentVersions.Length);
        }
    }
}