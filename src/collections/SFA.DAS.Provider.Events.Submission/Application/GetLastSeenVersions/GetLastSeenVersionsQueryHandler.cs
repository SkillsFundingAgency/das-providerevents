using System;
using MediatR;
using SFA.DAS.Provider.Events.Submission.Domain.Data;

namespace SFA.DAS.Provider.Events.Submission.Application.GetLastSeenVersions
{
    public class GetLastSeenVersionsQueryHandler : IRequestHandler<GetLastSeenVersionsQuery, GetLastSeenVersionsQueryResponse>
    {
        private readonly IIlrSubmissionRepository _ilrSubmissionRepository;

        public GetLastSeenVersionsQueryHandler(IIlrSubmissionRepository ilrSubmissionRepository)
        {
            _ilrSubmissionRepository = ilrSubmissionRepository;
        }

        public GetLastSeenVersionsQueryResponse Handle(GetLastSeenVersionsQuery message)
        {
            try
            {
                var items = _ilrSubmissionRepository.GetLastSeenVersions();

                return new GetLastSeenVersionsQueryResponse
                {
                    IsValid = true,
                    Items = items ?? new Domain.IlrDetails[0]
                };
            }
            catch (Exception ex)
            {
                return new GetLastSeenVersionsQueryResponse
                {
                    IsValid = false,
                    Exception = ex
                };
            }
        }
    }
}
