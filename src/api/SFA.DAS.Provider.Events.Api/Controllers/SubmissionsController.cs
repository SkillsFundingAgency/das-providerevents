using System;
using System.Threading.Tasks;
using System.Web.Http;
using MediatR;
using NLog;
using SFA.DAS.Provider.Events.Api.Plumbing.WebApi;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Submissions.GetSubmissionEventsQuery;
using SFA.DAS.Provider.Events.Application.Validation;
using SFA.DAS.Provider.Events.Domain.Mapping;

namespace SFA.DAS.Provider.Events.Api.Controllers
{
    [AuthorizeRemoteOnly(Roles = "ReadSubmissions")]
    public class SubmissionsController : ApiController
    {
        private const int PageSize = 1000;

        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public SubmissionsController(IMediator mediator, IMapper mapper, ILogger logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

        [VersionedRoute("api/submissions", 1, Name = "SubmissionEventsList")]
        [VersionedRoute("api/submissions", 2, Name = "SubmissionEventsListV2H")]
        [Route("api/v2/submissions", Name = "SubmissionEventsListV2")]
        [HttpGet]
        public async Task<IHttpActionResult> GetSubmissionEvents(long sinceEventId = 0, DateTime? sinceTime = null, long ukprn = 0, int pageNumber = 1)
        {
            try
            {
                _logger.Debug($"Processing GetSubmissionEvents, sinceEventId={sinceEventId}, sinceTime={sinceTime}, pageNumber={pageNumber}");

                var queryResponse = await _mediator.SendAsync(new GetSubmissionEventsQueryRequest
                {
                    SinceEventId = sinceEventId,
                    SinceTime = sinceTime,
                    Ukprn = ukprn,
                    PageNumber = pageNumber,
                    PageSize = PageSize
                });
                if (!queryResponse.IsValid)
                {
                    throw queryResponse.Exception;
                }

                return Ok(_mapper.Map<PageOfResults<SubmissionEvent>>(queryResponse.Result));
            }
            catch (ValidationException ex)
            {
                _logger.Info($"Bad request received to GetSubmissionEvents - {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Unexpected error processing GetSubmissionEvents - {ex.Message}");
                return InternalServerError();
            }
        }
    }
}
