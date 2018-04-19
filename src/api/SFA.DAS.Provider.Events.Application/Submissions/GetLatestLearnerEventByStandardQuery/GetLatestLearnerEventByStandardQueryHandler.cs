using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Provider.Events.Application.Repositories;
using SFA.DAS.Provider.Events.Application.Validation;

namespace SFA.DAS.Provider.Events.Application.Submissions.GetLatestLearnerEventByStandardQuery
{
    public class GetLatestLearnerEventByStandardQueryHandler :
        IAsyncRequestHandler<GetLatestLearnerEventByStandardQueryRequest, GetLatestLearnerEventByStandardQueryResponse>
    {
        private readonly IValidator<GetLatestLearnerEventByStandardQueryRequest> _validator;
        private readonly ISubmissionEventsRepository _submissionEventsRepository;

        public GetLatestLearnerEventByStandardQueryHandler(
            IValidator<GetLatestLearnerEventByStandardQueryRequest> validator,
            ISubmissionEventsRepository submissionEventsRepository)
        {
            _validator = validator;
            _submissionEventsRepository = submissionEventsRepository;
        }

        public async Task<GetLatestLearnerEventByStandardQueryResponse> Handle(GetLatestLearnerEventByStandardQueryRequest message)
        {
            var validationResult = await _validator.Validate(message);
            if (!validationResult.IsValid)
            {
                return new GetLatestLearnerEventByStandardQueryResponse
                {
                    IsValid = false,
                    Exception = new ValidationException(validationResult.ValidationMessages)
                };
            }

            var entities = await _submissionEventsRepository.GetLatestLearnerEventByStandard(message.SinceEventId, message.Uln);

            return new GetLatestLearnerEventByStandardQueryResponse
            {
                IsValid = true,
                Result = entities
            };
        }
    }
}