using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using MediatR;
using SFA.DAS.Payments.Events.Api.Types;
using SFA.DAS.Payments.Events.Application.Payments.GetPaymentsForPeriodQuery;
using SFA.DAS.Payments.Events.Application.Period.GetPeriodQuery;
using SFA.DAS.Payments.Events.Application.Validation;

namespace SFA.DAS.Payments.Events.Api.Controllers
{
    [RoutePrefix("api/payments")]
    public class PaymentsController : ApiController
    {
        private readonly IMediator _mediator;

        public PaymentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("", Name = "PaymentsList")]
        public async Task<IHttpActionResult> GetListOfPayments(string periodId = null, string employerAccountId = null, int page = 1, int pageSize = 1000)
        {
            try
            {
                Domain.Period period = null;
                if (!string.IsNullOrEmpty(periodId))
                {
                    var getPeriodResponse = await _mediator.SendAsync(new GetPeriodQueryRequest {PeriodId = periodId});
                    period = getPeriodResponse.Result;
                }

                var paymentsResponse = await _mediator.SendAsync(new GetPaymentsForPeriodQueryRequest
                {
                    Period = period,
                    EmployerAccountId = employerAccountId,
                    PageNumber = page,
                    PageSize = pageSize
                });

                var result = new PageOfResults<Payment>
                {
                    PageNumber = paymentsResponse.Result.PageNumber,
                    TotalNumberOfPages = paymentsResponse.Result.TotalNumberOfPages,
                    Items = paymentsResponse.Result.Items.Select(p => new Payment
                    {
                        Id = p.Id,
                        Ukprn = p.Ukprn,
                        Uln = p.Uln,
                        EmployerAccountId = p.EmployerAccountId,
                        ApprenticeshipId = p.ApprenticeshipId,
                        CollectionPeriod = new NamedCalendarPeriod
                        {
                            Month = p.CollectionPeriod.Month,
                            Year = p.CollectionPeriod.Year
                        },
                        DeliveryPeriod = new CalendarPeriod
                        {
                            Month = p.DeliveryPeriod.Month,
                            Year = p.DeliveryPeriod.Year
                        },
                        EvidenceSubmittedOn = p.EvidenceSubmittedOn,
                        EmployerAccountVersion = p.EmployerAccountVersion,
                        ApprenticeshipVersion = p.ApprenticeshipVersion,
                        FundingSource = (FundingSource) (int) p.FundingSource,
                        TransactionType = (TransactionType) (int) p.TransactionType,
                        Amount = p.Amount
                    }).ToArray()
                };

                return Ok(result);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
    }
}
