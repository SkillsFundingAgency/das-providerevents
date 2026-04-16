using System;
using System.Threading.Tasks;
using System.Web.Http;
using MediatR;
using NLog;
using Microsoft.ApplicationInsights;
using SFA.DAS.Provider.Events.Api.Plumbing.WebApi;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Mapping;
using SFA.DAS.Provider.Events.Application.Period.GetPeriodsQuery;
using SFA.DAS.Provider.Events.Application.Validation;

namespace SFA.DAS.Provider.Events.Api.Controllers
{
    [AuthorizeRemoteOnly(Roles = "ReadPayments")]
    public class PeriodEndsController : ApiController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly TelemetryClient _telemetry;

        public PeriodEndsController(
            IMediator mediator,
            IMapper mapper,
            ILogger logger,
            TelemetryClient telemetry)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
            _telemetry = telemetry;
        }

        [VersionedRoute("api/periodends", 1, Name = "PeriodEndList")]
        [VersionedRoute("api/periodends", 2, Name = "PeriodEndListV2H")]
        [Route("api/v2/periodends", Name = "PeriodEndListV2")]
        [HttpGet]
        public async Task<IHttpActionResult> ListPeriodEnds()
        {
            try
            {
                var periodsResponse = await _mediator.SendAsync(
                        new GetPeriodsQueryRequest())
                    .ConfigureAwait(false);

                if (!periodsResponse.IsValid)
                {
                    _telemetry.TrackException(periodsResponse.Exception);
                    _logger.Error(periodsResponse.Exception, "GetPeriodsQuery failed");
                    return InternalServerError(periodsResponse.Exception);
                }

                var periodEnds = _mapper.Map<PeriodEnd[]>(periodsResponse.Result);

                foreach (var periodEnd in periodEnds)
                {
                    periodEnd.Links = new PeriodEndLinks
                    {
                        PaymentsForPeriod = Url.Link(
                            "PaymentsList",
                            new { periodId = periodEnd.Id })
                    };

                    // Defensive null handling for new periods
                    if (periodEnd.ReferenceData == null)
                    {
                        periodEnd.ReferenceData = new ReferenceData
                        {
                            AccountDataValidAt = DateTime.MinValue,
                            CommitmentDataValidAt = DateTime.MinValue
                        };
                    }
                    else
                    {
                        if (!periodEnd.ReferenceData.AccountDataValidAt.HasValue)
                        {
                            periodEnd.ReferenceData.AccountDataValidAt = DateTime.MinValue;
                        }

                        if (!periodEnd.ReferenceData.CommitmentDataValidAt.HasValue)
                        {
                            periodEnd.ReferenceData.CommitmentDataValidAt = DateTime.MinValue;
                        }
                    }
                }

                return Ok(periodEnds);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                _logger.Error(ex, "Unhandled exception in ListPeriodEnds");
                return InternalServerError(ex);
            }
        }
    }
}
