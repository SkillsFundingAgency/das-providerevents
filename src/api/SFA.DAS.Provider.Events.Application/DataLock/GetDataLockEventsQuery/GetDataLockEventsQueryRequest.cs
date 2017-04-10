using System;
using MediatR;

namespace SFA.DAS.Provider.Events.Application.DataLock.GetDataLockEventsQuery
{
    public class GetDataLockEventsQueryRequest : IAsyncRequest<GetDataLockEventsQueryResponse>
    {
        public long SinceEventId { get; set; }
        public DateTime? SinceTime { get; set; }
        public string EmployerAccountId { get; set; }
        public long Ukprn { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}