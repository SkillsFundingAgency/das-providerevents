using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Provider.Events.Application.Repositories;
using SFA.DAS.Provider.Events.Application.Validation;

namespace SFA.DAS.Provider.Events.Application.Submissions.GetSubmissionEventsByUlnQuery
{
    public class GetSubmissionEventsByUlnQueryHandler : 
        IAsyncRequestHandler<GetSubmissionEventsByUlnQueryRequest, GetSubmissionEventsByUlnQueryResponse>
    {
        private readonly IValidator<GetSubmissionEventsByUlnQueryRequest> _validator;
        private readonly ISubmissionEventsRepository _submissionEventsRepository;

        public GetSubmissionEventsByUlnQueryHandler(
            IValidator<GetSubmissionEventsByUlnQueryRequest> validator, 
            ISubmissionEventsRepository submissionEventsRepository)
        {
            _validator = validator;
            _submissionEventsRepository = submissionEventsRepository;
        }

        public async Task<GetSubmissionEventsByUlnQueryResponse> Handle(GetSubmissionEventsByUlnQueryRequest message)
        {
            try
            {
                var validationResult = await _validator.Validate(message);
                if (!validationResult.IsValid)
                {
                    return new GetSubmissionEventsByUlnQueryResponse
                    {
                        IsValid = false,
                        Exception = new ValidationException(validationResult.ValidationMessages)
                    };
                }

                var entities = await _submissionEventsRepository.GetSubmissionEventsForUln(message.SinceEventId, message.Uln);

                return new GetSubmissionEventsByUlnQueryResponse
                {
                    IsValid = true,
                    Result = entities
                };
            }
            catch (Exception ex)
            {
                return new GetSubmissionEventsByUlnQueryResponse
                {
                    IsValid = false,
                    Exception = ex
                };
            }
        }
    }
}