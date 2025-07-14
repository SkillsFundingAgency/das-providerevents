using MediatR;
using Microsoft.ApplicationInsights;
using SFA.DAS.Provider.Events.Api.Plumbing.WebApi;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data;
using SFA.DAS.Provider.Events.Application.Period.GetPeriodQuery;
using SFA.DAS.Provider.Events.Application.Transfers.GetTransfersQuery;
using SFA.DAS.Provider.Events.Application.Validation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace SFA.DAS.Provider.Events.Api.Controllers
{
    [AuthorizeRemoteOnly(Roles = "ReadPayments")]
    public class TransfersController : ApiController
    {
        private const int PageSize = 10000;
        private readonly TelemetryClient _telemetryClient;
        private readonly IMediator _mediator;

        public TransfersController(TelemetryClient telemetryClient, IMediator mediator)
        {
            _telemetryClient = telemetryClient;
            _mediator = mediator;
        }

        [HttpGet]
        [Route("api/transfers", Name = "GetTransfers")]
        public async Task<IHttpActionResult> GetTransfers(string periodId = null, long? senderAccountId = null, long? receiverAccountId = null, int page = 1)
        {
            try
            {
                CollectionPeriod period = null;
                if (!string.IsNullOrEmpty(periodId))
                {
                    period = await GetPeriodAsync(periodId).ConfigureAwait(false);
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

                var transfersQueryResponse = await GetTransfersInternal(senderAccountId, receiverAccountId, page, period).ConfigureAwait(false);

                return Ok(transfersQueryResponse.Result);
            }
            catch (ValidationException ex)
            {
                _telemetryClient.TrackTrace(ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex, new Dictionary<string, string> { { "Message", ex.Message } });
                return InternalServerError();
            }
        }

        private async Task<CollectionPeriod> GetPeriodAsync(string periodId)
        {
            var getPeriodResponse = await _mediator.SendAsync(new GetPeriodQueryRequest { PeriodId = periodId }).ConfigureAwait(false);
            if (!getPeriodResponse.IsValid)
                throw getPeriodResponse.Exception;
            return getPeriodResponse.Result;
        }

        private async Task<GetTransfersQueryResponse> GetTransfersInternal(long? senderAccountId, long? receiverAccountId, int page, CollectionPeriod period)
        {
            var transfersResponse = await _mediator.SendAsync(new GetTransfersQueryRequest
                {
                    Period = period,
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
