using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data.Entities;
using SFA.DAS.Provider.Events.Application.Repositories;

namespace SFA.DAS.Provider.Events.Infrastructure.Data
{
    public class DcfsDataLockRepository : DcfsRepository, IDataLockRepository
    {
        public async Task<IList<ProviderEntity>> GetProviders()
        {
            using (var connection = await GetOpenConnection().ConfigureAwait(false))
            {
                return (await connection.QueryAsync<ProviderEntity>("DataLock.GetProviders", commandType: CommandType.StoredProcedure).ConfigureAwait(false)).ToList();
            }
        }

        public async Task<IList<DataLockEntity>> GetDataLocks(long ukprn, int page, int pageSize)
        {
            using (var connection = await GetOpenConnection().ConfigureAwait(false))
            {
                var dataLockEntities = (await connection.QueryAsync<DataLockEntity>("DataLock.GetDataLocks", new {ukprn, page, pageSize}, commandType: CommandType.StoredProcedure).ConfigureAwait(false)).ToList();
                return dataLockEntities;
            }
        }

        public async Task<PageOfResults<DataLockEventEntity>> GetDataLockEvents(long ukprn, int page, int pageSize)
        {
            using (var connection = await GetOpenConnection().ConfigureAwait(false))
            {
                var parameters = new DynamicParameters();
                parameters.Add("ukprn", ukprn);
                parameters.Add("offset", (page-1)*pageSize);
                parameters.Add("pageSize", pageSize);
                parameters.Add("totalPages", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var dataLockEntities = (await connection.QueryAsync<DataLockEventEntity>("DataLock.GetDataLockEvents", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false)).ToList();

                return new PageOfResults<DataLockEventEntity>
                {
                    Items = dataLockEntities.ToArray(),
                    PageNumber = page,
                    TotalNumberOfPages = parameters.Get<int>("totalPages")
                };
            }
        }
    }
}
