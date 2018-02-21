using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FastMember;
using Microsoft.Azure;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data.Entities;
using SFA.DAS.Provider.Events.Application.Repositories;

namespace SFA.DAS.Provider.Events.Infrastructure.Data
{
    public class DataLockEventRepository : IDataLockEventRepository
    {
        private readonly string _connectionStringName;

        public DataLockEventRepository() : this("DataLockEventConnectionString")
        {
        }

        public DataLockEventRepository(string connectionStringName)
        {
            _connectionStringName = connectionStringName;
        }

        public async Task<PageOfResults<DataLockEventEntity>> GetDataLockEvents(long? sinceEventId, DateTime? sinceTime, string employerAccountId, long? ukprn, int page, int pageSize)
        {
            using (var connection = new SqlConnection(CloudConfigurationManager.GetSetting(_connectionStringName)))
            {
                var parameters = new DynamicParameters();
                parameters.Add("sinceEventId", sinceEventId);
                parameters.Add("sinceTime", sinceTime);
                parameters.Add("employerAccountId", employerAccountId);
                parameters.Add("ukprn", ukprn);
                parameters.Add("offset", (page-1)*pageSize);
                parameters.Add("pageSize", pageSize);
                parameters.Add("totalPages", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var entities = await connection.QueryAsync<DataLockEventEntity>("[DataLockEvents].[GetDataLockEvents]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

                return new PageOfResults<DataLockEventEntity>
                {
                    Items = entities.ToArray(),
                    PageNumber = page,
                    TotalNumberOfPages = parameters.Get<int>("totalPages")
                };
            }
        }

        public async Task<IList<ProviderEntity>> GetProviders()
        {
            using (var connection = new SqlConnection(CloudConfigurationManager.GetSetting(_connectionStringName)))
            {
                return (await connection.QueryAsync<ProviderEntity>("SELECT * FROM [DataLockEvents].[Provider]").ConfigureAwait(false)).ToList();
            }
        }

        public async Task UpdateProvider(ProviderEntity provider)
        {
            using (var connection = new SqlConnection(CloudConfigurationManager.GetSetting(_connectionStringName)))
            {
                await connection.ExecuteAsync("[DataLockEvents].[UpdateProvider]", new { provider.Ukprn, provider.IlrSubmissionDateTime, provider.RequiresInitialImport }, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            }            
        }

        public async Task<PageOfResults<DataLockEntity>> GetLastDataLocks(long ukprn, int page, int pageSize)
        {
            using (var connection = new SqlConnection(CloudConfigurationManager.GetSetting(_connectionStringName)))
            {
                var parameters = new DynamicParameters();
                parameters.Add("ukprn", ukprn);
                parameters.Add("page", page);
                parameters.Add("pageSize", pageSize);
                parameters.Add("totalPages", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var entities = await connection.QueryAsync<DataLockEntity>("[DataLockEvents].[GetDataLocks]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

                return new PageOfResults<DataLockEntity>
                {
                    Items = entities.ToArray(),
                    PageNumber = page,
                    TotalNumberOfPages = parameters.Get<int>("totalPages")
                };
            }
        }

        public async Task WriteDataLocks(IList<DataLockEntity> dataLocks)
        {
            await WriteBulk(dataLocks, "[DataLockEvents].[LastDataLock]");
        }

        public async Task UpdateDataLocks(IList<DataLockEntity> dataLocks)
        {
            using (var connection = new SqlConnection(CloudConfigurationManager.GetSetting(_connectionStringName)))
            {
                connection.Open();
                var parameters = new DataLockEntityTableValueParameter(dataLocks);
                await connection.ExecuteAsync("[DataLockEvents].[UpdateDataLocks]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            }            
        }

        public async Task WriteDataLockEvents(IList<DataLockEventEntity> events)
        {
            await WriteBulk(events, "[DataLockEvents].[DataLockEvent]");
        }

        public async Task<bool> HasInitialRunRecord()
        {
            using (var connection = new SqlConnection(CloudConfigurationManager.GetSetting(_connectionStringName)))
            {
                connection.Open();
                return await connection.ExecuteScalarAsync<int>("select count(*) from [DataLockEvents].[ProcessorRun] where [IsInitialRun] = 1").ConfigureAwait(false) > 0;
            }
        }

        public async Task WriteProviders(IList<ProviderEntity> providers)
        {
            await WriteBulk(providers, "[DataLockEvents].[Provider]", "IlrFileName");
        }

        public async Task<int> InsertOrUpdateProcessRunRecord(int? id, long? ukprn, DateTime? ilrSubmissionDateTime, DateTime? startTimeUtc, DateTime? finishTimeUtc, bool? isInitialRun, bool? isSuccess, string error)
        {
            using (var connection = new SqlConnection(CloudConfigurationManager.GetSetting(_connectionStringName)))
            {
                connection.Open();
                if (id.HasValue)
                {
                    await connection.ExecuteAsync(@"
                    update [DataLockEvents].[ProcessorRun] set 
                        [Ukprn] = isnull(@ukprn, [Ukprn]),
                        [IlrSubmissionDateTime] = isnull(@ilrSubmissionDateTime, [IlrSubmissionDateTime]),
                        [StartTimeUtc] = isnull(@startTimeUtc, [StartTimeUtc]),
                        [FinishTimeUtc] = isnull(@finishTimeUtc, [FinishTimeUtc]),
                        [IsInitialRun] = isnull(@isInitialRun, [IsInitialRun]),
                        [IsSuccess] = isnull(@isSuccess, [IsSuccess]),
                        [Error] = isnull(@error, [Error])                    
                    where Id = @id", new {id, ukprn, ilrSubmissionDateTime, startTimeUtc, finishTimeUtc, isInitialRun, isSuccess, error}).ConfigureAwait(false);
                    return id.Value;
                }

                return await connection.ExecuteScalarAsync<int>(@"
                    insert into [DataLockEvents].[ProcessorRun] ([Ukprn], [IlrSubmissionDateTime], [StartTimeUtc], [FinishTimeUtc], [IsInitialRun], [IsSuccess], [Error])
                        values (@ukprn, @ilrSubmissionDateTime, @startTimeUtc, @finishTimeUtc, @isInitialRun, @isSuccess, @error);
                    select scope_identity();",
                    new {ukprn, ilrSubmissionDateTime, startTimeUtc, finishTimeUtc, isInitialRun, isSuccess, error}).ConfigureAwait(false);

            }
        }

        public async Task SetProviderProcessor(long ukprn, int runId)
        {
            using (var connection = new SqlConnection(CloudConfigurationManager.GetSetting(_connectionStringName)))
            {
                connection.Open();
                await connection.ExecuteScalarAsync<int>("update [DataLockEvents].[Provider] set [HandledBy] = @runId where [Ukprn] = @ukprn", new { ukprn, runId }).ConfigureAwait(false);
            }
        }

        public async Task ClearProviderProcessor(long ukprn)
        {
            using (var connection = new SqlConnection(CloudConfigurationManager.GetSetting(_connectionStringName)))
            {
                connection.Open();
                await connection.ExecuteScalarAsync<int>("update [DataLockEvents].[Provider] set [HandledBy] = null where [Ukprn] = @ukprn", new { ukprn }).ConfigureAwait(false);
            }
        }

        private async Task WriteBulk<T>(IList<T> batch, string destination, params string[] excludeColumns)
        {
            var columns = typeof(T).GetProperties().Where(c => excludeColumns == null || Array.IndexOf(excludeColumns, c.Name) < 0).Select(p => p.Name).ToArray();

            using (var bcp = new SqlBulkCopy(CloudConfigurationManager.GetSetting(_connectionStringName)))
            {
                foreach (var column in columns)
                {
                    bcp.ColumnMappings.Add(column, column);
                }

                bcp.BulkCopyTimeout = 0;
                bcp.DestinationTableName = destination;
                bcp.BatchSize = 1000;

                using (var reader = ObjectReader.Create(batch, columns))
                {
                    await bcp.WriteToServerAsync(reader);
                }
            }
        }
    }
}
