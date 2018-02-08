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
                parameters.Add("totalPages", dbType: DbType.Int64, direction: ParameterDirection.Output);

                var entities = await connection.QueryAsync<DataLockEventEntity>("GetProviders", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

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

        public async Task<PageOfResults<DataLockEntity>> GetLastDataLocks(long ukprn, int page, int pageSize)
        {
            using (var connection = new SqlConnection(CloudConfigurationManager.GetSetting(_connectionStringName)))
            {
                var parameters = new DynamicParameters();
                parameters.Add("ukprn", ukprn);
                parameters.Add("offset", (page-1)*pageSize);
                parameters.Add("pageSize", pageSize);
                parameters.Add("totalPages", dbType: DbType.Int64, direction: ParameterDirection.Output);

                var entities = await connection.QueryAsync<DataLockEntity>("GetDataLocks", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

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

        private async Task WriteBulk<T>(IList<T> batch, string destination)
        {
            var columns = typeof(T).GetProperties().Select(p => p.Name).ToArray();

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
