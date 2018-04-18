using MediatR;

namespace SFA.DAS.Provider.Events.Application.Submissions.GetLatestLearnerEventByStandardQuery
{
    public class GetLatestLearnerEventByStandardQueryRequest : IAsyncRequest<GetLatestLearnerEventByStandardQueryResponse>
    {
        public long SinceEventId { get; set; }
        public long Uln { get; set; }
    }
}
