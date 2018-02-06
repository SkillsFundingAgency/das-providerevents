using MediatR;

namespace SFA.DAS.Provider.Events.Application.DataLock.GetLatestDataLocksQuery
{
    public class GetLatestDataLocksQueryRequest : IAsyncRequest<GetLatestDataLocksQueryResponse>
    {
        public long Ukprn { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}