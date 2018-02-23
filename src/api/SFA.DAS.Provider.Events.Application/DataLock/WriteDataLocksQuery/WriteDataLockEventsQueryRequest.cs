using System.Collections.Generic;
using MediatR;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.Application.DataLock.WriteDataLocksQuery
{
    public class WriteDataLocksQueryRequest : IAsyncRequest<WriteDataLocksQueryResponse>
    {
        public IList<Api.Types.DataLock> NewDataLocks { get; set; }
        public IList<Api.Types.DataLock> UpdatedDataLocks { get; set; }
        public IList<Api.Types.DataLock> RemovedDataLocks { get; set; }
        public IList<DataLockEvent> DataLockEvents { get; set; }
    }
}