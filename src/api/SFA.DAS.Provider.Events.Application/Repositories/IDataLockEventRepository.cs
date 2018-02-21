using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data.Entities;

namespace SFA.DAS.Provider.Events.Application.Repositories
{
    public interface IDataLockEventRepository
    {
        // from web api
        Task<PageOfResults<DataLockEventEntity>> GetDataLockEvents(long? sinceEventId, DateTime? sinceTime, string employerAccountId, long? ukprn , int page, int pageSize);


        // from worker
        Task<IList<ProviderEntity>> GetProviders();
        Task UpdateProvider(ProviderEntity providers);

        Task<PageOfResults<DataLockEntity>> GetLastDataLocks(long ukprn, int page, int pageSize);

        Task WriteDataLocks(IList<DataLockEntity> dataLocks);
        Task UpdateDataLocks(IList<DataLockEntity> dataLocks);

        Task WriteDataLockEvents(IList<DataLockEventEntity> events);
        Task<bool> HasInitialRunRecord();

        Task WriteProviders(IList<ProviderEntity> providers);

        Task<int> InsertOrUpdateProcessRunRecord(int? id, long? ukprn, DateTime? ilrSubmissionDateTime, DateTime? startTimeUtc, DateTime? finishTimeUtc, bool? isInitialRun, bool? isSuccess, string error);

        Task SetProviderProcessor(long ukprn, int runId);
        Task ClearProviderProcessor(long ukprn);
    }
}
