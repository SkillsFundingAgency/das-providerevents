using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using Dapper;
using NUnit.Framework;

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

            var scripts = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(typeof(TestDataHelperDeds).Assembly.Location), "DedsDbSetUp"), "*.sql");

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
            var sql = @"
                IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'DataLock') EXEC('CREATE SCHEMA DataLock')
                IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Valid') EXEC('CREATE SCHEMA Valid')
                IF OBJECT_ID('DataLock.PriceEpisodeMatch', 'U') IS NOT NULL 
					IF EXISTS(SELECT 1 FROM [DataLock].[PriceEpisodeMatch]) 
						DROP TABLE [DataLock].[PriceEpisodeMatch];
                IF OBJECT_ID('DataLock.ValidationError', 'U') IS NOT NULL 
					IF EXISTS(SELECT 1 FROM DataLock.ValidationError) 
						DROP TABLE DataLock.ValidationError; 
                IF OBJECT_ID('Valid.LearningProvider', 'U') IS NOT NULL 
					IF EXISTS(SELECT 1 FROM [Valid].[LearningProvider]) 
						DROP TABLE [Valid].[LearningProvider]; 
                IF OBJECT_ID('[dbo].[FileDetails]', 'U') IS NOT NULL 
					IF EXISTS(SELECT 1 FROM [dbo].[FileDetails]) 
						DROP TABLE [dbo].[FileDetails]; 

                IF OBJECT_ID('DataLock.PriceEpisodeMatch', 'U') IS NULL
                    CREATE TABLE DataLock.PriceEpisodeMatch
                    (
                        [Ukprn] bigint NOT NULL,
                        [PriceEpisodeIdentifier] varchar(25) NOT NULL,
                        [LearnRefNumber] varchar(100) NOT NULL,
                        [AimSeqNumber] bigint NOT NULL,
                        [CommitmentId] varchar(50) NOT NULL,
                        [CollectionPeriodName] varchar(8),
                        [CollectionPeriodMonth] int,
                        [CollectionPeriodYear] int,
	                    [IsSuccess] bit
                    );

                IF OBJECT_ID('DataLock.ValidationError', 'U') IS NULL
                    CREATE TABLE DataLock.ValidationError
                    (
                        [Ukprn] bigint,
                        [LearnRefNumber] varchar(12),
                        [AimSeqNumber] int,
                        [RuleId] varchar(50),
                        [PriceEpisodeIdentifier] varchar(25) NOT NULL
                    );

                IF OBJECT_ID('Valid.LearningProvider', 'U') IS NULL
                    create table [Valid].[LearningProvider] (
	                    [UKPRN] [bigint] NOT NULL PRIMARY KEY
                    );

                IF OBJECT_ID('dbo.FileDetails', 'U') IS NULL
                    CREATE TABLE [dbo].[FileDetails] (
                        [ID] [int] IDENTITY(1,1) PRIMARY KEY,
                        [UKPRN] [bigint] NOT NULL,
                        [Filename] [nvarchar](50) NULL,
                        [FileSizeKb] [bigint] NULL,
                        [TotalLearnersSubmitted] [int] NULL,
                        [TotalValidLearnersSubmitted] [int] NULL,
                        [TotalInvalidLearnersSubmitted] [int] NULL,
                        [TotalErrorCount] [int] NULL,
                        [TotalWarningCount] [int] NULL,
                        [SubmittedTime] [datetime] NULL,
                        [Success] [bit]
                        CONSTRAINT [PK_dbo.FileDetails] UNIQUE ([UKPRN], [Filename], [Success] ASC)
                    );";

            using (var connection = new SqlConnection(_connectionString))
                connection.Execute(sql);
        }

        public static void AddProvider(long ukprn, DateTime ilrSubmissionDate)
        {
            Execute("insert into [Valid].[LearningProvider] (UKPRN) values (@ukprn)", new { ukprn });
            Execute("insert into [dbo].[FileDetails] (UKPRN, SubmittedTime) values (@ukprn, @ilrSubmissionDate)", new { ukprn, ilrSubmissionDate });
        }

        public static void AddCommitment(long id,
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

            //Execute("INSERT INTO Reference.DasCommitments " +
            //        "(CommitmentId,VersionId,AccountId,Uln,Ukprn,StartDate,EndDate,AgreedCost,StandardCode,ProgrammeType,FrameworkCode,PathwayCode,PaymentStatus,PaymentStatusDescription,Priority,EffectiveFrom) " +
            //        "VALUES " +
            //        "(@id, 1, '123', @uln, @ukprn, @startDate, @endDate, @agreedCost, @standardCode, @programmeType, @frameworkCode, @pathwayCode, 1, 'Active', 1, @startDate)",
            //        new { id, uln, ukprn, startDate, endDate, agreedCost, standardCode, programmeType, frameworkCode, pathwayCode });

            var priceEpisodeIdentifier = $"99-99-99-{startDate.ToString("yyyy-MM-dd")}";

            Execute("INSERT INTO DataLock.PriceEpisodeMatch "
                    + "(Ukprn,LearnRefNumber,AimSeqNumber,CommitmentId,PriceEpisodeIdentifier,IsSuccess) "
                    + "VALUES "
                    + "(@ukprn,@learnerRefNumber,@aimSequenceNumber,@id,@priceEpisodeIdentifier,@isSuccess)",
                    new { id, ukprn, learnerRefNumber, aimSequenceNumber, priceEpisodeIdentifier, isSuccess = passedDataLock });

            //var censusDate = LastDayOfMonth(startDate);
            //var period = 1;

            //while (censusDate <= endDate && period <= 12)
            //{
            //    foreach (var traxType in Enum.GetValues(typeof(TransactionTypesFlag)))
            //    {
            //        AddPriceEpisodePeriodMatch(id, ukprn, learnerRefNumber, aimSequenceNumber, priceEpisodeIdentifier, period, (int)traxType, passedDataLock);
            //    }

            //    censusDate = LastDayOfMonth(censusDate.AddMonths(1));
            //    period++;
            //}

            //if (endDate != LastDayOfMonth(endDate) && period <= 12)
            //{
            //    foreach (var traxType in Enum.GetValues(typeof(TransactionTypesFlag)))
            //    {
            //        AddPriceEpisodePeriodMatch(id, ukprn, learnerRefNumber, aimSequenceNumber, priceEpisodeIdentifier, period, (int)traxType, passedDataLock);
            //    }
            //}

            if (!passedDataLock)
            {
                Execute("INSERT INTO DataLock.ValidationError "
                      + "(Ukprn, LearnRefNumber, AimSeqNumber, RuleId, PriceEpisodeIdentifier) "
                      + "VALUES "
                      + "(@ukprn, @learnerRefNumber, @aimSequenceNumber, 'DLOCK_07', @priceEpisodeIdentifier)",
                      new { id, ukprn, learnerRefNumber, aimSequenceNumber, priceEpisodeIdentifier });
            }
        }

        public static void AddIlrDataForCommitment(long? commitmentId,
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
                new {commitmentId, learnerRefNumber, aimSequenceNumber});

            Execute("INSERT INTO Valid.Learner "
                    + "(LearnRefNumber, ULN, Ethnicity, Sex, LLDDHealthProb) "
                    + "SELECT @learnerRefNumber,Uln,0,0,0 FROM Reference.DasCommitments "
                    + "WHERE CommitmentId = @commitmentId",
                new {commitmentId, learnerRefNumber});

            Execute("INSERT INTO Valid.LearningDelivery "
                    + "(LearnRefNumber, LearnAimRef, AimType, AimSeqNumber, LearnStartDate, LearnPlanEndDate, FundModel, StdCode, ProgType, FworkCode, PwayCode) "
                    + "SELECT @learnerRefNumber, 'ZPROG001', 1, @aimSequenceNumber, StartDate, EndDate, 36, StandardCode, ProgrammeType, FrameworkCode, PathwayCode FROM Reference.DasCommitments "
                    + "WHERE CommitmentId = @commitmentId",
                new {commitmentId, learnerRefNumber, aimSequenceNumber});
        }
        

        public static void AddDataLockValidationError(long ukprn, string learnRefNumber, long? aimSeqNumber, string priceEpisodeIdentifier, string ruleId)
        {

        }

        public static void AddDataLockPricePeriodMatch(long ukprn, string learnRefNumber, long? aimSeqNumber, string priceEpisodeIdentifier, long commitmentId, bool isSuccess)
        {

        }

        private static void Execute(string command, object param = null)
        {
            using (var connection = new SqlConnection(_connectionString))
                connection.Execute(command, param);
        }
    }
}
