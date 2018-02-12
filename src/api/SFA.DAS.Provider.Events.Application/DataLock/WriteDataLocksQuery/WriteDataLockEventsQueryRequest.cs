﻿using System;
using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.Provider.Events.Application.DataLock.WriteDataLocksQuery
{
    public class WriteDataLocksQueryRequest : IAsyncRequest<WriteDataLocksQueryResponse>
    {
        public IList<Api.Types.DataLock> NewDataLocks { get; set; }
        public IList<Api.Types.DataLock> UpdatedDataLocks { get; set; }
        public IList<Api.Types.DataLock> RemovedDataLocks { get; set; }
    }
}