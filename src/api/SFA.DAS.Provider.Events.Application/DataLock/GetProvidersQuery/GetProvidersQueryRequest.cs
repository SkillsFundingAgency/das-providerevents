using MediatR;

namespace SFA.DAS.Provider.Events.Application.DataLock.GetProvidersQuery
{
    public class GetProvidersQueryRequest : IAsyncRequest<GetProvidersQueryResponse>
    {
        public bool UpdatedOnly {get; set; }
    }
}