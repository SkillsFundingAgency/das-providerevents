using MediatR;

namespace SFA.DAS.Provider.Events.DataLock.Application.GetLastSeenProviderEvents
{
    public class GetLastSeenProviderEventsRequest : IRequest<GetLastSeenProviderEventsResponse>
    {
        public long Ukprn { get; set; }
    }
}