using MediatR;
using SFA.DAS.Provider.Events.Application.Data;

namespace SFA.DAS.Provider.Events.Application.Transfers.GetTransfersQuery
{
    public class GetTransfersQueryRequest : IAsyncRequest<GetTransfersQueryResponse>
    {
        public CollectionPeriod Period { get; set; }
        public long? SenderAccountId { get; set; }
        public long? ReceiverAccountId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
