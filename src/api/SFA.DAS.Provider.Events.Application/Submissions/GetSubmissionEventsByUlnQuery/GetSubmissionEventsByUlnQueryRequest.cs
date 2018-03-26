using System;
using MediatR;

namespace SFA.DAS.Provider.Events.Application.Submissions.GetSubmissionEventsByUlnQuery
{
    public class GetSubmissionEventsByUlnQueryRequest : IAsyncRequest<GetSubmissionEventsByUlnQueryResponse>
    {
        public long SinceEventId { get; set; }
        public long Uln { get; set; }
    }
}
