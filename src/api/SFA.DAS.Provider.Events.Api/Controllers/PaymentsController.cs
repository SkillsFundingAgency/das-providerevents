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

        //TODO THIS NEEDS TO BE REMOVED WHEN PV2-2308 IS DEVELOPED
        private CollectionPeriod GetPeriodFromPeriodIdTEMP(string periodId)
        {
            return string.IsNullOrWhiteSpace(periodId) ? null : new CollectionPeriod
            {
                Id = periodId,
                Period = byte.Parse(periodId.Substring(6)),
                AcademicYear = short.Parse(periodId.Substring(0, 4))
            };
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
                CollectionPeriod period = null;
                if (PeriodHasBeenProvided(periodId))
                {
                    //period = await GetPeriodAsync(periodId).ConfigureAwait(false); TODO THIS NEEDS TO BE PUT BACK IN WHEN PV2-2308 IS DEVELOPED
                    period = GetPeriodFromPeriodIdTEMP(periodId);
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

        private static bool PeriodNotFound(CollectionPeriod period)
        {
            return period == null;
        }

        private static bool PeriodHasBeenProvided(string periodId)
        {
            return !string.IsNullOrEmpty(periodId);
        }

        private async Task<CollectionPeriod> GetPeriodAsync(string periodId)
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

        private async Task<GetPaymentsQueryResponse> GetPaymentsAsync(string employerAccountId, int page, long? ukprn, CollectionPeriod period)
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
