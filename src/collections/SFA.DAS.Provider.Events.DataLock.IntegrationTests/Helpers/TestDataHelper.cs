using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Dapper;
using SFA.DAS.Payments.DCFS.Domain;
using SFA.DAS.Payments.DCFS.Extensions;
using SFA.DAS.Provider.Events.DataLock.Domain;
using SFA.DAS.Provider.Events.DataLock.IntegrationTests.TestContext;

namespace SFA.DAS.Provider.Events.DataLock.IntegrationTests.Helpers
{
    public class TestDataHelper
    {
        private static readonly string[] PeriodEndCopyReferenceDataScripts =
        {
            "01 datalock.PreRun.DataLockEvents.PopulateDataLockEventsData.sql",
            "02 datalock.populate.reference.history.sql",
            "03 datalock.populate.reference.datalockeventperiods.sql",
            "04 datalock.populate.reference.datalockeventerrors.sql",
            "05 datalock.populate.reference.datalockeventcommitmentversions.sql",
        };

        private static readonly string[] SubmissionCopyReferenceDataScripts = PeriodEndCopyReferenceDataScripts;

        internal static void Clean()
        {
            Clean(true);
            Clean(true, inSubmission: false);
            Clean(false);
        }

        private static void Clean(bool inTransient, bool inSubmission = true)
        {
            Execute(@"DECLARE @SQL NVARCHAR(MAX) = ''

                    SELECT @SQL = (
                        SELECT 'TRUNCATE TABLE [' + s.name + '].[' + o.name + ']' + CHAR(13)
                        FROM sys.objects o WITH (NOWAIT)
                        JOIN sys.schemas s WITH (NOWAIT) ON o.[schema_id] = s.[schema_id]
                        WHERE o.[type] = 'U'
                            AND s.name IN ('dbo', 'Valid', 'Reference', 'DataLockEvents', 'DataLock', 'Rulebase', 'Payments')
                        FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)')

                    EXEC sys.sp_executesql @SQL                
                ", inTransient: inTransient, inSubmission: inSubmission);
        }

        internal static void PeriodEndCopyReferenceData()
        {
            using (var connection = new SqlConnection(GlobalTestContext.Current.TransientPeriodEndDatabaseConnectionString))
            {
                connection.Open();
                try
                {
                    foreach (var script in PeriodEndCopyReferenceDataScripts)
                    {
                        var path = Path.Combine(GlobalTestContext.Current.AssemblyDirectory,
                            @"DbSetupScripts\Copy Reference Data", script);
                        connection.RunSqlScriptFile(path, GlobalTestContext.Current.DedsDatabaseNameBracketed);
                    }
                }
                finally
                {
                    connection.Close();
                }
            }

        }

        internal static void SubmissionCopyReferenceData()
        {
            using (var connection = new SqlConnection(GlobalTestContext.Current.TransientSubmissionDatabaseConnectionString))
            {
                connection.Open();
                try
                {
                    foreach (var script in SubmissionCopyReferenceDataScripts)
                    {
                        var path = Path.Combine(GlobalTestContext.Current.AssemblyDirectory,
                            @"DbSetupScripts\Copy Reference Data", script);
                        connection.RunSqlScriptFile(path, GlobalTestContext.Current.DedsDatabaseNameBracketed);
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        internal static void AddLearningProvider(long ukprn)
        {
            Execute("INSERT INTO Valid.LearningProvider (UKPRN) VALUES (@ukprn)", new { ukprn });
        }

        internal static void PeriodEndAddLearningProvider(long ukprn)
        {
            Execute("INSERT INTO Reference.Providers (Ukprn, IlrFilename, IlrSubmissionDateTime) VALUES (@ukprn, 'ILR-{ukprn}-1617-20161013-092500-98', @submissionDate)", new { ukprn, submissionDate = DateTime.Today }, inSubmission: false);
        }

        internal static void SetCurrentPeriodEnd()
        {
            Execute("INSERT INTO Reference.CollectionPeriods (Id,Name,CalendarMonth,CalendarYear,[Open]) Values(1, 'R11','" + DateTime.Now.Month + "','" + DateTime.Now.Year + "',1)");
            Execute("INSERT INTO Reference.CollectionPeriods (Id,Name,CalendarMonth,CalendarYear,[Open]) Values(1, 'R11','" + DateTime.Now.Month + "','" + DateTime.Now.Year + "',1)", inSubmission: false);
        }

        internal static void AddFileDetails(long ukprn, bool successful = true)
        {
            Execute($"INSERT INTO dbo.FileDetails (UKPRN, FileName, SubmittedTime, Success) VALUES (@ukprn, 'ILR-{ukprn}-1617-20161013-092500-98', @submissionDate, @successful)",
                new { ukprn, submissionDate = DateTime.Today, successful });
        }

        internal static void AddCommitment(long id,
                                           long ukprn,
                                           string learnerRefNumber,
                                           int aimSequenceNumber = 1,
                                           long uln = 0L,
                                           DateTime startDate = default(DateTime),
                                           DateTime endDate = default(DateTime),
                                           decimal agreedCost = 15000m,
                                           long? standardCode = null,
                                           int? programmeType = null,
                                           int? frameworkCode = null,
                                           int? pathwayCode = null,
                                           bool passedDataLock = true)
        {
            AddCommitment(id, ukprn, learnerRefNumber, aimSequenceNumber, uln, startDate, endDate, agreedCost, standardCode, programmeType, frameworkCode, pathwayCode, passedDataLock, true);
        }

        internal static void PeriodEndAddCommitment(long id,
                                           long ukprn,
                                           string learnerRefNumber,
                                           int aimSequenceNumber = 1,
                                           long uln = 0L,
                                           DateTime startDate = default(DateTime),
                                           DateTime endDate = default(DateTime),
                                           decimal agreedCost = 15000m,
                                           long? standardCode = null,
                                           int? programmeType = null,
                                           int? frameworkCode = null,
                                           int? pathwayCode = null,
                                           bool passedDataLock = true)
        {
            AddCommitment(id, ukprn, learnerRefNumber, aimSequenceNumber, uln, startDate, endDate, agreedCost, standardCode, programmeType, frameworkCode, pathwayCode, passedDataLock, false);
        }

        private static void AddCommitment(long id,
                                           long ukprn,
                                           string learnerRefNumber,
                                           int aimSequenceNumber,
                                           long uln,
                                           DateTime startDate,
                                           DateTime endDate,
                                           decimal agreedCost,
                                           long? standardCode,
                                           int? programmeType,
                                           int? frameworkCode,
                                           int? pathwayCode,
                                           bool passedDataLock,
                                           bool inSubmission)
        {
            var minStartDate = new DateTime(2017, 4, 1);

            if (uln == 0)
            {
                uln = 123456;
            }

            if (!standardCode.HasValue && !frameworkCode.HasValue)
            {
                standardCode = 27;
            }

            if (startDate < minStartDate)
            {
                startDate = minStartDate;
            }

            if (endDate < startDate)
            {
                endDate = startDate.AddYears(1);
            }

            Execute("INSERT INTO Reference.DasCommitments " +
                    "(CommitmentId,VersionId,AccountId,Uln,Ukprn,StartDate,EndDate,AgreedCost,StandardCode,ProgrammeType,FrameworkCode,PathwayCode,PaymentStatus,PaymentStatusDescription,Priority,EffectiveFrom) " +
                    "VALUES " +
                    "(@id, 1, '123', @uln, @ukprn, @startDate, @endDate, @agreedCost, @standardCode, @programmeType, @frameworkCode, @pathwayCode, 1, 'Active', 1, @startDate)",
                    new { id, uln, ukprn, startDate, endDate, agreedCost, standardCode, programmeType, frameworkCode, pathwayCode }, inSubmission: inSubmission);

            var priceEpisodeIdentifier = $"99-99-99-{startDate.ToString("yyyy-MM-dd")}";

            Execute("INSERT INTO DataLock.PriceEpisodeMatch "
                    + "(Ukprn,LearnRefNumber,AimSeqNumber,CommitmentId,PriceEpisodeIdentifier,IsSuccess) "
                    + "VALUES "
                    + "(@ukprn,@learnerRefNumber,@aimSequenceNumber,@id,@priceEpisodeIdentifier,@isSuccess)",
                    new { id, ukprn, learnerRefNumber, aimSequenceNumber, priceEpisodeIdentifier, isSuccess = passedDataLock }, inSubmission: inSubmission);

            var censusDate = startDate.LastDayOfMonth();
            var period = 1;

            while (censusDate <= endDate && period <= 12)
            {
                foreach (var traxType in Enum.GetValues(typeof(TransactionType)))
                {
                    AddPriceEpisodePeriodMatch(id, ukprn, learnerRefNumber, aimSequenceNumber, priceEpisodeIdentifier, period, (int)traxType, passedDataLock, inSubmission);
                }

                censusDate = censusDate.AddMonths(1).LastDayOfMonth();
                period++;
            }

            if (endDate != endDate.LastDayOfMonth() && period <= 12)
            {
                foreach (var traxType in Enum.GetValues(typeof(TransactionType)))
                {
                    AddPriceEpisodePeriodMatch(id, ukprn, learnerRefNumber, aimSequenceNumber, priceEpisodeIdentifier, period, (int)traxType, passedDataLock, inSubmission);
                }
            }

            if (!passedDataLock)
            {
                Execute("INSERT INTO DataLock.ValidationError "
                      + "(Ukprn, LearnRefNumber, AimSeqNumber, RuleId, PriceEpisodeIdentifier) "
                      + "VALUES "
                      + "(@ukprn, @learnerRefNumber, @aimSequenceNumber, 'DLOCK_07', @priceEpisodeIdentifier)",
                      new { id, ukprn, learnerRefNumber, aimSequenceNumber, priceEpisodeIdentifier }, inSubmission: inSubmission);
            }
        }

        private static void AddPriceEpisodePeriodMatch(long commitmentId,
                                                       long ukprn,
                                                       string learnerRefNumber,
                                                       int aimSequenceNumber,
                                                       string priceEpisodeIdentifier,
                                                       int period,
                                                       int transactionType,
                                                       bool payable,
                                                       bool inSubmission)
        {
            Execute("INSERT INTO DataLock.PriceEpisodePeriodMatch "
                  + "(Ukprn, PriceEpisodeIdentifier, LearnRefNumber, AimSeqNumber, CommitmentId, VersionId, Period, Payable, TransactionType) "
                  + "VALUES "
                  + "(@ukprn, @priceEpisodeIdentifier, @learnerRefNumber, @aimSequenceNumber, @commitmentId, 1, @period, @payable, @transactionType)",
                  new { commitmentId, ukprn, learnerRefNumber, aimSequenceNumber, priceEpisodeIdentifier, period, payable, transactionType }, inSubmission: inSubmission);
        }

        internal static void AddIlrDataForCommitment(long? commitmentId,
                                                     string learnerRefNumber,
                                                     int aimSequenceNumber = 1)
        {
            Execute("INSERT INTO Rulebase.AEC_ApprenticeshipPriceEpisode "
                    + "(LearnRefNumber, PriceEpisodeIdentifier, EpisodeEffectiveTNPStartDate, EpisodeStartDate, "
                    + "PriceEpisodeAimSeqNumber, PriceEpisodePlannedEndDate, PriceEpisodeTotalTNPPrice, TNP1, TNP2) "
                    + "SELECT "
                    + "@learnerRefNumber, "
                    + "'99-99-99-' + CONVERT(char(10), StartDate, 126), "
                    + "StartDate, "
                    + "StartDate, "
                    + "@aimSequenceNumber, "
                    + "EndDate, "
                    + "AgreedCost, "
                    + "AgreedCost * 0.8, "
                    + "AgreedCost * 0.2 "
                    + "FROM Reference.DasCommitments "
                    + "WHERE CommitmentId = @commitmentId",
                new { commitmentId, learnerRefNumber, aimSequenceNumber });

            Execute("INSERT INTO Valid.Learner "
                    + "(LearnRefNumber, ULN, Ethnicity, Sex, LLDDHealthProb) "
                    + "SELECT @learnerRefNumber,Uln,0,0,0 FROM Reference.DasCommitments "
                    + "WHERE CommitmentId = @commitmentId",
                new { commitmentId, learnerRefNumber });

            Execute("INSERT INTO Valid.LearningDelivery "
                    + "(LearnRefNumber, LearnAimRef, AimType, AimSeqNumber, LearnStartDate, LearnPlanEndDate, FundModel, StdCode, ProgType, FworkCode, PwayCode) "
                    + "SELECT @learnerRefNumber, 'ZPROG001', 1, @aimSequenceNumber, StartDate, EndDate, 36, StandardCode, ProgrammeType, FrameworkCode, PathwayCode FROM Reference.DasCommitments "
                    + "WHERE CommitmentId = @commitmentId",
                new { commitmentId, learnerRefNumber, aimSequenceNumber });
        }

        internal static void PeriodEndAddIlrDataForCommitment(long? commitmentId,
                                                              string learnerRefNumber,
                                                              int aimSequenceNumber = 1)
        {
            Execute("INSERT INTO Reference.DataLockPriceEpisode "
                    + "(Ukprn, LearnRefNumber, Uln, AimSeqNumber, StandardCode, ProgrammeType, FrameworkCode, PathwayCode, "
                    + "StartDate, NegotiatedPrice, PriceEpisodeIdentifier, EndDate, Tnp1, Tnp2, LearningStartDate) "
                    + "SELECT "
                    + "Ukprn, "
                    + "@learnerRefNumber, "
                    + "Uln, "
                    + "@aimSequenceNumber, "
                    + "StandardCode, "
                    + "ProgrammeType, "
                    + "FrameworkCode, "
                    + "PathwayCode, "
                    + "StartDate, "
                    + "AgreedCost, "
                    + "'99-99-99-' + CONVERT(char(10), StartDate, 126), "
                    + "EndDate, "
                    + "AgreedCost * 0.8, "
                    + "AgreedCost * 0.2, "
                    + "StartDate "
                    + "FROM Reference.DasCommitments "
                    + "WHERE CommitmentId = @commitmentId",
                new { commitmentId, learnerRefNumber, aimSequenceNumber }, inSubmission: false);
        }

        internal static void AddDataLockEvent(long ukprn,
                                            string learnerRefNumber,
                                            int aimSequenceNumber = 1,
                                            string priceEpisodeIdentifier = null,
                                            long uln = 0L,
                                            long commitmentId = 1,
                                            DateTime startDate = default(DateTime),
                                            DateTime endDate = default(DateTime),
                                            decimal agreedCost = 15000m,
                                            DateTime priceEffectiveFromDate = default(DateTime),
                                            long? standardCode = null,
                                            int? programmeType = null,
                                            int? frameworkCode = null,
                                            int? pathwayCode = null,
                                            bool passedDataLock = true)
        {
            var minStartDate = new DateTime(2017, 4, 1);

            if (uln == 0)
            {
                uln = 123456;
            }

            if (!standardCode.HasValue && !frameworkCode.HasValue)
            {
                standardCode = 27;
            }

            if (startDate < minStartDate)
            {
                startDate = minStartDate;
            }

            if (endDate < startDate)
            {
                endDate = startDate.AddYears(1);
            }

            if (priceEffectiveFromDate < startDate)
            {
                priceEffectiveFromDate = startDate;
            }

            if (priceEpisodeIdentifier == null)
            {
                priceEpisodeIdentifier = $"99-99-99-{startDate.ToString("yyyy-MM-dd")}";
            }
            var eventId = Guid.NewGuid();

            Execute("INSERT INTO DataLock.DataLockEvents "
                + "(DataLockEventId,ProcessDateTime, Status, IlrFileName, SubmittedDateTime, AcademicYear, UKPRN, ULN, LearnRefNumber, AimSeqNumber, "
                + "PriceEpisodeIdentifier, CommitmentId, EmployerAccountId, EventSource, HasErrors, IlrStartDate, IlrStandardCode, "
                + "IlrProgrammeType, IlrFrameworkCode, IlrPathwayCode, IlrTrainingPrice, IlrEndpointAssessorPrice, IlrPriceEffectiveFromDate) "
                + "VALUES "
                + $"(@eventId, @processed, 1, 'ILR-{ukprn}-1617-20161013-092500-98.xml', @submittedDateTime, '1617', @ukprn, @uln, @learnerRefNumber, @aimSequenceNumber, "
                + "@priceEpisodeIdentifier, @commitmentId, 123, 1, @hasErrors, @startDate, @standardCode, @programmeType, @frameworkCode, @pathwayCode, "
                + "@trainingCost, @endpointCost, @priceEffectiveFromDate)",
                new
                {
                    eventId,
                    processed = DateTime.Today,
                    submittedDateTime = DateTime.Now.AddDays(-1),
                    ukprn,
                    uln,
                    learnerRefNumber,
                    aimSequenceNumber,
                    priceEpisodeIdentifier,
                    commitmentId,
                    hasErrors = !passedDataLock,
                    startDate,
                    standardCode,
                    programmeType,
                    frameworkCode,
                    pathwayCode,
                    trainingCost = agreedCost * 0.8m,
                    endpointCost = agreedCost - agreedCost * 0.8m,
                    priceEffectiveFromDate
                }, false);

            var censusDate = startDate.LastDayOfMonth();
            var period = 1;

            while (censusDate <= endDate && period <= 12)
            {
                foreach (var traxType in Enum.GetValues(typeof(TransactionType)))
                {
                    AddDataLockEventPeriod(period, (int)traxType, passedDataLock, eventId);
                }

                censusDate = censusDate.AddMonths(1).LastDayOfMonth();
                period++;
            }

            if (endDate != endDate.LastDayOfMonth() && period <= 12)
            {
                foreach (var traxType in Enum.GetValues(typeof(TransactionType)))
                {
                    AddDataLockEventPeriod(period, (int)traxType, passedDataLock, eventId);
                }
            }

            if (!passedDataLock)
            {
                Execute("INSERT INTO DataLock.DataLockEventErrors "
                      + "(DataLockEventId, ErrorCode, SystemDescription) "
                      + "VALUES "
                      + "(@eventId, 'DLOCK_07', 'No matching record found in the employer digital account for the negotiated cost of training')", new { eventId }, inTransient: false);
            }

            Execute("INSERT INTO DataLock.DataLockEventCommitmentVersions "
                + "(DataLockEventId, CommitmentVersion, CommitmentStartDate, CommitmentStandardCode, CommitmentProgrammeType, "
                + "CommitmentFrameworkCode, CommitmentPathwayCode, CommitmentNegotiatedPrice, CommitmentEffectiveDate) "
                + "VALUES "
                + "(@eventId, 1, @startDate, @standardCode, @programmeType, @frameworkCode, @pathwayCode, @agreedCost, @startDate)",
                new { startDate, standardCode, programmeType, frameworkCode, pathwayCode, agreedCost, eventId }, false);
        }

        private static void AddDataLockEventPeriod(int period,
                                                int transactionType,
                                                bool payable,
                                                Guid dataLockEventId)
        {
            var collectionPeriod = GetCollectionPeriod(period);

            Execute("INSERT INTO DataLock.DataLockEventPeriods "
                + "(DataLockEventId, CollectionPeriodName, CollectionPeriodMonth, CollectionPeriodYear, CommitmentVersion, IsPayable, TransactionType) "
                + "VALUES "
                + "(@dataLockEventId, @name, @month, @year, 1, @payable, @transactionType)",
                new { name = collectionPeriod.Name, month = collectionPeriod.Month, year = collectionPeriod.Year, payable, transactionType, dataLockEventId }, false);
        }

        private static CollectionPeriod GetCollectionPeriod(int period)
        {
            var month = 0;
            var year = 2016;
            var name = string.Empty;

            switch (period)
            {
                case 1:
                    name = "R01";
                    month = 8;
                    break;
                case 2:
                    name = "R02";
                    month = 9;
                    break;
                case 3:
                    name = "R03";
                    month = 10;
                    break;
                case 4:
                    name = "R04";
                    month = 11;
                    break;
                case 5:
                    name = "R05";
                    month = 12;
                    break;
                case 6:
                    name = "R06";
                    month = 1;
                    year = 2017;
                    break;
                case 7:
                    name = "R07";
                    month = 2;
                    year = 2017;
                    break;
                case 8:
                    name = "R08";
                    month = 3;
                    year = 2017;
                    break;
                case 9:
                    name = "R09";
                    month = 4;
                    year = 2017;
                    break;
                case 10:
                    name = "R10";
                    month = 5;
                    year = 2017;
                    break;
                case 11:
                    name = "R11";
                    month = 6;
                    year = 2017;
                    break;
                case 12:
                    name = "R12";
                    month = 7;
                    year = 2017;
                    break;
            }

            return new CollectionPeriod
            {
                Name = $"1617-{name}",
                Month = month,
                Year = year
            };
        }

        internal static DataLockEvent[] GetAllEvents(bool inSubmission = true)
        {
            return Query<DataLockEvent>("SELECT * FROM DataLockEvents.DataLockEvents", inSubmission: inSubmission);
        }

        internal static DataLockEventError[] GetAllEventErrors(Guid eventId, bool inSubmission = true)
        {
            return Query<DataLockEventError>("SELECT * FROM DataLockEvents.DataLockEventErrors WHERE DataLockEventId = @eventId", new { eventId }, inSubmission);
        }

        internal static DataLockEventPeriod[] GetAllEventPeriods(Guid eventId, bool inSubmission = true)
        {
            return Query<DataLockEventPeriod>("SELECT * FROM DataLockEvents.DataLockEventPeriods WHERE DataLockEventId = @eventId", new { eventId }, inSubmission);
        }

        internal static DataLockEventCommitmentVersion[] GetAllEventCommitmentVersions(Guid eventId, bool inSubmission = true)
        {
            return Query<DataLockEventCommitmentVersion>("SELECT * FROM DataLockEvents.DataLockEventCommitmentVersions WHERE DataLockEventId = @eventId", new { eventId }, inSubmission);
        }

        private static void Execute(string command, object param = null, bool inTransient = true, bool inSubmission = true)
        {
            var connectionString = inTransient
                ? (inSubmission
                    ? GlobalTestContext.Current.TransientSubmissionDatabaseConnectionString
                    : GlobalTestContext.Current.TransientPeriodEndDatabaseConnectionString)
                : GlobalTestContext.Current.DedsDatabaseConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                try
                {
                    connection.Execute(command, param);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private static T[] Query<T>(string command, object param = null, bool inSubmission = true)
        {
            var connectionString = inSubmission
                ? GlobalTestContext.Current.TransientSubmissionDatabaseConnectionString
                : GlobalTestContext.Current.TransientPeriodEndDatabaseConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                try
                {
                    return connection.Query<T>(command, param)?.ToArray();
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}