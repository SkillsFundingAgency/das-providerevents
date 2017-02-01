using System;
using MediatR;
using SFA.DAS.Provider.Events.Submission.Domain.Data;

namespace SFA.DAS.Provider.Events.Submission.Application.GetCurrentVersions
{
    public class GetCurrentVersionsQueryHandler : IRequestHandler<GetCurrentVersionsQuery, GetCurrentVersionsQueryResponse>
    {
        private readonly IIlrSubmissionRepository _ilrSubmissionRepository;

        public GetCurrentVersionsQueryHandler(IIlrSubmissionRepository ilrSubmissionRepository)
        {
            _ilrSubmissionRepository = ilrSubmissionRepository;
        }

        public GetCurrentVersionsQueryResponse Handle(GetCurrentVersionsQuery message)
        {
            try
            {
                var items = _ilrSubmissionRepository.GetCurrentVersions();

                return new GetCurrentVersionsQueryResponse
                {
                    IsValid = true,
                    Items = items ?? new Domain.IlrDetails[0]
                };
            }
            catch (Exception ex)
            {
                return new GetCurrentVersionsQueryResponse
                {
                    IsValid = false,
                    Exception = ex
                };
            }
        }
    }
}
