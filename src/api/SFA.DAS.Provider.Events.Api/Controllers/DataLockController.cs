using System;
using System.Threading.Tasks;
using System.Web.Http;
using MediatR;
using NLog;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.DataLock.GetDataLockEventsQuery;
using SFA.DAS.Provider.Events.Application.Validation;
using SFA.DAS.Provider.Events.Domain.Mapping;

namespace SFA.DAS.Provider.Events.Api.Controllers
{
    [RoutePrefix("api/datalock")]
    [Authorize(Roles = "ReadDataLock")]
    public class DataLockController : ApiController
    {
        private const int PageSize = 250;

        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public DataLockController(IMediator mediator, IMapper mapper, ILogger logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

        [Route("", Name = "DataLockEventsList")]
        [HttpGet]
        public async Task<IHttpActionResult> GetDataLockEvents(long sinceEventId = 0, DateTime? sinceTime = null, string employerAccountId = null, long ukprn = 0, int pageNumber = 1)
        {
            try
            {
                _logger.Debug($"Processing GetDataLockEvents, sinceEventId={sinceEventId}, sinceTime={sinceTime}, employerAccountId={employerAccountId}, ukprn={ukprn}, pageNumber={pageNumber}");

                var queryResponse = await _mediator.SendAsync(new GetDataLockEventsQueryRequest
                {
                    SinceEventId = sinceEventId,
                    SinceTime = sinceTime,
                    EmployerAccountId = employerAccountId,
                    Ukprn = ukprn,
                    PageNumber = pageNumber,
                    PageSize = PageSize
                });

                if (!queryResponse.IsValid)
                {
                    throw queryResponse.Exception;
                }

                return Ok(_mapper.Map<PageOfResults<DataLockEvent>>(queryResponse.Result));
            }
            catch (ValidationException ex)
            {
                _logger.Info($"Bad request received to GetDataLockEvents - {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Unexpected error processing GetDataLockEvents - {ex.Message}");
                return InternalServerError();
            }
        }
    }
}