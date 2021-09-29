using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using MediatR;
using NLog;
using SFA.DAS.Provider.Events.Api.ObsoleteModels;
using SFA.DAS.Provider.Events.Api.Plumbing.WebApi;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.DataLock.GetDataLockEventsQuery;
using SFA.DAS.Provider.Events.Application.Mapping;
using SFA.DAS.Provider.Events.Application.Validation;

namespace SFA.DAS.Provider.Events.Api.Controllers
{
    [AuthorizeRemoteOnly(Roles = "ReadDataLock")]
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

        [VersionedRoute("api/datalock", 1, Name = "DataLockEventsList")]
        [Route("api/v1/datalock", Name = "DataLockEventsListV1")]
        [HttpGet]
        public async Task<IHttpActionResult> GetDataLockEventsV1(long sinceEventId = 0, DateTime? sinceTime = null, string employerAccountId = null, long ukprn = 0, int pageNumber = 1)
        {
            var result = await GetDataLockEventsV2(sinceEventId, sinceTime, employerAccountId, ukprn, pageNumber)
                .ConfigureAwait(false);
            if (result.GetType() != typeof(OkNegotiatedContentResult<PageOfResults<DataLockEvent>>))
            {
                return result;
            }

            var v2Result = ((OkNegotiatedContentResult<PageOfResults<DataLockEvent>>)result).Content;
            return Ok(new PageOfResults<DataLockEventV1>
            {
                PageNumber = v2Result.PageNumber,
                TotalNumberOfPages = v2Result.TotalNumberOfPages,
                Items = _mapper.Map<DataLockEventV1[]>(v2Result.Items.Where(x => x.Status != EventStatus.Removed).ToArray())
            });
        }

        [VersionedRoute("api/datalock", 2, Name = "DataLockEventsListV2H")]
        [Route("api/v2/datalock", Name = "DataLockEventsListV2")]
        [HttpGet]
        public async Task<IHttpActionResult> GetDataLockEventsV2(
            long sinceEventId = 0, 
            DateTime? sinceTime = null, 
            string employerAccountId = null, 
            long ukprn = 0, 
            int pageNumber = 1)
        {
            try
            {
                _logger.Debug($"Processing GetDataLockEvents, sinceEventId={sinceEventId}, " +
                              $"sinceTime={sinceTime}, employerAccountId={employerAccountId}, " +
                              $"ukprn={ukprn}, pageNumber={pageNumber}");

                var queryResponse = await _mediator.SendAsync(new GetDataLockEventsQueryRequest
                {
                    SinceEventId = sinceEventId,
                    SinceTime = sinceTime,
                    EmployerAccountId = employerAccountId,
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