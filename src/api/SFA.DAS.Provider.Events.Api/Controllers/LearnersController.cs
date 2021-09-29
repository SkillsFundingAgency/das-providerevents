using System;
using System.Threading.Tasks;
using System.Web.Http;
using MediatR;
using NLog;
using SFA.DAS.Provider.Events.Api.Plumbing.WebApi;
using SFA.DAS.Provider.Events.Application.Submissions.GetLatestLearnerEventByStandardQuery;

namespace SFA.DAS.Provider.Events.Api.Controllers
{
    [AuthorizeRemoteOnly(Roles = "ReadLearners")]
    public class LearnersController : ApiController
    {
        private IMediator _mediator;
        private ILogger _logger;

        public LearnersController(IMediator mediator, ILogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }


        [Route("api/learners", Name = "GetLatestLearnerEventForStandards")]
        [HttpGet]
        public async Task<IHttpActionResult> GetLatestLearnerEventForStandards(long uln, long sinceEventId = 0)
        {
            try
            {
                _logger.Debug($"Processing GetLatestLearnerEventForStandards, uln={uln}, sinceEventId={sinceEventId}");

                var queryResponse = await _mediator.SendAsync(new GetLatestLearnerEventForStandardsQueryRequest
                    {
                        SinceEventId = sinceEventId,
                        Uln = uln
                    })
                    .ConfigureAwait(false);

                if (queryResponse.IsValid) return Ok(queryResponse.Result);

                _logger.Info($"Bad request received to GetLatestLearnerEventForStandards - {queryResponse.Exception.Message}");
                return BadRequest(queryResponse.Exception.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Unexpected error processing GetLatestLearnerEventForStandards - {ex.Message}");
                return InternalServerError();
            }
        }
    }
}