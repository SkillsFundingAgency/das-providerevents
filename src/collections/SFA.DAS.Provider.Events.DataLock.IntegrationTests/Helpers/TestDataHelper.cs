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
            "01 datalock.populate.reference.provider.periodend.sql",
            "02 datalock.populate.reference.input.periodend.sql",
            "03 datalock.populate.reference.history.sql"
        };

        private static readonly string[] SubmissionCopyReferenceDataScripts =
        {
            "01 datalock.populate.reference.provider.submission.sql",
            "02 datalock.populate.reference.input.submission.sql",
            "03 datalock.populate.reference.history.sql"
        };

        internal static void Clean()
        {
            Clean(true);
            Clean(false);
        }

        private static void Clean(bool inTransient)
        {
            Execute(@"DECLARE @SQL NVARCHAR(MAX) = ''

                    SELECT @SQL = (
                        SELECT 'TRUNCATE TABLE [' + s.name + '].[' + o.name + ']' + CHAR(13)
                        FROM sys.objects o WITH (NOWAIT)
                        JOIN sys.schemas s WITH (NOWAIT) ON o.[schema_id] = s.[schema_id]
                        WHERE o.[type] = 'U'
                            AND s.name IN ('dbo', 'Valid', 'Reference', 'DataLock', 'Rulebase', 'Payments')
                        FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)')

                    EXEC sys.sp_executesql @SQL                
                ", inTransient: inTransient);
        }

        internal static void PeriodEndCopyReferenceData()
        {
            using (var connection = new SqlConnection(GlobalTestContext.Current.TransientDatabaseConnectionString))
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
            using (var connection = new SqlConnection(GlobalTestContext.Current.TransientDatabaseConnectionString))
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

        internal static void AddFileDetails(long ukprn, bool successful = true)
        {
            Execute($"INSERT INTO dbo.FileDetails (UKPRN, FileName, SubmittedTime, Success) VALUES (@ukprn, 'ILR-{ukprn}-1617-20161013-092500-98', @submissionDate, @successful)",
                new { ukprn, submissionDate = DateTime.Today, successful }, false);
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

            Execute("INSERT INTO dbo.DasCommitments " +
                    "(CommitmentId,VersionId,AccountId,Uln,Ukprn,StartDate,EndDate,AgreedCost,StandardCode,ProgrammeType,FrameworkCode,PathwayCode,PaymentStatus,PaymentStatusDescription,Priority,EffectiveFromDate) " +
                    "VALUES " +
                    "(@id, 1, '123', @uln, @ukprn, @startDate, @endDate, @agreedCost, @standardCode, @programmeType, @frameworkCode, @pathwayCode, 1, 'Active', 1, @startDate)",
                    new { id, uln, ukprn, startDate, endDate, agreedCost, standardCode, programmeType, frameworkCode, pathwayCode }, false);

            var priceEpisodeIdentifier = $"99-99-99-{startDate.ToString("yyyy-MM-dd")}";

            Execute("INSERT INTO DataLock.PriceEpisodeMatch "
                    + "(Ukprn,LearnRefNumber,AimSeqNumber,CommitmentId,PriceEpisodeIdentifier,IsSuccess, CollectionPeriodName, CollectionPeriodMonth, CollectionPeriodYear) "
                    + "VALUES "
                    + "(@ukprn,@learnerRefNumber,@aimSequenceNumber,@id,@priceEpisodeIdentifier,@isSuccess, '1617-R09', 4, 2017)",
                    new { id, ukprn, learnerRefNumber, aimSequenceNumber, priceEpisodeIdentifier, isSuccess = passedDataLock }, false);

            var censusDate = startDate.LastDayOfMonth();
            var period = 1;

            while (censusDate <= endDate && period <= 12)
            {
                foreach (var traxType in Enum.GetValues(typeof(TransactionType)))
                {
                    AddPriceEpisodePeriodMatch(id, ukprn, learnerRefNumber, aimSequenceNumber, priceEpisodeIdentifier, period, (int)traxType, passedDataLock);
                }

                censusDate = censusDate.AddMonths(1).LastDayOfMonth();
                period++;
            }

            if (endDate != endDate.LastDayOfMonth() && period <= 12)
            {
                foreach (var traxType in Enum.GetValues(typeof(TransactionType)))
                {
                    AddPriceEpisodePeriodMatch(id, ukprn, learnerRefNumber, aimSequenceNumber, priceEpisodeIdentifier, period, (int)traxType, passedDataLock);
                }
            }

            if (!passedDataLock)
            {
                Execute("INSERT INTO DataLock.ValidationError "
                      + "(Ukprn, LearnRefNumber, AimSeqNumber, RuleId, PriceEpisodeIdentifier, CollectionPeriodName, CollectionPeriodMonth, CollectionPeriodYear) "
                      + "VALUES "
                      + "(@ukprn, @learnerRefNumber, @aimSequenceNumber, 'DLOCK_07', @priceEpisodeIdentifier, '1617-R09', 4, 2017)",
                      new { id, ukprn, learnerRefNumber, aimSequenceNumber, priceEpisodeIdentifier }, false);
            }
        }

        private static void AddPriceEpisodePeriodMatch(long commitmentId,
                                                       long ukprn,
                                                       string learnerRefNumber,
                                                       int aimSequenceNumber,
                                                       string priceEpisodeIdentifier,
                                                       int period,
                                                       int transactionType,
                                                       bool payable)
        {
            Execute("INSERT INTO DataLock.PriceEpisodePeriodMatch "
                  + "(Ukprn, PriceEpisodeIdentifier, LearnRefNumber, AimSeqNumber, CommitmentId, VersionId, Period, Payable, TransactionType, CollectionPeriodName, CollectionPeriodMonth, CollectionPeriodYear) "
                  + "VALUES "
                  + "(@ukprn, @priceEpisodeIdentifier, @learnerRefNumber, @aimSequenceNumber, @commitmentId, 1, @period, @payable, @transactionType, '1617-R09', 4, 2017)",
                  new { commitmentId, ukprn, learnerRefNumber, aimSequenceNumber, priceEpisodeIdentifier, period, payable, transactionType }, false);
        }

        internal static void AddIlrDataForCommitment(long? commitmentId,
                                                     string learnerRefNumber,
                                                     int aimSequenceNumber = 1)
        {
            Execute("INSERT INTO Rulebase.AEC_ApprenticeshipPriceEpisode "
                    + "(Ukprn, LearnRefNumber, PriceEpisodeIdentifier, EpisodeEffectiveTNPStartDate, EpisodeStartDate, "
                    + "PriceEpisodeAimSeqNumber, PriceEpisodePlannedEndDate, PriceEpisodeTotalTNPPrice, TNP1, TNP2) "
                    + "SELECT "
                    + "Ukprn, "
                    + "@learnerRefNumber, "
                    + "'99-99-99-' + CONVERT(char(10), StartDate, 126), "
                    + "StartDate, "
                    + "StartDate, "
                    + "@aimSequenceNumber, "
                    + "EndDate, "
                    + "AgreedCost, "
                    + "AgreedCost * 0.8, "
                    + "AgreedCost * 0.2 "
                    + "FROM dbo.DasCommitments "
                    + "WHERE CommitmentId = @commitmentId",
                new {commitmentId, learnerRefNumber, aimSequenceNumber}, false);

            Execute("INSERT INTO Valid.Learner "
                    + "(UKPRN, LearnRefNumber, ULN, Ethnicity, Sex, LLDDHealthProb) "
                    + "SELECT Ukprn, @learnerRefNumber,Uln,0,0,0 FROM dbo.DasCommitments "
                    + "WHERE CommitmentId = @commitmentId",
                new { commitmentId, learnerRefNumber }, false);

            Execute("INSERT INTO Valid.LearningDelivery "
                    + "(UKPRN, LearnRefNumber, LearnAimRef, AimType, AimSeqNumber, LearnStartDate, LearnPlanEndDate, FundModel, StdCode, ProgType, FworkCode, PwayCode) "
                    + "SELECT Ukprn, @learnerRefNumber, 'ZPROG001', 1, @aimSequenceNumber, StartDate, EndDate, 36, StandardCode, ProgrammeType, FrameworkCode, PathwayCode FROM dbo.DasCommitments "
                    + "WHERE CommitmentId = @commitmentId",
                new { commitmentId, learnerRefNumber, aimSequenceNumber }, false);
        }

        internal static void AddPeriodEndPeriod()
        {
            Execute("INSERT INTO Payments.Periods"
                + "(PeriodName, CalendarMonth, CalendarYear, AccountDataValidAt, CommitmentDataValidAt, CompletionDateTime) "
                + "VALUES "
                + "('1617-R09', 4, 2017, @validTime, @validTime, @completionTime)",
                new { validTime = DateTime.Today, completionTime = DateTime.Now }, false);
        }

        internal static void AddDataLockLastSeenSubmission(long ukprn, DateTime submittedDateTime)
        {
            Execute("INSERT INTO DataLock.DataLockLastSeenSubmissions "
                + "(Ukprn, SubmittedDateTime) "
                + "VALUES "
                + "(@ukprn, @submittedDateTime)",
                new { ukprn, submittedDateTime }, false);
        }

        internal static void AddDataLockEvent(long ukprn,
                                            string learnerRefNumber,
                                            int aimSequenceNumber = 1,
                                            long uln = 0L,
                                            long commitmentId = 1,
                                            DateTime startDate = default(DateTime),
                                            DateTime endDate = default(DateTime),
                                            decimal agreedCost = 15000m,
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

            var priceEpisodeIdentifier = $"99-99-99-{startDate.ToString("yyyy-MM-dd")}";

            Execute("INSERT INTO DataLock.DataLockEvents "
                + "(Id, ProcessDateTime, IlrFileName, SubmittedDateTime, AcademicYear, UKPRN, ULN, LearnRefNumber, AimSeqNumber, "
                + "PriceEpisodeIdentifier, CommitmentId, EmployerAccountId, EventSource, HasErrors, IlrStartDate, IlrStandardCode, "
                + "IlrProgrammeType, IlrFrameworkCode, IlrPathwayCode, IlrTrainingPrice, IlrEndpointAssessorPrice) "
                + "VALUES "
                + $"(1, @processed, 'ILR-{ukprn}-1617-20161013-092500-98', @submittedDateTime, '1617', @ukprn, @uln, @learnerRefNumber, @aimSequenceNumber, "
                + "@priceEpisodeIdentifier, @commitmentId, 123, 1, @hasErrors, @startDate, @standardCode, @programmeType, @frameworkCode, @pathwayCode, "
                + "@trainingCost, @endpointCost)",
                new
                {
                    processed = DateTime.Today, submittedDateTime = DateTime.Now.AddDays(-1), ukprn, uln, learnerRefNumber, aimSequenceNumber,
                    priceEpisodeIdentifier, commitmentId, hasErrors = !passedDataLock, startDate, standardCode, programmeType, frameworkCode, pathwayCode,
                    trainingCost = agreedCost * 0.8m, endpointCost = agreedCost - agreedCost * 0.8m
                }, false);

            var censusDate = startDate.LastDayOfMonth();
            var period = 1;

            while (censusDate <= endDate && period <= 12)
            {
                foreach (var traxType in Enum.GetValues(typeof(TransactionType)))
                {
                    AddDataLockEventPeriod(period, (int)traxType, passedDataLock);
                }

                censusDate = censusDate.AddMonths(1).LastDayOfMonth();
                period++;
            }

            if (endDate != endDate.LastDayOfMonth() && period <= 12)
            {
                foreach (var traxType in Enum.GetValues(typeof(TransactionType)))
                {
                    AddDataLockEventPeriod(period, (int)traxType, passedDataLock);
                }
            }

            if (!passedDataLock)
            {
                Execute("INSERT INTO DataLock.DataLockEventErrors "
                      + "(DataLockEventId, ErrorCode, SystemDescription) "
                      + "VALUES "
                      + "(1, 'DLOCK_07', 'DLOCK_07')", inTransient: false);
            }

            Execute("INSERT INTO DataLock.DataLockEventCommitmentVersions "
                + "(DataLockEventId, CommitmentVersion, CommitmentStartDate, CommitmentStandardCode, CommitmentProgrammeType, "
                + "CommitmentFrameworkCode, CommitmentPathwayCode, CommitmentNegotiatedPrice, CommitmentEffectiveDate) "
                + "VALUES "
                + "(1, 1, @startDate, @standardCode, @programmeType, @frameworkCode, @pathwayCode, @agreedCost, @startDate)",
                new { startDate, standardCode, programmeType, frameworkCode, pathwayCode, agreedCost }, false);
        }

        private static void AddDataLockEventPeriod(int period,
                                                int transactionType,
                                                bool payable)
        {
            var collectionPeriod = GetCollectionPeriod(period);

            Execute("INSERT INTO DataLock.DataLockEventPeriods "
                + "(DataLockEventId, CollectionPeriodName, CollectionPeriodMonth, CollectionPeriodYear, CommitmentVersion, IsPayable, TransactionType) "
                + "VALUES "
                + "(1, @name, @month, @year, 1, @payable, @transactionType)",
                new { name = collectionPeriod.Name, month = collectionPeriod.Month, year = collectionPeriod.Year, payable, transactionType }, false);
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

        internal static DataLockEvent[] GetAllEvents()
        {
            return Query<DataLockEvent>("SELECT * FROM DataLock.DataLockEvents");
        }

        internal static DataLockEventError[] GetAllEventErrors(long eventId)
        {
            return Query<DataLockEventError>("SELECT * FROM DataLock.DataLockEventErrors WHERE DataLockEventId = @eventId", new { eventId });
        }

        internal static DataLockEventPeriod[] GetAllEventPeriods(long eventId)
        {
            return Query<DataLockEventPeriod>("SELECT * FROM DataLock.DataLockEventPeriods WHERE DataLockEventId = @eventId", new { eventId });
        }

        internal static DataLockEventCommitmentVersion[] GetAllEventCommitmentVersions(long eventId)
        {
            return Query<DataLockEventCommitmentVersion>("SELECT * FROM DataLock.DataLockEventCommitmentVersions WHERE DataLockEventId = @eventId", new { eventId });
        }

        private static void Execute(string command, object param = null, bool inTransient = true)
        {
            var connectionString = inTransient
                ? GlobalTestContext.Current.TransientDatabaseConnectionString
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

        private static T[] Query<T>(string command, object param = null)
        {
            using (var connection = new SqlConnection(GlobalTestContext.Current.TransientDatabaseConnectionString))
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