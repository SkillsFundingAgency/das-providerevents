using System;
using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.Provider.Events.Application.Submissions.GetSubmissionEventsQuery
{
    public class GetSubmissionEventsQueryHandler : IAsyncRequestHandler<GetSubmissionEventsQueryRequest, GetSubmissionEventsQueryResponse>
    {
        public Task<GetSubmissionEventsQueryResponse> Handle(GetSubmissionEventsQueryRequest message)
        {
            throw new NotImplementedException();
        }
    }
}