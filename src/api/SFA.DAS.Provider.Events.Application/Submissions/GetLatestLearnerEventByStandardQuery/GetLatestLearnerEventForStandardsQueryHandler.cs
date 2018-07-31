using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Mapping;
using SFA.DAS.Provider.Events.Application.Repositories;
using SFA.DAS.Provider.Events.Application.Validation;

namespace SFA.DAS.Provider.Events.Application.Submissions.GetLatestLearnerEventByStandardQuery
{
    public class GetLatestLearnerEventForStandardsQueryHandler :
        IAsyncRequestHandler<GetLatestLearnerEventForStandardsQueryRequest, GetLatestLearnerEventForStandardsQueryResponse>
    {
        private readonly IValidator<GetLatestLearnerEventForStandardsQueryRequest> _validator;
        private readonly ISubmissionEventsRepository _submissionEventsRepository;
        private readonly IMapper _mapper;

        public GetLatestLearnerEventForStandardsQueryHandler(
            IValidator<GetLatestLearnerEventForStandardsQueryRequest> validator,
            ISubmissionEventsRepository submissionEventsRepository, IMapper mapper)
        {
            _validator = validator;
            _submissionEventsRepository = submissionEventsRepository;
            _mapper = mapper;
        }

        public async Task<GetLatestLearnerEventForStandardsQueryResponse> Handle(GetLatestLearnerEventForStandardsQueryRequest message)
        {
            var validationResult = await _validator.Validate(message);
            if (!validationResult.IsValid)
            {
                return new GetLatestLearnerEventForStandardsQueryResponse
                {
                    IsValid = false,
                    Exception = new ValidationException(validationResult.ValidationMessages)
                };
            }

            var eventEntity = await _submissionEventsRepository.GetLatestLearnerEventByStandard(message.SinceEventId, message.Uln);

            var submissionEvents = _mapper.Map<List<SubmissionEvent>>(eventEntity);

            return new GetLatestLearnerEventForStandardsQueryResponse
            {
                IsValid = true,
                Result = submissionEvents
            };
        }
    }
}