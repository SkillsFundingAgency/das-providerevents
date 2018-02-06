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
                return (await connection.QueryAsync<ProviderEntity>("GetProviders", commandType: CommandType.StoredProcedure).ConfigureAwait(false)).ToList();
            }
        }

        public async Task<PageOfResults<DataLock>> GetDataLocks(long ukprn, int page, int pageSize)
        {
            using (var connection = await GetOpenConnection().ConfigureAwait(false))
            {
                var entities = await connection.QueryAsync<DataLock>("GetDataLocks", new {ukprn, page, pageSize}, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
                return new PageOfResults<DataLock>
                {
                    Items = entities.ToArray(),
                    PageNumber = page,
                    TotalNumberOfPages = -1
                };
            }
        }
    }
}
