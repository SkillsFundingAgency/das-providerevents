using System;
using System.Threading.Tasks;
using System.Web.Http;
using MediatR;
using NLog;
using SFA.DAS.Provider.Events.Api.Plumbing.WebApi;
using SFA.DAS.Provider.Events.Application.Payments.GetPaymentsQuery;
using SFA.DAS.Provider.Events.Application.Period.GetPeriodQuery;
using SFA.DAS.Provider.Events.Application.Validation;
using SFA.DAS.Provider.Events.Domain;
using SFA.DAS.Provider.Events.Domain.Mapping;
using Payment = SFA.DAS.Provider.Events.Api.Types.Payment;

namespace SFA.DAS.Provider.Events.Api.Controllers
{
    [AuthorizeRemoteOnly(Roles = "ReadPayments")]
    public class PaymentsController : ApiController
    {
        private const int PageSize = 1000;

        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public PaymentsController(IMediator mediator, IMapper mapper, ILogger logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

        [VersionedRoute("api/payments", 1, Name = "PaymentsList")]
        [VersionedRoute("api/payments", 2, Name = "PaymentsListV2H")]
        [Route("api/v2/payments", Name = "PaymentsListV2")]
        [HttpGet]
        public async Task<IHttpActionResult> GetListOfPayments(string periodId = null, string employerAccountId = null, int page = 1, long? ukprn = null)
        {
            try
            {
                Period period = null;
                if (PeriodHasBeenProvided(periodId))
                {
                    period = await GetPeriod(periodId).ConfigureAwait(false);
                    if (PeriodNotFound(period))
                    {
                        return Ok(new Types.PageOfResults<Payment>
                        {
                            PageNumber = page,
                            TotalNumberOfPages = 0,
                            Items = new Payment[0]
                        });
                    }
                }

                var paymentsResponse = await GetPayments(employerAccountId, page, ukprn, period);

                return Ok(_mapper.Map<Types.PageOfResults<Payment>>(paymentsResponse.Result));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return InternalServerError();
            }
        }

        private static bool PeriodNotFound(Period period)
        {
            return period == null;
        }

        private static bool PeriodHasBeenProvided(string periodId)
        {
            return !string.IsNullOrEmpty(periodId);
        }

        private async Task<Period> GetPeriod(string periodId)
        {
            var getPeriodResponse = await _mediator.SendAsync(new GetPeriodQueryRequest { PeriodId = periodId }).ConfigureAwait(false);
            if (!getPeriodResponse.IsValid)
            {
                throw getPeriodResponse.Exception;
            }
            return getPeriodResponse.Result;
        }

        private async Task<GetPaymentsQueryResponse> GetPayments(string employerAccountId, int page, long? ukprn, Period period)
        {
            var paymentsResponse = await _mediator.SendAsync(new GetPaymentsQueryRequest
            {
                Period = period,
                EmployerAccountId = employerAccountId,
                PageNumber = page,
                PageSize = PageSize,
                Ukprn = ukprn
            }).ConfigureAwait(false);

            if (!paymentsResponse.IsValid)
            {
                throw paymentsResponse.Exception;
            }

            return paymentsResponse;
        }
    }
}
