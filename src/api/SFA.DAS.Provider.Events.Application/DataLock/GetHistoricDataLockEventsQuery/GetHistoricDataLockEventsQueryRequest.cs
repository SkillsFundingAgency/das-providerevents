using MediatR;

namespace SFA.DAS.Provider.Events.Application.DataLock.GetHistoricDataLockEventsQuery
{
    public class GetHistoricDataLockEventsQueryRequest : IAsyncRequest<GetHistoricDataLockEventsQueryResponse>
    {
        public long Ukprn { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}