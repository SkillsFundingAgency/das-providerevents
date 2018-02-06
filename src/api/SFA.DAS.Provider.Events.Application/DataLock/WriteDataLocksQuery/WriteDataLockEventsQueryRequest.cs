using System;
using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.Provider.Events.Application.DataLock.WriteDataLocksQuery
{
    public class WriteDataLocksQueryRequest : IAsyncRequest<WriteDataLocksQueryResponse>
    {
        public IList<Api.Types.DataLock> DataLocks { get; set; }
    }
}