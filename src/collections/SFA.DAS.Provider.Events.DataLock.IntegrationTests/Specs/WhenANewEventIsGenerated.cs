using System;
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

        [Test, Explicit]
        public void ThenItShouldCompleteInAnAcceptableTimeInASubmissionRun()
        {
            // Arrange
            var ukprn = 10000534;
            var numberOfLearners = 1200;

            TestDataHelper.AddLearningProvider(ukprn);
            TestDataHelper.AddFileDetails(ukprn);
            for (var i = 1; i <= numberOfLearners; i++)
            {
                var learnRefNumber = $"Lrn-{i:0000}";
                TestDataHelper.AddCommitment(i, ukprn, learnRefNumber, passedDataLock: false);
                TestDataHelper.AddIlrDataForCommitment(i, learnRefNumber);
            }
            TestDataHelper.SubmissionCopyReferenceData();

            // Act
            var startTime = DateTime.Now;
            TaskRunner.RunTask();

            // Assert
            var duration = DateTime.Now - startTime;
            Console.WriteLine($"Execution took {duration.TotalSeconds:0.0}");
            Assert.IsTrue(duration.TotalSeconds < 30, $"Expected to complete in less than 30 seconds but took {duration.TotalSeconds:0.0}");
        }

        [Test]
        public void ThenItShouldCompleteInAnAcceptableTimeInAPeriodEndRun()
        {
            // Arrange
            var ukprn = 10000534;
            var numberOfLearners = 20000;

            TestDataHelper.PeriodEndAddLearningProvider(ukprn);
            for (var i = 1; i <= numberOfLearners; i++)
            {
                var learnRefNumber = $"Lrn-{i:0000}";
                TestDataHelper.PeriodEndAddCommitment(i, ukprn, learnRefNumber, passedDataLock: false);
                TestDataHelper.PeriodEndAddIlrDataForCommitment(i, learnRefNumber);
            }

            TestDataHelper.PeriodEndCopyReferenceData();

            // Act
            var startTime = DateTime.Now;
            TaskRunner.RunTask(eventsSource: EventSource.PeriodEnd);

            // Assert
            var duration = DateTime.Now - startTime;
            Console.WriteLine($"Execution took {duration.TotalSeconds:0.0}");
            Assert.IsTrue(duration.TotalSeconds < 600, $"Expected to complete in less than 300 seconds but took {duration.TotalSeconds:0.0}");
        }

    }
}