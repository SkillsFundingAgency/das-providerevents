using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Provider.Events.Application.Mapping;
using SFA.DAS.Provider.Events.Application.Repositories;
using SFA.DAS.Provider.Events.Application.Validation;

namespace SFA.DAS.Provider.Events.Application.Period.GetPeriodQuery
{
    public class GetPeriodQueryHandler : IAsyncRequestHandler<GetPeriodQueryRequest, GetPeriodQueryResponse>
    {
        private readonly IValidator<GetPeriodQueryRequest> _requestValidator;
        private readonly IPeriodRepository _periodRepository;
        private readonly IMapper _mapper;

        public GetPeriodQueryHandler(IValidator<GetPeriodQueryRequest> requestValidator, IPeriodRepository periodRepository, IMapper mapper)
        {
            _requestValidator = requestValidator ?? throw new ArgumentNullException(nameof(requestValidator));
            _periodRepository = periodRepository ?? throw new ArgumentNullException(nameof(periodRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<GetPeriodQueryResponse> Handle(GetPeriodQueryRequest message)
        {
            try
            {
                var validationResult = await _requestValidator.Validate(message).ConfigureAwait(false);

                if (!validationResult.IsValid)
                {
                    return new GetPeriodQueryResponse
                    {
                        IsValid = false,
                        Exception = new ValidationException(validationResult.ValidationMessages)
                    };
                }

                var period = await _periodRepository.GetPeriod(message.AcademicYear, message.Period).ConfigureAwait(false);

                if (period == null)
                {
                    return new GetPeriodQueryResponse
                    {
                        IsValid = true,
                        Result = null
                    };
                }
                return new GetPeriodQueryResponse
                {
                    IsValid = true,
                    Result = _mapper.Map<Data.CollectionPeriod>(period),
                };
            }
            catch (Exception ex)
            {
                return new GetPeriodQueryResponse
                {
                    IsValid = false,
                    Exception = ex
                };
            }
        }
    }
}