using MediatR;

namespace SFA.DAS.Provider.Events.Submission.Application.GetLastSeenVersions
{
    public class GetLastSeenVersionsQuery : IRequest<GetLastSeenVersionsQueryResponse>
    {
    }
}
