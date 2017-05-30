using System.Threading.Tasks;
using SFA.DAS.Provider.Events.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.Domain.Data
{
    public interface IPaymentRepository
    {
        Task<PageOfEntities<PaymentEntity>> GetPayments(int page, int pageSize, string employerAccountId= null, int? collectionPeriodYear = null, int? collectionPeriodMonth = null, long? ukprn = null);

    }
}