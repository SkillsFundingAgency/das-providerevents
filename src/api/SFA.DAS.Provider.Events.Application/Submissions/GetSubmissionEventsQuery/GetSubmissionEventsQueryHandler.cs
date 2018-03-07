using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data.Entities;
using SFA.DAS.Provider.Events.Application.Mapping;
using SFA.DAS.Provider.Events.Application.Repositories;
using SFA.DAS.Provider.Events.Application.Validation;

namespace SFA.DAS.Provider.Events.Application.Submissions.GetSubmissionEventsQuery
{
    public class GetSubmissionEventsQueryHandler : 
        IAsyncRequestHandler<GetSubmissionEventsQueryRequest, GetSubmissionEventsQueryResponse>
    {
        private readonly IValidator<GetSubmissionEventsQueryRequest> _validator;
        private readonly ISubmissionEventsRepository _submissionEventsRepository;
        private readonly IMapper _mapper;

        public GetSubmissionEventsQueryHandler(
            IValidator<GetSubmissionEventsQueryRequest> validator, 
            ISubmissionEventsRepository submissionEventsRepository, 
            IMapper mapper)
        {
            _validator = validator;
            _submissionEventsRepository = submissionEventsRepository;
            _mapper = mapper;
        }

        public async Task<GetSubmissionEventsQueryResponse> Handle(GetSubmissionEventsQueryRequest message)
        {
            try
            {
                var validationResult = await _validator.Validate(message);
                if (!validationResult.IsValid)
                {
                    return new GetSubmissionEventsQueryResponse
                    {
                        IsValid = false,
                        Exception = new ValidationException(validationResult.ValidationMessages)
                    };
                }

                PageOfResults<SubmissionEventEntity> pageOfEntities;

                if (message.Ukprn > 0)
                {
                    pageOfEntities = message.SinceTime.HasValue
                        ? await _submissionEventsRepository.GetSubmissionEventsForProviderSinceTime(message.Ukprn, message.SinceTime.Value, message.PageNumber, message.PageSize)
                        : await _submissionEventsRepository.GetSubmissionEventsForProviderSinceId(message.Ukprn, message.SinceEventId, message.PageNumber, message.PageSize);
                }
                else if (message.SinceTime.HasValue)
                {
                    pageOfEntities = await _submissionEventsRepository.GetSubmissionEventsSinceTime(message.SinceTime.Value, message.PageNumber, message.PageSize);
                }
                else
                {
                    pageOfEntities = await _submissionEventsRepository.GetSubmissionEventsSinceId(message.SinceEventId, message.PageNumber, message.PageSize);
                }

                return new GetSubmissionEventsQueryResponse
                {
                    IsValid = true,
                    Result = _mapper.Map<Api.Types.PageOfResults<SubmissionEvent>>(pageOfEntities)
                };
            }
            catch (Exception ex)
            {
                return new GetSubmissionEventsQueryResponse
                {
                    IsValid = false,
                    Exception = ex
                };
            }
        }
    }
}