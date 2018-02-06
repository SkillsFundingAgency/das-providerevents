using MediatR;

namespace SFA.DAS.Provider.Events.Application.DataLock.GetCurrentDataLocksQuery
{
    public class GetCurrentDataLocksQueryRequest : IAsyncRequest<GetCurrentDataLocksQueryResponse>
    {
        public long Ukprn { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}