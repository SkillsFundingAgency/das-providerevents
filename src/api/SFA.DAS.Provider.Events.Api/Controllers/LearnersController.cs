using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using MediatR;
using Microsoft.ApplicationInsights;
using SFA.DAS.Provider.Events.Api.Plumbing.WebApi;
using SFA.DAS.Provider.Events.Application.Submissions.GetLatestLearnerEventByStandardQuery;

namespace SFA.DAS.Provider.Events.Api.Controllers
{
    [AuthorizeRemoteOnly(Roles = "ReadLearners")]
    public class LearnersController : ApiController
    {
        private IMediator _mediator;
        private TelemetryClient _telemetryClient;

        public LearnersController(IMediator mediator, TelemetryClient telemetryClient)
        {
            _mediator = mediator;
            _telemetryClient = telemetryClient;
        }


        [Route("api/learners", Name = "GetLatestLearnerEventForStandards")]
        [HttpGet]
        public async Task<IHttpActionResult> GetLatestLearnerEventForStandards(long uln, long sinceEventId = 0)
        {
            try
            {
                _telemetryClient.TrackTrace($"Processing GetLatestLearnerEventForStandards, uln={uln}, sinceEventId={sinceEventId}");

                var queryResponse = await _mediator.SendAsync(new GetLatestLearnerEventForStandardsQueryRequest
                    {
                        SinceEventId = sinceEventId,
                        Uln = uln
                    })
                    .ConfigureAwait(false);

                if (queryResponse.IsValid) return Ok(queryResponse.Result);

                _telemetryClient.TrackException(queryResponse.Exception, new Dictionary<string, string> { {"Message", $"Bad request received to GetLatestLearnerEventForStandards - {queryResponse.Exception.Message}"}});
                return BadRequest(queryResponse.Exception.Message);
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex, new Dictionary<string, string> {
                    {
                        "Message", $"Unexpected error processing GetLatestLearnerEventForStandards - {ex.Message}"
                    }
                });
                return InternalServerError();
            }
        }
    }
}