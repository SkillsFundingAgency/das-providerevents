using System.Collections.Generic;
using MediatR;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.Application.DataLock.WriteDataLockEventsQuery
{
    public class WriteDataLockEventsQueryRequest : IAsyncRequest<WriteDataLockEventsQueryResponse>
    {
        public IList<DataLockEvent> DataLockEvents { get; set; }
    }
}