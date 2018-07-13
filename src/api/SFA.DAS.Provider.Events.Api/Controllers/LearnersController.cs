using System;
using System.Threading.Tasks;
using System.Web.Http;
using MediatR;
using NLog;
using SFA.DAS.Provider.Events.Application.Submissions.GetLatestLearnerEventByStandardQuery;

namespace SFA.DAS.Provider.Events.Api.Controllers
{
    public class LearnersController : ApiController
    {
        private IMediator _mediator;
        private ILogger _logger;

        public LearnersController(IMediator mediator, ILogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }


        [Route("api/v2/learners", Name = "GetLatestLearnerEventByStandard")]
        [HttpGet]
        public async Task<IHttpActionResult> GetLatestLearnerEventByStandard(long uln, long sinceEventId = 0)
        {
            try
            {
                _logger.Debug($"Processing GetLatestLearnerEventByStandard, uln={uln}, sinceEventId={sinceEventId}");

                var queryResponse = await _mediator.SendAsync(new GetLatestLearnerEventByStandardQueryRequest
                    {
                        SinceEventId = sinceEventId,
                        Uln = uln
                    })
                    .ConfigureAwait(false);

                if (queryResponse.IsValid) return Ok(queryResponse.Result);

                _logger.Info($"Bad request received to GetLatestLearnerEventByStandard - {queryResponse.Exception.Message}");
                return BadRequest(queryResponse.Exception.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Unexpected error processing GetLatestLearnerEventByStandard - {ex.Message}");
                return InternalServerError();
            }
        }
    }
}