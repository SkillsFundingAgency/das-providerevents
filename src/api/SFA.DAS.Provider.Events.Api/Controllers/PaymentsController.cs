using System;
using System.Threading.Tasks;
using System.Web.Http;
using MediatR;
using NLog;
using SFA.DAS.Provider.Events.Api.Plumbing.WebApi;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data;
using SFA.DAS.Provider.Events.Application.Payments.GetPaymentsQuery;
using SFA.DAS.Provider.Events.Application.Payments.GetPaymentsStatistics;
using SFA.DAS.Provider.Events.Application.Period.GetPeriodQuery;
using SFA.DAS.Provider.Events.Application.Validation;

namespace SFA.DAS.Provider.Events.Api.Controllers
{
    [AuthorizeRemoteOnly(Roles = "ReadPayments")]
    public class PaymentsController : ApiController
    {
        private const int PageSize = 10000;

        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public PaymentsController(IMediator mediator, ILogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [VersionedRoute("api/payments", 1, Name = "PaymentsList")]
        [VersionedRoute("api/payments", 2, Name = "PaymentsListV2H")]
        [Route("api/v2/payments", Name = "PaymentsListV2")]
        [HttpGet]
        public async Task<IHttpActionResult> GetListOfPayments(
            string periodId = null, 
            string employerAccountId = null, 
            int page = 1, long? 
            ukprn = null)
        {
            try
            {
                Period period = null;
                if (PeriodHasBeenProvided(periodId))
                {
                    period = await GetPeriodAsync(periodId).ConfigureAwait(false);
                    if (PeriodNotFound(period))
                    {
                        return Ok(new PageOfResults<Payment>
                        {
                            PageNumber = page,
                            TotalNumberOfPages = 0,
                            Items = new Payment[0]
                        });
                    }
                }

                var paymentsResponse = await 
                    GetPaymentsAsync(employerAccountId, page, ukprn, period)
                    .ConfigureAwait(false);

                return Ok(paymentsResponse.Result);
            }
            catch (ValidationException ex)
            {
                _logger.Error(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return InternalServerError();
            }
        }

        [HttpGet]
        [VersionedRoute("api/payments/statistics", 1, Name = "PaymentsStatistics")]
        [VersionedRoute("api/payments/statistics", 2, Name = "PaymentsStatisticsV2H")]
        [Route("api/v2/payments/statistics", Name = "PaymentsStatisticsV2")]
        public async Task<IHttpActionResult> GetPaymentStatistics()
        {
            try
            {
                var paymentsResponse = await _mediator
                    .SendAsync(new GetPaymentsStatisticsRequest()
                    ).ConfigureAwait(false);

                return Ok(paymentsResponse.Result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                throw;
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

        private async Task<Period> GetPeriodAsync(string periodId)
        {
            var getPeriodResponse = await _mediator
                .SendAsync(new GetPeriodQueryRequest { PeriodId = periodId })
                .ConfigureAwait(false);
            if (!getPeriodResponse.IsValid)
            {
                throw getPeriodResponse.Exception;
            }
            return getPeriodResponse.Result;
        }

        private async Task<GetPaymentsQueryResponse> GetPaymentsAsync(string employerAccountId, int page, long? ukprn, Period period)
        {
            var paymentsResponse = await _mediator
                .SendAsync(new GetPaymentsQueryRequest
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
