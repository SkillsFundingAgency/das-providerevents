using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Payments.Events.Application.Validation;
using SFA.DAS.Payments.Events.Domain.Data;

namespace SFA.DAS.Payments.Events.Application.Period.GetPeriodQuery
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
                var validationResult = await _requestValidator.Validate(message);
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

                var period = await _periodRepository.GetPeriod(academicYear, periodName);
                return new GetPeriodQueryResponse
                {
                    IsValid = true,
                    Result = new Domain.Period
                    {
                        Id = period.Id,
                        CalendarMonth = period.CalendarMonth,
                        CalendarYear = period.CalendarYear
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