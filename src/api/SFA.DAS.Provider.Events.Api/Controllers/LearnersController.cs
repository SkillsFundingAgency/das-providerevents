using MediatR;
using NLog;
using SFA.DAS.Provider.Events.Application.Submissions.GetLatestLearnerEventByStandardQuery;
using SFA.DAS.Provider.Events.Application.Validation;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace SFA.DAS.Provider.Events.Api.Controllers
{
    public class LearnersController : ApiController
    {
        private const int PageSize = 1000;

        private IMediator _mediator;
        private ILogger _logger;

        public LearnersController(IMediator mediator, ILogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }


        [Route("api/learners/{uln}", Name = "GetLatestLearnerEventForStandardsByUln")]
        [HttpGet]
        public async Task<IHttpActionResult> GetLatestLearnerEventForStandardsByUln(long uln, long sinceEventId = 0, int pageNumber = 1)
        {
            try
            {
                _logger.Debug($"Processing GetLatestLearnerEventForStandardsByUln, uln={uln}, sinceEventId={sinceEventId}, pageNumber={pageNumber}");

                var queryResponse = await _mediator.SendAsync(new GetLatestLearnerEventForStandardsQueryRequest
                {
                    SinceEventId = sinceEventId,
                    Uln = uln,
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
                _logger.Info($"Bad request received to GetLatestLearnerEventForStandardsByUln - {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Unexpected error processing GetLatestLearnerEventForStandards - {ex.Message}");
                return InternalServerError();
            }
        }

        [Route("api/learners", Name = "GetLatestLearnerEventForStandards")]
        [HttpGet]
        public async Task<IHttpActionResult> GetLatestLearnerEventForStandards(long sinceEventId = 0, int pageNumber = 1)
        {
            try
            {
                _logger.Debug($"Processing GetLatestLearnerEventForStandards, sinceEventId={sinceEventId}, pageNumber={pageNumber}");

                var queryResponse = await _mediator.SendAsync(new GetLatestLearnerEventForStandardsQueryRequest
                {
                    SinceEventId = sinceEventId,
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
                _logger.Info($"Bad request received to GetLatestLearnerEventForStandards - {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Unexpected error processing GetLatestLearnerEventForStandards - {ex.Message}");
                return InternalServerError();
            }
        }
    }
}