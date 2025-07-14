using MediatR;
using Microsoft.ApplicationInsights;
using SFA.DAS.Provider.Events.Api.Plumbing.WebApi;
using SFA.DAS.Provider.Events.Application.Submissions.GetSubmissionEventsQuery;
using SFA.DAS.Provider.Events.Application.Validation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace SFA.DAS.Provider.Events.Api.Controllers
{
    [AuthorizeRemoteOnly(Roles = "ReadSubmissions")]
    public class SubmissionsController : ApiController
    {
        private const int PageSize = 1000;

        private readonly IMediator _mediator;
        private readonly TelemetryClient _telemetryClient;

        public SubmissionsController(IMediator mediator, TelemetryClient telemetryClient)
        {
            _mediator = mediator;
            _telemetryClient = telemetryClient;
        }

        [VersionedRoute("api/submissions", 1, Name = "SubmissionEventsList")]
        [VersionedRoute("api/submissions", 2, Name = "SubmissionEventsListV2H")]
        [Route("api/v2/submissions", Name = "SubmissionEventsListV2")]
        [HttpGet]
        public async Task<IHttpActionResult> GetSubmissionEvents(long sinceEventId = 0, DateTime? sinceTime = null, long ukprn = 0, int pageNumber = 1)
        {
            try
            {
                _telemetryClient.TrackTrace($"Processing GetSubmissionEvents, sinceEventId={sinceEventId}, sinceTime={sinceTime}, pageNumber={pageNumber}");

                var queryResponse = await _mediator.SendAsync(new GetSubmissionEventsQueryRequest
                {
                    SinceEventId = sinceEventId,
                    SinceTime = sinceTime,
                    Ukprn = ukprn,
                    PageNumber = pageNumber,
                    PageSize = PageSize
                }).ConfigureAwait(false);
                if (!queryResponse.IsValid)
                {
                    throw queryResponse.Exception;
                }

                return Ok(queryResponse.Result);
            }
            catch (ValidationException ex)
            {
                _telemetryClient.TrackTrace(ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex, new Dictionary<string, string> { { "Message", $"Unexpected error processing GetSubmissionEvents - {ex.Message}" } });
                return InternalServerError();
            }
        }

        
    }
}
