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
    [RoutePrefix("api/payments")]
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

        [Route("", Name = "PaymentsList")]
        [HttpGet]
        public async Task<IHttpActionResult> GetListOfPayments(string periodId = null, string employerAccountId = null, int page = 1)
        {
            try
            {
                Period period = null;
                if (!string.IsNullOrEmpty(periodId))
                {
                    var getPeriodResponse = await _mediator.SendAsync(new GetPeriodQueryRequest { PeriodId = periodId });
                    if (!getPeriodResponse.IsValid)
                    {
                        throw getPeriodResponse.Exception;
                    }
                    if (getPeriodResponse.Result == null)
                    {
                        return Ok(new Types.PageOfResults<Payment>
                        {
                            PageNumber = page,
                            TotalNumberOfPages = 0,
                            Items = new Payment[0]
                        });
                    }
                    period = getPeriodResponse.Result;
                }

                var paymentsResponse = await _mediator.SendAsync(new GetPaymentsQueryRequest
                {
                    Period = period,
                    EmployerAccountId = employerAccountId,
                    PageNumber = page,
                    PageSize = PageSize
                });
                if (!paymentsResponse.IsValid)
                {
                    throw paymentsResponse.Exception;
                }

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
    }
}
