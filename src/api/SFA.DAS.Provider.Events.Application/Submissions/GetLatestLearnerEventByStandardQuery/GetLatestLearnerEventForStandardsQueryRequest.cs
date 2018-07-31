using MediatR;

namespace SFA.DAS.Provider.Events.Application.Submissions.GetLatestLearnerEventByStandardQuery
{
    public class GetLatestLearnerEventForStandardsQueryRequest : IAsyncRequest<GetLatestLearnerEventForStandardsQueryResponse>
    {
        public long SinceEventId { get; set; }
        public long Uln { get; set; }
    }
}
