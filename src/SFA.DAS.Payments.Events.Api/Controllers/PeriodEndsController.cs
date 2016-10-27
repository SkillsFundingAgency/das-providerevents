using System;
using System.Threading.Tasks;
using System.Web.Http;
using MediatR;
using NLog;
using SFA.DAS.Payments.Events.Api.Types;
using SFA.DAS.Payments.Events.Application.Period.GetPeriodsQuery;
using SFA.DAS.Payments.Events.Application.Validation;
using SFA.DAS.Payments.Events.Domain.Mapping;

namespace SFA.DAS.Payments.Events.Api.Controllers
{
    [RoutePrefix("api/periodends")]
    public class PeriodEndsController : ApiController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public PeriodEndsController(IMediator mediator, IMapper mapper, ILogger logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

        [Route("", Name = "PeriodEndList")]
        [HttpGet]
        public async Task<IHttpActionResult> ListPeriodEnds()
        {
            try
            {
                var periodsResponse = await _mediator.SendAsync(new GetPeriodsQueryRequest());
                if (!periodsResponse.IsValid)
                {
                    throw periodsResponse.Exception;
                }

                var periodEnds = _mapper.Map<PeriodEnd[]>(periodsResponse.Result);
                foreach (var periodEnd in periodEnds)
                {
                    periodEnd.Links = new PeriodEndLinks
                    {
                        PaymentsForPeriod = Url.Link("PaymentsList", new { periodId = periodEnd.Id })
                    };
                }
                return Ok(periodEnds);
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
