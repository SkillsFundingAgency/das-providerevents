using MediatR;

namespace SFA.DAS.Provider.Events.DataLock.Application.GetCurrentProviderEvents
{
    public class GetCurrentProviderEventsRequest : IRequest<GetCurrentProviderEventsResponse>
    {
        public long Ukprn { get; set; }
    }
}