using System;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.DataLockEventWorker.AcceptanceTests.Specs
{
    [TestFixture]
    public class WhenInitialRunGoesWrong : DataLockProcessorTestBase
    {
        private const long Ukprn = 10000666;
        private const long CommitmentId = 6;
        private const string LearnerRefNumber = "Lrn-666";

        [Test]
        public void AndGetDataLocksFailsThenInitialRunFlagShouldStay()
        {
            //Arrange
            TestDataHelperDeds.AddProvider(Ukprn, DateTime.Today);
            TestDataHelperDeds.AddDataLock(new[] {CommitmentId}, Ukprn, LearnerRefNumber, priceEpisodeIdentifier: "3", errorCodesCsv: "E1");
            TestDataHelperDeds.AddDataLockEvent(Ukprn, DateTime.Today, EventStatus.Removed, 1, "1", 1, DateTime.Today.AddDays(-1), null);
            TestDataHelperDeds.Execute("update DataLock.ValidationError set RuleId = '\"' where RuleId = 'E1'");

            //Act
            Act(() => TestDataHelperDataLockEventsDatabase.Count("select count(*) from [DataLockEvents].[ProcessorRun] where [IsInitialRun] = 1 and FinishTimeUtc is not null") > 0);

            //Assert
            var providers = TestDataHelperDataLockEventsDatabase.GetAllProviders();
            Assert.AreEqual(1, providers.Count);
            Assert.IsTrue(providers[0].RequiresInitialImport);
            Assert.IsNull(providers[0].HandledBy);

            Assert.AreEqual(0, TestDataHelperDataLockEventsDatabase.GetAllEvents().Count);
            Assert.AreEqual(0, TestDataHelperDataLockEventsDatabase.GetAllLastDataLocks().Count);
        }

        [Test]
        public void AndWriteDataLocksFailsAfterFirstPageThenInitialRunFlagShouldStayAndDataCleared()
        {
            //Arrange
            // write data lock events should fail due to LarnRefNumber too long
            TestDataHelperDeds.Execute("alter table [DataLock].[PriceEpisodeMatch] alter column [LearnRefNumber] varchar(200) not null");
            TestDataHelperDeds.Execute("alter table [DataLock].[ValidationError] alter column [LearnRefNumber] varchar(200) not null");
            TestDataHelperDeds.Execute("alter table [Rulebase].[AEC_ApprenticeshipPriceEpisode] alter column [LearnRefNumber] varchar(200) not null");
            TestDataHelperDeds.Execute("alter table [Valid].[Learner] alter column [LearnRefNumber] varchar(200) not null");
            TestDataHelperDeds.Execute("alter table [Valid].[LearningDelivery] alter column [LearnRefNumber] varchar(200) not null");

            TestDataHelperDeds.AddProvider(Ukprn, DateTime.Today);
            TestDataHelperDeds.AddDataLock(new[] {CommitmentId}, Ukprn, LearnerRefNumber, priceEpisodeIdentifier: "1", errorCodesCsv: "E1");
            TestDataHelperDeds.AddDataLock(new[] {CommitmentId}, Ukprn, LearnerRefNumber, priceEpisodeIdentifier: "2", errorCodesCsv: "E1");
            TestDataHelperDeds.AddDataLock(new[] {CommitmentId}, Ukprn, new string('X', 200), priceEpisodeIdentifier: "3", errorCodesCsv: "E1");
            TestDataHelperDeds.AddDataLockEvent(Ukprn, DateTime.Today, EventStatus.Removed, 1, "1", 1, DateTime.Today.AddDays(-1), null);

            //Act
            Act(() => TestDataHelperDataLockEventsDatabase.Count("select count(*) from [DataLockEvents].[ProcessorRun] where [IsInitialRun] = 1 and FinishTimeUtc is not null") > 0
                      &&
                      TestDataHelperDataLockEventsDatabase.Count("select count(*) from [DataLockEvents].[Provider] where HandledBy is not null") == 0,
                pageSize: 2);

            //Assert
            var providers = TestDataHelperDataLockEventsDatabase.GetAllProviders();
            Assert.AreEqual(1, providers.Count);
            Assert.IsTrue(providers[0].RequiresInitialImport);
            Assert.IsNull(providers[0].HandledBy);

            Assert.AreEqual(0, TestDataHelperDataLockEventsDatabase.GetAllEvents().Count);
            Assert.AreEqual(0, TestDataHelperDataLockEventsDatabase.GetAllLastDataLocks().Count);
        }

        [Test]
        public void AndGetDataLockEventsFailsThenInitialRunFlagShouldStayAndDataCleared()
        {
            //Arrange
            TestDataHelperDeds.AddProvider(Ukprn, DateTime.Today);
            TestDataHelperDeds.AddDataLock(new[] {CommitmentId}, Ukprn, LearnerRefNumber, priceEpisodeIdentifier: "3", errorCodesCsv: "E1");
            TestDataHelperDeds.AddDataLockEvent(Ukprn, DateTime.Today, EventStatus.Removed, 1, "1", 1, DateTime.Today.AddDays(-1), null);
            TestDataHelperDeds.AddDataLockEvent(Ukprn, DateTime.Today, EventStatus.Updated, 2, "2", 2, DateTime.Today.AddDays(-1), new[] {"E1", "E2"});
            TestDataHelperDeds.Execute("update [DataLock].[DataLockEventErrors] set [ErrorCode] = '\"' where [ErrorCode] = 'E1'");

            //Act
            Act(() => TestDataHelperDataLockEventsDatabase.Count("select count(*) from [DataLockEvents].[ProcessorRun] where [IsInitialRun] = 1 and FinishTimeUtc is not null") > 0
                      &&
                      TestDataHelperDataLockEventsDatabase.Count("select count(*) from [DataLockEvents].[Provider] where HandledBy is not null") == 0);

            //Assert
            var providers = TestDataHelperDataLockEventsDatabase.GetAllProviders();
            Assert.AreEqual(1, providers.Count);
            Assert.IsTrue(providers[0].RequiresInitialImport);
            Assert.IsNull(providers[0].HandledBy);

            Assert.AreEqual(0, TestDataHelperDataLockEventsDatabase.GetAllEvents().Count);
            Assert.AreEqual(0, TestDataHelperDataLockEventsDatabase.GetAllLastDataLocks().Count);
        }

        [Test]
        public void AndWriteDataLockEventsFailsAfterFirstPageThenInitialRunFlagShouldStayAndDataCleared()
        {
            //Arrange
            // write data lock events should fail due to long value
            TestDataHelperDeds.Execute("alter table [DataLock].[DataLockEvents] alter column [IlrFileName] nvarchar(51) not null");

            TestDataHelperDeds.AddProvider(Ukprn, DateTime.Today);
            TestDataHelperDeds.AddDataLock(new[] {CommitmentId}, Ukprn, LearnerRefNumber, priceEpisodeIdentifier: "3", errorCodesCsv: "E1");
            TestDataHelperDeds.AddDataLockEvent(Ukprn, DateTime.Today, EventStatus.Removed, 1, "1", 1, DateTime.Today.AddDays(-1), null);
            TestDataHelperDeds.AddDataLockEvent(Ukprn, DateTime.Today, EventStatus.Updated, 2, "2", 2, DateTime.Today.AddDays(-1), new[] {"E1", "E2"});
            TestDataHelperDeds.AddDataLockEvent(Ukprn, DateTime.Today, EventStatus.Removed, 3, "3", 1, DateTime.Today.AddDays(-1), null);
            TestDataHelperDeds.Execute("update [DataLock].[DataLockEvents] set [IlrFileName] = '" + new string('X', 51) + "' where [IlrFileName] = '3'");

            //Act
            Act(() => TestDataHelperDataLockEventsDatabase.Count("select count(*) from [DataLockEvents].[ProcessorRun] where [IsInitialRun] = 1 and FinishTimeUtc is not null") > 0
                      &&
                      TestDataHelperDataLockEventsDatabase.Count("select count(*) from [DataLockEvents].[Provider] where HandledBy is not null") == 0,
                pageSize: 2);

            //Assert
            var providers = TestDataHelperDataLockEventsDatabase.GetAllProviders();
            Assert.AreEqual(1, providers.Count);
            Assert.IsTrue(providers[0].RequiresInitialImport);
            Assert.IsNull(providers[0].HandledBy);

            Assert.AreEqual(0, TestDataHelperDataLockEventsDatabase.GetAllEvents().Count);
            Assert.AreEqual(0, TestDataHelperDataLockEventsDatabase.GetAllLastDataLocks().Count);
        }

        //[Test]
        //public void AndUpdateProvideFailsThenDataShouldGetCleared()
        //{
        //    Assert.Fail("TODO");
        //}
    }
}
