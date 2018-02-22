using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using Dapper;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.DataLockEventWorker.AcceptanceTests
{
    internal static class TestDataHelperDeds
    {
        private static readonly string _connectionString = ConfigurationManager.AppSettings["MonthEndConnectionString"];

        public static void CreateDatabase()
        {
            if (_connectionString.Contains("ProviderEventsAT"))
            {
                using (var connection = new SqlConnection(_connectionString.Replace("ProviderEventsAT", "master")))
                    connection.Execute("if(db_id(N'ProviderEventsAT') IS NULL) create database [ProviderEventsAT]");
            }

            var scripts =
                Directory.GetFiles(
                    Path.Combine(
                        Path.GetDirectoryName(typeof(TestDataHelperDeds).Assembly.Location) ??
                        throw new InvalidOperationException("Failed to get assembly location path"), "DedsDbSetUp"),
                    "*.sql");

            using (var connection = new SqlConnection(_connectionString))
            {
                foreach (var script in scripts)
                {
                    var sql = File.ReadAllText(script).Split(new[] {"GO --split here"}, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var snippet in sql)
                    {
                        connection.Execute(snippet);
                    }
                }
            }
        }

        public static void Clean()
        {
            var script =
                Directory.GetFiles(
                    Path.Combine(
                        Path.GetDirectoryName(typeof(TestDataHelperDeds).Assembly.Location) ??
                        throw new InvalidOperationException("Failed to get assembly location path"), "DedsDbSetUp"),
                    "Cleanup.sql")[0];

            using (var connection = new SqlConnection(_connectionString))
                connection.Execute(File.ReadAllText(script));
        }

        public static void AddProvider(long ukprn, DateTime ilrSubmissionDate)
        {
            Execute("insert into [Valid].[LearningProvider] (UKPRN) values (@ukprn)", new { ukprn });
            Execute("insert into [dbo].[FileDetails] (UKPRN, SubmittedTime, Filename) values (@ukprn, @ilrSubmissionDate, cast(@ilrSubmissionDate as sysname) + '.xml')", new { ukprn, ilrSubmissionDate });
        }

        public static void AddDataLock(long[] commitmentIds,
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
            string errorCodesCsv = null,
            string priceEpisodeIdentifier = null,
            long? employerAccountId = 77,
            DateTime? effectiveFromDate = null,
            DateTime? effectiveToDate = null,
            decimal totalTrainingPrice = 0,
            decimal totalEndpointAssessorPrice = 0,
            string learnAimRef = "60051255")
        {
            var minStartDate = new DateTime(2017, 4, 1);

            if (!effectiveFromDate.HasValue)
                effectiveFromDate = DateTime.Today;

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

            if (priceEpisodeIdentifier == null)
                priceEpisodeIdentifier = $"99-99-99-{startDate:yyyy-MM-dd}";

            if (commitmentIds != null)
            {
                foreach (var id in commitmentIds)
                {
                    Execute(@"INSERT INTO [dbo].[DasCommitments]
                                   ([CommitmentId]
                                   ,[VersionId]
                                   ,[Uln]
                                   ,[Ukprn]
                                   ,[AccountId]
                                   ,[StartDate]
                                   ,[EndDate]
                                   ,[AgreedCost]
                                   ,[StandardCode]
                                   ,[ProgrammeType]
                                   ,[FrameworkCode]
                                   ,[PathwayCode]
                                   ,[PaymentStatus]
                                   ,[PaymentStatusDescription]
                                   ,[Priority]
                                   ,[EffectiveFromDate]
                                   ,[EffectiveToDate]
                                   ,[LegalEntityName])
                             VALUES
                                   (@id
                                   ,cast(@id as sysname) + '-' + cast(@id as sysname)
                                   ,@uln
                                   ,@ukprn
                                   ,@employerAccountId
                                   ,@startDate
                                   ,@endDate
                                   ,@agreedCost
                                   ,@standardCode
                                   ,@programmeType
                                   ,@frameworkCode
                                   ,@pathwayCode
                                   ,@paymentStatus
                                   ,@paymentStatusDescription
                                   ,@priority
                                   ,@effectiveFromDate
                                   ,@effectiveToDate
                                   ,'LegalEntityName')",
                        new {id, uln, ukprn, employerAccountId, startDate, endDate, agreedCost, standardCode, programmeType, frameworkCode, pathwayCode, paymentStatus = 0, paymentStatusDescription = "",
                            priority = 1, learnerRefNumber, aimSequenceNumber, priceEpisodeIdentifier, isSuccess = errorCodesCsv == null, effectiveFromDate, effectiveToDate});

                    Execute(@"INSERT INTO DataLock.PriceEpisodeMatch
                        (Ukprn,LearnRefNumber,AimSeqNumber,CommitmentId,PriceEpisodeIdentifier,IsSuccess)
                        VALUES
                        (@ukprn,@learnerRefNumber,@aimSequenceNumber,@id,@priceEpisodeIdentifier,@isSuccess)",
                        new {id, ukprn, learnerRefNumber, aimSequenceNumber, priceEpisodeIdentifier, isSuccess = errorCodesCsv == null});

                    if (errorCodesCsv != null)
                    {
                        foreach (var errorCode in errorCodesCsv.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries))
                        {
                            Execute(@"INSERT INTO DataLock.ValidationError
                              (Ukprn, LearnRefNumber, AimSeqNumber, RuleId, PriceEpisodeIdentifier)
                              VALUES
                              (@ukprn, @learnerRefNumber, @aimSequenceNumber, @errorCode, @priceEpisodeIdentifier)",
                                new {id, ukprn, learnerRefNumber, aimSequenceNumber, priceEpisodeIdentifier, errorCode});
                        }
                    }
                }
            }



            Execute(@"
                if not exists(select 1 from [Rulebase].[AEC_ApprenticeshipPriceEpisode] where Ukprn = @ukprn and LearnRefNumber = @learnerRefNumber and PriceEpisodeIdentifier = @priceEpisodeIdentifier)
                INSERT INTO [Rulebase].[AEC_ApprenticeshipPriceEpisode]
                           ([Ukprn]
                           ,[LearnRefNumber]
                           ,[PriceEpisodeIdentifier]
                           ,[PriceEpisodeAimSeqNumber]
                           ,[TNP1]
                           ,[TNP2]
                           ,[TNP3]
                           ,[TNP4]
                           ,[EpisodeEffectiveTNPStartDate]
                           ,[PriceEpisodeContractType])
                     VALUES
                           (@ukprn
                           ,@learnerRefNumber
                           ,@priceEpisodeIdentifier
                           ,@aimSequenceNumber
                           ,@totalTrainingPrice
                           ,@totalEndpointAssessorPrice
                           ,0
                           ,0
                           ,@effectiveFromDate
                           ,'Levy Contract')",
                new
                    {ukprn, learnerRefNumber, priceEpisodeIdentifier, effectiveFromDate, totalTrainingPrice, totalEndpointAssessorPrice, aimSequenceNumber});

            Execute(@"
                if not exists(select 1 from [Valid].[Learner] where Ukprn = @ukprn and LearnRefNumber = @learnerRefNumber)
                INSERT INTO [Valid].[Learner]
                           ([Ukprn]
                           ,[LearnRefNumber]
                           ,[Uln]
                           ,[Ethnicity]
                           ,[Sex]
                           ,[LLDDHealthProb]
                           )
                     VALUES
                           (@ukprn
                           ,@learnerRefNumber
                           ,@uln
                           ,0
                           ,'Y'
                           ,1)",
                new
                    {ukprn, learnerRefNumber, uln });

            Execute(@"
                if not exists(select 1 from [Valid].[LearningDelivery] where Ukprn = @ukprn and LearnRefNumber = @learnerRefNumber and AimSeqNumber = @aimSequenceNumber)
                INSERT INTO [Valid].[LearningDelivery]
                           ([Ukprn]
                           ,[LearnRefNumber]
                           ,[AimSeqNumber]
                           ,[LearnStartDate]
                           ,[StdCode]
                           ,[ProgType]
                           ,[FworkCode]
                           ,[PwayCode]
                           ,[LearnAimRef]
                           ,[AimType]
                           ,[LearnPlanEndDate]
                           ,[FundModel]
                           )
                     VALUES
                           (@ukprn
                           ,@learnerRefNumber
                           ,@aimSequenceNumber
                           ,@startDate
                           ,@standardCode
                           ,@programmeType
                           ,@frameworkCode
                           ,@pathwayCode
                           ,@learnAimRef
                           ,3
                           ,@endDate
                           ,36
                           )",
                new
                    {ukprn, learnerRefNumber, aimSequenceNumber, startDate, standardCode, programmeType, frameworkCode, pathwayCode, learnAimRef, endDate });
        }

        public static void AddDataLockEvent(long ukprn, DateTime processedDateTime, EventStatus status, string otherStrings, int otherNumbers, DateTime otherDates, string[] errors)
        {
            var id = Guid.NewGuid();
            Execute(@"
                INSERT INTO [DataLock].[DataLockEvents]
                           ([DataLockEventId]
                           ,[ProcessDateTime]
                           ,[IlrFileName]
                           ,[SubmittedDateTime]
                           ,[AcademicYear]
                           ,[UKPRN]
                           ,[ULN]
                           ,[LearnRefNumber]
                           ,[AimSeqNumber]
                           ,[PriceEpisodeIdentifier]
                           ,[CommitmentId]
                           ,[EmployerAccountId]
                           ,[EventSource]
                           ,[HasErrors]
                           ,[IlrStartDate]
                           ,[IlrStandardCode]
                           ,[IlrProgrammeType]
                           ,[IlrFrameworkCode]
                           ,[IlrPathwayCode]
                           ,[IlrTrainingPrice]
                           ,[IlrEndpointAssessorPrice]
                           ,[IlrPriceEffectiveFromDate]
                           ,[IlrPriceEffectiveToDate]
                           ,[Status])
                     VALUES
                           (@id
                           ,@processedDateTime
                           ,@otherStrings
                           ,@otherDates
                           ,'1617'
                           ,@ukprn
                           ,@otherNumbers
                           ,@otherStrings
                           ,@otherNumbers
                           ,@otherStrings
                           ,@otherNumbers
                           ,@otherNumbers
                           ,@otherNumbers
                           ,@hasErrors
                           ,@otherDates
                           ,@otherNumbers
                           ,@otherNumbers
                           ,@otherNumbers
                           ,@otherNumbers
                           ,@otherNumbers
                           ,@otherNumbers
                           ,@otherDates
                           ,@otherDates
                           ,@status)
                ", new {id, ukprn, otherDates, otherNumbers, otherStrings, processedDateTime, hasErrors = errors != null, status});

            if (errors != null)
            {
                foreach (var error in errors)
                {
                    Execute(@"
                        INSERT INTO [DataLock].[DataLockEventErrors]
                                   ([DataLockEventId]
                                   ,[ErrorCode]
                                   ,[SystemDescription])
                             VALUES
                                   (@id
                                   ,@error
                                   ,@error)", new {id, error});
                }
            }

        }

        private static void Execute(string command, object param = null)
        {
            using (var connection = new SqlConnection(_connectionString))
                connection.Execute(command, param);
        }
    }
}
