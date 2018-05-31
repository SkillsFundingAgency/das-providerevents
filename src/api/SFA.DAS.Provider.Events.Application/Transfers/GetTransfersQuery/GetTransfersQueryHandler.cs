using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Mapping;
using SFA.DAS.Provider.Events.Application.Repositories;

namespace SFA.DAS.Provider.Events.Application.Transfers.GetTransfersQuery
{
    public class GetTransfersQueryHandler : IAsyncRequestHandler<GetTransfersQueryRequest, GetTransfersQueryResponse>
    {
        private readonly ITransferRepository _transferRepository;
        private readonly IMapper _mapper;

        public GetTransfersQueryHandler(ITransferRepository transferRepository, IMapper mapper)
        {
            _transferRepository = transferRepository;
            _mapper = mapper;
        }

        public async Task<GetTransfersQueryResponse> Handle(GetTransfersQueryRequest message)
        {
            try
            {
                var transfers = await _transferRepository.GetTransfers(
                        message.PageNumber,
                        message.PageSize,
                        message.SenderAccountId,
                        message.ReceiverAccountId,
                        message.CollectionPeriodName)
                    .ConfigureAwait(false);

                var result = _mapper.Map<PageOfResults<AccountTransfer>>(transfers);

                return new GetTransfersQueryResponse
                {
                    IsValid = true,
                    Result = result,
                };
            }
            catch (Exception ex)
            {
                return new GetTransfersQueryResponse
                {
                    IsValid = false,
                    Exception = ex,
                };
            }
        }
    }
}