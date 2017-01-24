using System.Threading.Tasks;
using SFA.DAS.Payments.Events.Domain.Data.Entities;

namespace SFA.DAS.Payments.Events.Domain.Data
{
    public interface IPaymentRepository
    {
        Task<PageOfEntities<PaymentEntity>> GetPayments(int page, int pageSize);
        Task<PageOfEntities<PaymentEntity>> GetPaymentsForPeriod(int collectionPeriodYear, int collectionPeriodMonth, int page, int pageSize);
        Task<PageOfEntities<PaymentEntity>> GetPaymentsForAccount(string employerAccountId, int page, int pageSize);
        Task<PageOfEntities<PaymentEntity>> GetPaymentsForAccountInPeriod(string employerAccountId, int collectionPeriodYear, int collectionPeriodMonth, int page, int pageSize);
    }
}