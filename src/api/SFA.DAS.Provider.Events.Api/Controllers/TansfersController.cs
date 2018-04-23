using System;
using System.Threading.Tasks;
using System.Web.Http;
using MediatR;
using NLog;
using SFA.DAS.Provider.Events.Api.Plumbing.WebApi;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data;
using SFA.DAS.Provider.Events.Application.Period.GetPeriodQuery;
using SFA.DAS.Provider.Events.Application.Transfers.GetTransfersQuery;
using SFA.DAS.Provider.Events.Application.Validation;

namespace SFA.DAS.Provider.Events.Api.Controllers
{
    [AuthorizeRemoteOnly(Roles = "ReadPayments")]
    public class TansfersController : ApiController
    {
        private const int PageSize = 10000;
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public TansfersController(ILogger logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        [Route("api/transfers", Name = "GetTransfers")]
        public async Task<IHttpActionResult> GetTransfers(string periodId = null, long? senderAccountId = null, long? receiverAccountId = null, int page = 1)
        {
            try
            {
                if (!string.IsNullOrEmpty(periodId))
                {
                    var period = await GetPeriodAsync(periodId).ConfigureAwait(false);
                    if (period == null)
                    {
                        return Ok(new PageOfResults<AccountTransfer>
                        {
                            PageNumber = page,
                            TotalNumberOfPages = 0,
                            Items = new AccountTransfer[0]
                        });
                    }
                }

                var transfersQueryResponse = await GetTransfersInternal(senderAccountId, receiverAccountId, page, periodId).ConfigureAwait(false);

                return Ok(transfersQueryResponse.Result);
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

        private async Task<Period> GetPeriodAsync(string periodId)
        {
            var getPeriodResponse = await _mediator.SendAsync(new GetPeriodQueryRequest { PeriodId = periodId }).ConfigureAwait(false);
            if (!getPeriodResponse.IsValid)
                throw getPeriodResponse.Exception;
            return getPeriodResponse.Result;
        }

        private async Task<GetTransfersQueryResponse> GetTransfersInternal(long? senderAccountId, long? receiverAccountId, int page, string collectionPeriodName)
        {
            var transfersResponse = await _mediator.SendAsync(new GetTransfersQueryRequest
                {
                    CollectionPeriodName = collectionPeriodName,
                    SenderAccountId = senderAccountId,
                    ReceiverAccountId = receiverAccountId,
                    PageNumber = page,
                    PageSize = PageSize
                }).ConfigureAwait(false);

            if (!transfersResponse.IsValid)
            {
                throw transfersResponse.Exception;
            }

            return transfersResponse;
        }

    }
}
