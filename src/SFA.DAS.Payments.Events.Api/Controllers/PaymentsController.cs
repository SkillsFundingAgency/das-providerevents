using System;
using System.Threading.Tasks;
using System.Web.Http;
using MediatR;
using SFA.DAS.Payments.Events.Api.Types;
using SFA.DAS.Payments.Events.Application.Payments.GetPaymentsForPeriodQuery;
using SFA.DAS.Payments.Events.Application.Period.GetPeriodQuery;
using SFA.DAS.Payments.Events.Application.Validation;
using SFA.DAS.Payments.Events.Domain.Mapping;

namespace SFA.DAS.Payments.Events.Api.Controllers
{
    [RoutePrefix("api/payments")]
    public class PaymentsController : ApiController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public PaymentsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [Route("", Name = "PaymentsList")]
        public async Task<IHttpActionResult> GetListOfPayments(string periodId = null, string employerAccountId = null, int page = 1, int pageSize = 1000)
        {
            try
            {
                Domain.Period period = null;
                if (!string.IsNullOrEmpty(periodId))
                {
                    var getPeriodResponse = await _mediator.SendAsync(new GetPeriodQueryRequest { PeriodId = periodId });
                    if (!getPeriodResponse.IsValid)
                    {
                        throw getPeriodResponse.Exception;
                    }
                    period = getPeriodResponse.Result;
                }

                var paymentsResponse = await _mediator.SendAsync(new GetPaymentsForPeriodQueryRequest
                {
                    Period = period,
                    EmployerAccountId = employerAccountId,
                    PageNumber = page,
                    PageSize = pageSize
                });
                if (!paymentsResponse.IsValid)
                {
                    throw paymentsResponse.Exception;
                }

                return Ok(_mapper.Map<PageOfResults<Payment>>(paymentsResponse.Result));
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
