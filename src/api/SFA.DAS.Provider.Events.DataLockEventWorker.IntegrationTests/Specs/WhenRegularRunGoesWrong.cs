using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.DataLockEventWorker.AcceptanceTests.Specs
{
    [TestFixture]
    public class WhenRegularRunGoesWrong : DataLockProcessorTestBase
    {
        private const long Ukprn = 10000777;
        private const long CommitmentId = 7;
        private const string LearnerRefNumber = "Lrn-777";

        [SetUp]
        public override void SetupBase()
        {
            base.SetupBase();
            TestDataHelperDataLockEventsDatabase.PopulateInitialRun(Ukprn, DateTime.Today.AddDays(-1));
            TestDataHelperDataLockEventsDatabase.AddProvider(Ukprn, DateTime.Today.AddDays(-1));
        }

        [Test]
        public void AndWriteDataLockEventsFailsAfterFirstPageThenOnlyFirstPageOfDataLocksShouldStay()
        {
            //Arrange
            // write data lock events should fail due to long value
            TestDataHelperDeds.AddProvider(Ukprn, DateTime.Today);
            TestDataHelperDeds.AddDataLock(new[] {CommitmentId}, Ukprn, LearnerRefNumber, priceEpisodeIdentifier: "1", errorCodesCsv: "E1");
            TestDataHelperDeds.AddDataLock(new[] {CommitmentId}, Ukprn, LearnerRefNumber, priceEpisodeIdentifier: "2", errorCodesCsv: "E1");
            TestDataHelperDeds.AddDataLock(new[] {CommitmentId}, Ukprn, LearnerRefNumber, priceEpisodeIdentifier: "3", errorCodesCsv: "E1");
            TestDataHelperDeds.AddDataLock(new[] {CommitmentId}, Ukprn, LearnerRefNumber, priceEpisodeIdentifier: "4", errorCodesCsv: "E1");
            TestDataHelperDeds.AddDataLockEvent(Ukprn, DateTime.Today, EventStatus.Removed, 1, "1", 1, DateTime.Today.AddDays(-1), null);
            TestDataHelperDeds.AddDataLockEvent(Ukprn, DateTime.Today, EventStatus.Updated, 2, "2", 2, DateTime.Today.AddDays(-1), new[] {"E1", "E2"});
            TestDataHelperDeds.AddDataLockEvent(Ukprn, DateTime.Today, EventStatus.Removed, 3, "3", 3, DateTime.Today.AddDays(-1), null);
            TestDataHelperDeds.AddDataLockEvent(Ukprn, DateTime.Today, EventStatus.Removed, 4, "4", 4, DateTime.Today.AddDays(-1), null);

            // in DEDS emulation DB PriceEpisodeIdentifier is fudged to be 26 chars, should be 25
            TestDataHelperDeds.Execute("update [Rulebase].[AEC_ApprenticeshipPriceEpisode] set [PriceEpisodeIdentifier] = '" + new string('X', 26) + "' where [PriceEpisodeIdentifier] = '3'");
            TestDataHelperDeds.Execute("update [DataLock].[PriceEpisodeMatch] set [PriceEpisodeIdentifier] = '" + new string('X', 26) + "' where [PriceEpisodeIdentifier] = '3'");

            //Act
            Act(() => TestDataHelperDataLockEventsDatabase.Count(@"select
                 (select count(*) from [DataLockEvents].[ProcessorRun] where [IsInitialRun] = 0 and FinishTimeUtc is not null) -
                 (select count(*) from [DataLockEvents].[Provider] where HandledBy is not null)") == 1,
                pageSize: 2);

            //Assert
            var providers = TestDataHelperDataLockEventsDatabase.GetAllProviders();
            Assert.AreEqual(1, providers.Count);
            Assert.IsFalse(providers[0].RequiresInitialImport);
            Assert.IsNull(providers[0].HandledBy);

            var events = TestDataHelperDataLockEventsDatabase.GetAllEvents();
            var dataLocks = TestDataHelperDataLockEventsDatabase.GetAllLastDataLocks();

            Assert.AreEqual(2, events.Count);
            Assert.AreEqual("1", events[0].PriceEpisodeIdentifier);
            Assert.AreEqual("2", events[1].PriceEpisodeIdentifier);

            Assert.AreEqual(2, dataLocks.Count);
            Assert.AreEqual("1", dataLocks[0].PriceEpisodeIdentifier);
            Assert.AreEqual("2", dataLocks[1].PriceEpisodeIdentifier);








            // subsequent run shouldn't change a thing
            TestDataHelperDeds.Execute("update [dbo].[FileDetails] set [SubmittedTime] = @time", new {time = DateTime.Now});
            
            Act(() => TestDataHelperDataLockEventsDatabase.Count(@"select
                 (select count(*) from [DataLockEvents].[ProcessorRun] where [IsInitialRun] = 0 and FinishTimeUtc is not null) -
                 (select count(*) from [DataLockEvents].[Provider] where HandledBy is not null)") == 2,
                pageSize: 2);

            providers = TestDataHelperDataLockEventsDatabase.GetAllProviders();
            Assert.AreEqual(1, providers.Count);
            Assert.IsFalse(providers[0].RequiresInitialImport);
            Assert.IsNull(providers[0].HandledBy);

            events = TestDataHelperDataLockEventsDatabase.GetAllEvents();
            dataLocks = TestDataHelperDataLockEventsDatabase.GetAllLastDataLocks();

            Assert.AreEqual(2, events.Count);
            Assert.AreEqual("1", events[0].PriceEpisodeIdentifier);
            Assert.AreEqual("2", events[1].PriceEpisodeIdentifier);

            Assert.AreEqual(2, dataLocks.Count);
            Assert.AreEqual("1", dataLocks[0].PriceEpisodeIdentifier);
            Assert.AreEqual("2", dataLocks[1].PriceEpisodeIdentifier);









            // subsequent run after data fix should not produce duplicates
            TestDataHelperDeds.Execute("update [Rulebase].[AEC_ApprenticeshipPriceEpisode] set [PriceEpisodeIdentifier] = '3' where [PriceEpisodeIdentifier] = '" + new string('X', 26) + "'");
            TestDataHelperDeds.Execute("update [DataLock].[PriceEpisodeMatch] set [PriceEpisodeIdentifier] = '3' where [PriceEpisodeIdentifier] = '" + new string('X', 26) + "'");

            TestDataHelperDeds.Execute("update [dbo].[FileDetails] set [SubmittedTime] = @time", new {time = DateTime.Now});
            
            Act(() => TestDataHelperDataLockEventsDatabase.Count(@"select
                 (select count(*) from [DataLockEvents].[ProcessorRun] where [IsInitialRun] = 0 and FinishTimeUtc is not null) -
                 (select count(*) from [DataLockEvents].[Provider] where HandledBy is not null)") == 3,
                pageSize: 2);

            providers = TestDataHelperDataLockEventsDatabase.GetAllProviders();
            Assert.AreEqual(1, providers.Count);
            Assert.IsFalse(providers[0].RequiresInitialImport);
            Assert.IsNull(providers[0].HandledBy);

            events = TestDataHelperDataLockEventsDatabase.GetAllEvents();
            dataLocks = TestDataHelperDataLockEventsDatabase.GetAllLastDataLocks();

            Assert.AreEqual(4, events.Count);
            Assert.AreEqual("1", events[0].PriceEpisodeIdentifier);
            Assert.AreEqual("2", events[1].PriceEpisodeIdentifier);
            Assert.AreEqual("3", events[2].PriceEpisodeIdentifier);
            Assert.AreEqual("4", events[3].PriceEpisodeIdentifier);

            Assert.AreEqual(4, dataLocks.Count);
            Assert.AreEqual("1", dataLocks[0].PriceEpisodeIdentifier);
            Assert.AreEqual("2", dataLocks[1].PriceEpisodeIdentifier);
            Assert.AreEqual("3", dataLocks[2].PriceEpisodeIdentifier);
            Assert.AreEqual("4", dataLocks[3].PriceEpisodeIdentifier);
        }



        [Test]
        public void AndWriteDataLocksFailsAfterFirstPageThenDataIsIntegral()
        {
            //Arrange
            // write data lock events should fail due to long value
            TestDataHelperDeds.AddProvider(Ukprn, DateTime.Today);
            TestDataHelperDeds.AddDataLock(new[] {11L}, Ukprn, LearnerRefNumber, priceEpisodeIdentifier: "1", errorCodesCsv: "E1", aimSequenceNumber: 38, programmeType: 48);
            TestDataHelperDeds.AddDataLock(new[] {12L}, Ukprn, LearnerRefNumber, priceEpisodeIdentifier: "2", errorCodesCsv: "E1", aimSequenceNumber: 39, programmeType: 49);
            TestDataHelperDeds.AddDataLock(new[] {13L}, Ukprn, LearnerRefNumber, priceEpisodeIdentifier: "3", errorCodesCsv: "E1", aimSequenceNumber: 41);
            TestDataHelperDeds.AddDataLock(new[] {14L}, Ukprn, LearnerRefNumber, priceEpisodeIdentifier: "4", errorCodesCsv: "E1", aimSequenceNumber: 40, programmeType: 50);
            TestDataHelperDeds.AddDataLockEvent(Ukprn, DateTime.Today, EventStatus.Removed, 38, "1", 1, DateTime.Today.AddDays(-1), null);
            TestDataHelperDeds.AddDataLockEvent(Ukprn, DateTime.Today, EventStatus.Updated, 39, "2", 2, DateTime.Today.AddDays(-1), new[] {"E1", "E2"});
            TestDataHelperDeds.AddDataLockEvent(Ukprn, DateTime.Today, EventStatus.Removed, 41, "3", 1, DateTime.Today.AddDays(-1), null);
            TestDataHelperDeds.AddDataLockEvent(Ukprn, DateTime.Today, EventStatus.Removed, 40, "4", 1, DateTime.Today.AddDays(-1), null);

            // fudge column to not accept nulls so it can fail on third data lock
            TestDataHelperDataLockEventsDatabase.Execute("alter table DataLockEvents.LastDataLock alter column IlrProgrammeType int not null");

            //Act
            Act(() => TestDataHelperDataLockEventsDatabase.Count(@"select
                 (select count(*) from [DataLockEvents].[ProcessorRun] where [IsInitialRun] = 0 and FinishTimeUtc is not null) -
                 (select count(*) from [DataLockEvents].[Provider] where HandledBy is not null)") == 1,
                pageSize: 2);

            //Assert
            var providers = TestDataHelperDataLockEventsDatabase.GetAllProviders();
            Assert.AreEqual(1, providers.Count);
            Assert.IsFalse(providers[0].RequiresInitialImport);
            Assert.IsNull(providers[0].HandledBy);

            var events = TestDataHelperDataLockEventsDatabase.GetAllEvents();
            var dataLocks = TestDataHelperDataLockEventsDatabase.GetAllLastDataLocks();

            Assert.AreEqual(2, events.Count);
            Assert.AreEqual("1", events[0].PriceEpisodeIdentifier);
            Assert.AreEqual("2", events[1].PriceEpisodeIdentifier);

            Assert.AreEqual(2, dataLocks.Count);
            Assert.AreEqual("1", dataLocks[0].PriceEpisodeIdentifier);
            Assert.AreEqual("2", dataLocks[1].PriceEpisodeIdentifier);








            // subsequent run shouldn't change a thing
            TestDataHelperDeds.Execute("update [dbo].[FileDetails] set [SubmittedTime] = @time", new { time = DateTime.Now });

            Act(() => TestDataHelperDataLockEventsDatabase.Count(@"select
                 (select count(*) from [DataLockEvents].[ProcessorRun] where [IsInitialRun] = 0 and FinishTimeUtc is not null) -
                 (select count(*) from [DataLockEvents].[Provider] where HandledBy is not null)") == 2,
                pageSize: 2);

            providers = TestDataHelperDataLockEventsDatabase.GetAllProviders();
            Assert.AreEqual(1, providers.Count);
            Assert.IsFalse(providers[0].RequiresInitialImport);
            Assert.IsNull(providers[0].HandledBy);

            events = TestDataHelperDataLockEventsDatabase.GetAllEvents();
            dataLocks = TestDataHelperDataLockEventsDatabase.GetAllLastDataLocks();

            Assert.AreEqual(2, events.Count);
            Assert.AreEqual("1", events[0].PriceEpisodeIdentifier);
            Assert.AreEqual("2", events[1].PriceEpisodeIdentifier);

            Assert.AreEqual(2, dataLocks.Count);
            Assert.AreEqual("1", dataLocks[0].PriceEpisodeIdentifier);
            Assert.AreEqual("2", dataLocks[1].PriceEpisodeIdentifier);









            // subsequent run after data fix should not produce duplicates
            TestDataHelperDataLockEventsDatabase.Execute("alter table DataLockEvents.LastDataLock alter column IlrProgrammeType int null");
            TestDataHelperDeds.Execute("update [dbo].[FileDetails] set [SubmittedTime] = @time", new { time = DateTime.Now });

            Act(() => TestDataHelperDataLockEventsDatabase.Count(@"select
                 (select count(*) from [DataLockEvents].[ProcessorRun] where [IsInitialRun] = 0 and FinishTimeUtc is not null) -
                 (select count(*) from [DataLockEvents].[Provider] where HandledBy is not null)") == 3,
                pageSize: 2);

            providers = TestDataHelperDataLockEventsDatabase.GetAllProviders();
            Assert.AreEqual(1, providers.Count);
            Assert.IsFalse(providers[0].RequiresInitialImport);
            Assert.IsNull(providers[0].HandledBy);

            events = TestDataHelperDataLockEventsDatabase.GetAllEvents();
            dataLocks = TestDataHelperDataLockEventsDatabase.GetAllLastDataLocks();

            Assert.AreEqual(4, events.Count);
            Assert.AreEqual("1", events[0].PriceEpisodeIdentifier);
            Assert.AreEqual("2", events[1].PriceEpisodeIdentifier);
            Assert.AreEqual("3", events[2].PriceEpisodeIdentifier);
            Assert.AreEqual("4", events[3].PriceEpisodeIdentifier);

            Assert.AreEqual(4, dataLocks.Count);
            Assert.AreEqual("1", dataLocks[0].PriceEpisodeIdentifier);
            Assert.AreEqual("2", dataLocks[1].PriceEpisodeIdentifier);
            Assert.AreEqual("3", dataLocks[2].PriceEpisodeIdentifier);
            Assert.AreEqual("4", dataLocks[3].PriceEpisodeIdentifier);
        }

    }
}
