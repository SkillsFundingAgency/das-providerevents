using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Dapper;
using Microsoft.SqlServer.Dac;
using SFA.DAS.Provider.Events.Application.Data.Entities;

namespace SFA.DAS.Provider.Events.DataLockEventWorker.AcceptanceTests
{
    internal static class TestDataHelperDataLockEventsDatabase
    {
        static readonly string _connectionString = ConfigurationManager.AppSettings["DataLockEventConnectionString"];
        private const string DatabaseName = "ProviderEventsAT_Local";

        public static void CreateDatabase()
        {
            //if (_connectionString.Contains(DatabaseName))
            //{
            //    using (var connection = new SqlConnection(_connectionString.Replace(DatabaseName, "master")))
            //        connection.Execute($"if(db_id(N'{DatabaseName}') IS NOT NULL) drop database [{DatabaseName}]");
            //}

            var instance = new DacServices(_connectionString);
            var path = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(typeof(TestDataHelperDeds).Assembly.Location) ?? throw new InvalidOperationException("Failed to get assembly location path"),
                @"..\..\..\SFA.DAS.Provider.Events.DataLockEventsDatabase\bin\Debug\SFA.DAS.Provider.Events.DataLockEventsDatabase.dacpac"));

            using (var dacpac = DacPackage.Load(path))
            {
                instance.Deploy(dacpac, DatabaseName, true);
            }
        }

        public static int Count(string sql, object parameters = null)
        {
            using (var connection = new SqlConnection(_connectionString))
                return connection.ExecuteScalar<int>(sql, parameters);
        }

        public static IList<DataLockEventEntity> GetAllEvents()
        {
            using (var connection = new SqlConnection(_connectionString))
                return connection.Query<DataLockEventEntity>("select * from DataLockEvents.DataLockEvent").ToList();
        }

        public static void Clean()
        {
            const string sql = @"
                EXEC sys.sp_msforeachtable @command1='truncate table ?',@whereand='and o.id in(
                        SELECT t.object_id 
                        FROM SYS.DM_DB_PARTITION_STATS p JOIN SYS.TABLES t ON p.object_id = t.object_id 
                        WHERE p.index_id <= 1 
                        GROUP BY t.object_id 
                        HAVING SUM(p.row_count) > 0
                    )'";

            using (var connection = new SqlConnection(_connectionString))
                connection.Execute(sql);
        }

        public static void AddProvider(long ukprn, DateTime ilrSubmissionDateTime)
        {
            Execute(@"
                insert into [DataLockEvents].[Provider] ([Ukprn], [IlrSubmissionDateTime])
                values (@ukprn, @ilrSubmissionDateTime)
            ", new {ukprn, ilrSubmissionDateTime});
        }

        public static IList<ProviderEntity> GetAllProviders()
        {
            using (var connection = new SqlConnection(_connectionString))
                return connection.Query<ProviderEntity>("select * from DataLockEvents.Provider").ToList();
        }

        public static IList<DataLockEntity> GetAllLastDataLocks()
        {
            using (var connection = new SqlConnection(_connectionString))
                return connection.Query<DataLockEntity>("select * from DataLockEvents.LastDataLock").ToList();
        }

        private static void Execute(string command, object param = null)
        {
            using (var connection = new SqlConnection(_connectionString))
                connection.Execute(command, param);
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
            DateTime? deletedTime = null)
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

            if (priceEpisodeIdentifier == null)
                priceEpisodeIdentifier = $"99-99-99-{startDate:yyyy-MM-dd}";

            if (commitmentIds != null)
            {
                foreach (var id in commitmentIds)
                {
                    var errorCodes = errorCodesCsv == null ? null : string.Concat("[\"", string.Join("\",\"", errorCodesCsv.Split(new[]{"'"}, StringSplitOptions.RemoveEmptyEntries)), "\"]");

                    Execute(@"INSERT INTO DataLockEvents.LastDataLock
                        (Ukprn,[LearnerReferenceNumber],[AimSequenceNumber],[CommitmentId],PriceEpisodeIdentifier,ErrorCodes,Uln,[EmployerAccountId],[IlrStandardCode],[IlrFrameworkCode],[IlrProgrammeType],[IlrPathwayCode],[IlrPriceEffectiveFromDate],[IlrPriceEffectiveToDate],[DeletedUtc])
                        VALUES
                        (@ukprn,@learnerRefNumber,@aimSequenceNumber,@id,@priceEpisodeIdentifier,@errorCodes,@uln,@employerAccountId,@standardCode,@frameworkCode,@programmeType,@pathwayCode,@startDate,@endDate,@deletedTime)",
                        new {id, ukprn, learnerRefNumber, aimSequenceNumber, priceEpisodeIdentifier, errorCodes, uln, employerAccountId = 888, standardCode, frameworkCode, programmeType, pathwayCode, startDate, endDate, deletedTime});
                }
            }
        }

        public static void PopulateInitialRun(long ukprn = 10000, DateTime? ilrSubmissionDateTime = null)
        {
            Execute(@"
                insert into [DataLockEvents].[ProcessorRun] ([Ukprn], [IlrSubmissionDateTime], [StartTimeUtc], [FinishTimeUtc], [IsInitialRun], [IsSuccess])
                values (@ukprn, @ilrSubmissionDateTime, getdate(), getdate(), 1, 1)
            ", new {ukprn, ilrSubmissionDateTime = ilrSubmissionDateTime.GetValueOrDefault(DateTime.Today)});
        }
    }
}
