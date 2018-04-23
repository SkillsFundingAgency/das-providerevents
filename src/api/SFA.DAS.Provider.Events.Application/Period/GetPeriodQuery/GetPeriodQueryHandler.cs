using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Provider.Events.Application.Repositories;
using SFA.DAS.Provider.Events.Application.Validation;

namespace SFA.DAS.Provider.Events.Application.Period.GetPeriodQuery
{
    public class GetPeriodQueryHandler : IAsyncRequestHandler<GetPeriodQueryRequest, GetPeriodQueryResponse>
    {
        private readonly IValidator<GetPeriodQueryRequest> _requestValidator;
        private readonly IPeriodRepository _periodRepository;

        public GetPeriodQueryHandler(IValidator<GetPeriodQueryRequest> requestValidator, IPeriodRepository periodRepository)
        {
            _requestValidator = requestValidator;
            _periodRepository = periodRepository;
        }

        public async Task<GetPeriodQueryResponse> Handle(GetPeriodQueryRequest message)
        {
            try
            {
                var validationResult = await _requestValidator.Validate(message)
                    .ConfigureAwait(false);
                if (!validationResult.IsValid)
                {
                    return new GetPeriodQueryResponse
                    {
                        IsValid = false,
                        Exception = new ValidationException(validationResult.ValidationMessages)
                    };
                }

                var academicYear = message.PeriodId.Substring(0, 4);
                var periodName = message.PeriodId.Substring(5);

                var period = await _periodRepository.GetPeriod(academicYear, periodName)
                    .ConfigureAwait(false);
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
                    Result = new Data.Period
                    {
                        Id = period.Id,
                        CalendarMonth = period.CalendarMonth,
                        CalendarYear = period.CalendarYear,
                        PeriodName = message.PeriodId
                    }
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