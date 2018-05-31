using System.Threading.Tasks;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data.Entities;

namespace SFA.DAS.Provider.Events.Application.Repositories
{
    public interface ITransferRepository
    {
        Task<PageOfResults<TransferEntity>> GetTransfers(
            int page, int pageSize,
            long? senderAccountId = null,
            long? receiverAccountId = null,
            string collectionPeriodName = null);
    }
}