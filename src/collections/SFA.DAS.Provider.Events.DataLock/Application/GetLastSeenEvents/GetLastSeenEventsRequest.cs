using MediatR;

namespace SFA.DAS.Provider.Events.DataLock.Application.GetLastSeenEvents
{
    public class GetLastSeenEventsRequest : IRequest<GetLastSeenEventsResponse>
    {
    }
}