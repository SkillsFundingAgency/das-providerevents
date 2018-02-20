using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.DataLockEventWorker.AcceptanceTests.FromPaymentsComponent
{
    [TestFixture(Description = "Ported from Feature: Datalock produces correct errors when ILR does not match commitment")]
    public class DataLockErrorsTest : DataLockProcessorTestBase
    {
        [Test(Description = "Scenario: DLOCK07 - When no matching record found in an employer digital account for for the agreed price then datalock DLOCK_07 will be produced")]
        public void Dlock07()
        {
            // Arrange
            long ukprn = 10000;
            TestDataHelperDeds.AddProvider(ukprn, DateTime.Today);
            TestDataHelperDeds.AddDataLock(new long[] {73}, ukprn, "learner a", 1, 11000, new DateTime(2017, 5, 1), new DateTime(2018, 5, 1), 10000, 23, 2, 450, 1, "DLOCK_07", "2-450-1-01/05/2017", 73, new DateTime(2017, 5, 1), null, 10010, 2);

            Act(() => TestDataHelperDataLockEventsDatabase.Count("select count(*) from [DataLockEvents].[Provider] where Ukprn = @ukprn and IlrSubmissionDateTime = @date", new {ukprn, date = DateTime.Today}) > 0);

            // Assert
            var events = TestDataHelperDataLockEventsDatabase.GetAllEvents();

            Assert.IsNotNull(events);
            Assert.AreEqual(1, events.Count);

            var @event = events[0];

            Assert.AreEqual(ukprn, @event.Ukprn);
            Assert.AreEqual(1L, @event.AimSeqNumber);
            Assert.AreEqual(73L, @event.CommitmentId);
            Assert.AreEqual(true, @event.HasErrors);
            Assert.AreEqual("learner a", @event.LearnRefNumber);
            Assert.AreEqual("2-450-1-01/05/2017", @event.PriceEpisodeIdentifier);
            Assert.AreEqual(73L, @event.EmployerAccountId);
            Assert.AreEqual(EventStatus.New, (EventStatus)@event.Status);
            Assert.AreEqual(2m, @event.IlrEndpointAssessorPrice); // Tnp2
            Assert.AreEqual(450, @event.IlrFrameworkCode);
            Assert.AreEqual(1, @event.IlrPathwayCode);
            Assert.AreEqual(new DateTime(2017, 5, 1), @event.IlrPriceEffectiveFromDate);
            Assert.IsNull(@event.IlrPriceEffectiveToDate);
            Assert.AreEqual(2, @event.IlrProgrammeType);
            Assert.AreEqual(23, @event.IlrStandardCode); // original specflow has null here but it looked like a bug
            Assert.AreEqual(new DateTime(2017, 5, 1), @event.IlrStartDate);
            Assert.AreEqual(10010m, @event.IlrTrainingPrice);
            Assert.AreEqual(11000L, @event.Uln);

            var eventErrors = @event.ErrorCodes;
            Assert.IsNotNull(eventErrors);
            Assert.AreEqual("[\"DLOCK_07\"]", eventErrors);
        }

        [Test(Description = "Scenario: DLOCK09 - When no matching record found in an employer digital account for for the start date then datalock DLOCK_09 will be produced")]
        public void Dlock09()
        {
            // Arrange
            long ukprn = 10000;
            TestDataHelperDeds.AddProvider(ukprn, DateTime.Today);
            TestDataHelperDeds.AddDataLock(new long[] {73}, ukprn, "learner a", 1, 11000, new DateTime(2017, 6, 1), new DateTime(2018, 5, 1), 10000, 23, 2, 450, 1, "DLOCK_09", "2-450-1-01/05/2017", 73, new DateTime(2017, 5, 1), null, 10010, 2);

            Act(() => TestDataHelperDataLockEventsDatabase.Count("select count(*) from [DataLockEvents].[Provider] where Ukprn = @ukprn and IlrSubmissionDateTime = @date", new {ukprn, date = DateTime.Today}) > 0);

            // Assert
            var events = TestDataHelperDataLockEventsDatabase.GetAllEvents();

            Assert.IsNotNull(events);
            Assert.AreEqual(1, events.Count);

            var @event = events[0];

            Assert.AreEqual(ukprn, @event.Ukprn);
            Assert.AreEqual(1L, @event.AimSeqNumber);
            Assert.AreEqual(73L, @event.CommitmentId);
            Assert.AreEqual(true, @event.HasErrors);
            Assert.AreEqual("learner a", @event.LearnRefNumber);
            Assert.AreEqual("2-450-1-01/05/2017", @event.PriceEpisodeIdentifier);
            Assert.AreEqual(73L, @event.EmployerAccountId);
            Assert.AreEqual(EventStatus.New, (EventStatus)@event.Status);
            Assert.AreEqual(2m, @event.IlrEndpointAssessorPrice); // Tnp2
            Assert.AreEqual(450, @event.IlrFrameworkCode);
            Assert.AreEqual(1, @event.IlrPathwayCode);
            Assert.AreEqual(new DateTime(2017, 5, 1), @event.IlrPriceEffectiveFromDate);
            Assert.IsNull(@event.IlrPriceEffectiveToDate);
            Assert.AreEqual(2, @event.IlrProgrammeType);
            Assert.AreEqual(23, @event.IlrStandardCode); // original specflow has null here but it looked like a bug
            Assert.AreEqual(new DateTime(2017, 6, 1), @event.IlrStartDate);
            Assert.AreEqual(10010m, @event.IlrTrainingPrice);
            Assert.AreEqual(11000L, @event.Uln);

            var eventErrors = @event.ErrorCodes;
            Assert.IsNotNull(eventErrors);
            Assert.AreEqual("[\"DLOCK_09\"]", eventErrors);
        }


        [Test(Description = "Scenario: DLOCK08 - When multiple matching record found in an employer digital account then datalock DLOCK_08 will be produced")]
        public void Dlock08()
        {
            // Arrange
            long ukprn = 10000;
            TestDataHelperDeds.AddProvider(ukprn, DateTime.Today);
            TestDataHelperDeds.AddDataLock(new long[] {73}, ukprn, "learner a", 1, 11000, new DateTime(2017, 5, 1), new DateTime(2018, 5, 1), 10000, 23, 2, 450, 1, "DLOCK_08", "2-450-1-01/05/2017", 77, new DateTime(2017, 5, 1), null, 10010, 2);
            TestDataHelperDeds.AddDataLock(new long[] {74}, ukprn, "learner a", 1, 11000, new DateTime(2017, 5, 1), new DateTime(2018, 5, 1), 10000, 23, 2, 450, 1, "DLOCK_08", "2-450-1-01/05/2017", 88, new DateTime(2017, 5, 1), null, 10010, 2);

            Act(() => TestDataHelperDataLockEventsDatabase.Count("select count(*) from [DataLockEvents].[Provider] where Ukprn = @ukprn and IlrSubmissionDateTime = @date", new {ukprn, date = DateTime.Today}) > 0);

            // Assert
            var events = TestDataHelperDataLockEventsDatabase.GetAllEvents();

            Assert.IsNotNull(events);
            Assert.AreEqual(2, events.Count);

            var @event = events[0];

            Assert.AreEqual(ukprn, @event.Ukprn);
            Assert.AreEqual(1L, @event.AimSeqNumber);
            Assert.AreEqual(73L, @event.CommitmentId);
            Assert.AreEqual(true, @event.HasErrors);
            Assert.AreEqual("learner a", @event.LearnRefNumber);
            Assert.AreEqual("2-450-1-01/05/2017", @event.PriceEpisodeIdentifier);
            Assert.AreEqual(77L, @event.EmployerAccountId);
            Assert.AreEqual(EventStatus.New, (EventStatus)@event.Status);
            Assert.AreEqual(2m, @event.IlrEndpointAssessorPrice); // Tnp2
            Assert.AreEqual(450, @event.IlrFrameworkCode);
            Assert.AreEqual(1, @event.IlrPathwayCode);
            Assert.AreEqual(new DateTime(2017, 5, 1), @event.IlrPriceEffectiveFromDate);
            Assert.IsNull(@event.IlrPriceEffectiveToDate);
            Assert.AreEqual(2, @event.IlrProgrammeType);
            Assert.AreEqual(23, @event.IlrStandardCode); // original specflow has null here but it looked like a bug
            Assert.AreEqual(new DateTime(2017, 5, 1), @event.IlrStartDate);
            Assert.AreEqual(10010m, @event.IlrTrainingPrice);
            Assert.AreEqual(11000L, @event.Uln);

            var eventErrors = @event.ErrorCodes;
            Assert.IsNotNull(eventErrors);
            Assert.AreEqual("[\"DLOCK_08\"]", eventErrors);



            @event = events[1];

            Assert.AreEqual(ukprn, @event.Ukprn);
            Assert.AreEqual(1L, @event.AimSeqNumber);
            Assert.AreEqual(74L, @event.CommitmentId);
            Assert.AreEqual(true, @event.HasErrors);
            Assert.AreEqual("learner a", @event.LearnRefNumber);
            Assert.AreEqual("2-450-1-01/05/2017", @event.PriceEpisodeIdentifier);
            Assert.AreEqual(88L, @event.EmployerAccountId);
            Assert.AreEqual(EventStatus.New, (EventStatus)@event.Status);
            Assert.AreEqual(2m, @event.IlrEndpointAssessorPrice); // Tnp2
            Assert.AreEqual(450, @event.IlrFrameworkCode);
            Assert.AreEqual(1, @event.IlrPathwayCode);
            Assert.AreEqual(new DateTime(2017, 5, 1), @event.IlrPriceEffectiveFromDate);
            Assert.IsNull(@event.IlrPriceEffectiveToDate);
            Assert.AreEqual(2, @event.IlrProgrammeType);
            Assert.AreEqual(23, @event.IlrStandardCode); // original specflow has null here but it looked like a bug
            Assert.AreEqual(new DateTime(2017, 5, 1), @event.IlrStartDate);
            Assert.AreEqual(10010m, @event.IlrTrainingPrice);
            Assert.AreEqual(11000L, @event.Uln);

            eventErrors = @event.ErrorCodes;
            Assert.IsNotNull(eventErrors);
            Assert.AreEqual("[\"DLOCK_08\"]", eventErrors);
        }
    }
}
