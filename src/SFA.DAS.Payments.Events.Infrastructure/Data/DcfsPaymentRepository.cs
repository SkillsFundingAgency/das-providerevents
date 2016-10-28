using System;
using System.Threading.Tasks;
using SFA.DAS.Payments.Events.Domain.Data;
using SFA.DAS.Payments.Events.Domain.Data.Entities;

namespace SFA.DAS.Payments.Events.Infrastructure.Data
{
    public class DcfsPaymentRepository : DcfsRepository, IPaymentRepository
    {
        private const string Source = "Payments.Payments";
        private const string Columns = "CAST(PaymentId as varchar(36)) [Id], "
                                     + "CommitmentId [ApprenticeshipId], "
                                     + "CommitmentVersion [ApprenticeshipVersion], "
                                     + "Ukprn, "
                                     + "Uln, "
                                     + "AccountId [EmployerAccountId], "
                                     + "AccountVersion [EmployerAccountVersion], "
                                     + "DeliveryMonth [DeliveryPeriodMonth], "
                                     + "DeliveryYear [DeliveryPeriodYear], "
                                     + "CollectionPeriodName [CollectionPeriodId], "
                                     + "CollectionPeriodMonth, "
                                     + "CollectionPeriodYear, "
                                     + "IlrSubmissionDate [EvidenceSubmittedOn], "
                                     + "FundingSource, "
                                     + "TransactionType, "
                                     + "Amount";
        private const string CountColumn = "COUNT(PaymentId)";
        private const string Pagination = "ORDER BY CollectionPeriodYear, CollectionPeriodMonth OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

        public async Task<PageOfEntities<PaymentEntity>> GetPayments(int page, int pageSize)
        {
            return await GetPageOfPayments(string.Empty, page, pageSize);
        }

        public async Task<PageOfEntities<PaymentEntity>> GetPaymentsForPeriod(int collectionPeriodYear, int collectionPeriodMonth, int page, int pageSize)
        {
            var whereClause = $"WHERE CollectionPeriodYear = {collectionPeriodYear} AND CollectionPeriodMonth = {collectionPeriodMonth}";

            return await GetPageOfPayments(whereClause, page, pageSize);
        }

        public async Task<PageOfEntities<PaymentEntity>> GetPaymentsForAccount(string employerAccountId, int page, int pageSize)
        {
            var whereClause = $"WHERE AccountId = '{employerAccountId.Replace("'", "''")}'";

            return await GetPageOfPayments(whereClause, page, pageSize);
        }

        public async Task<PageOfEntities<PaymentEntity>> GetPaymentsForAccountInPeriod(string employerAccountId, int collectionPeriodYear, int collectionPeriodMonth,
            int page, int pageSize)
        {
            var whereClause = $"WHERE AccountId = '{employerAccountId.Replace("'", "''")}' AND CollectionPeriodYear = {collectionPeriodYear} AND CollectionPeriodMonth = {collectionPeriodMonth}";

            return await GetPageOfPayments(whereClause, page, pageSize);
        }


        private async Task<PageOfEntities<PaymentEntity>> GetPageOfPayments(string whereClause, int page, int pageSize)
        {
            var numberOfPages = await GetNumberOfPages(whereClause, pageSize);

            var payments = await GetPayments(whereClause, page, pageSize);

            return new PageOfEntities<PaymentEntity>
            {
                PageNumber = page,
                TotalNumberOfPages = numberOfPages,
                Items = payments
            };
        }
        private async Task<PaymentEntity[]> GetPayments(string whereClause, int page, int pageSize)
        {
            var command = $"SELECT {Columns} FROM {Source} {whereClause} {Pagination}";

            var offset = (page - 1) * pageSize;
            return await Query<PaymentEntity>(command, new { offset, pageSize });
        }
        private async Task<int> GetNumberOfPages(string whereClause, int pageSize)
        {
            var command = $"SELECT {CountColumn} FROM {Source} {whereClause}";
            var count = await QuerySingle<int>(command);

            return (int)Math.Ceiling(count / (float)pageSize);
        }
    }
}
