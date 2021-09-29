using System;
using MediatR;

namespace SFA.DAS.Provider.Events.Application.Submissions.GetSubmissionEventsQuery
{
    public class GetSubmissionEventsQueryRequest : IAsyncRequest<GetSubmissionEventsQueryResponse>
    {
        public long SinceEventId { get; set; }
        public DateTime? SinceTime { get; set; }
        public long Ukprn { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
