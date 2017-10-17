using System;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.Provider.Events.Domain.Data;
using SFA.DAS.Provider.Events.Domain.Data.Entities;
using SFA.DAS.Provider.Events.Infrastructure.Extensions;

namespace SFA.DAS.Provider.Events.Infrastructure.Data
{
    public class DcfsPaymentRepository : DcfsRepository, IPaymentRepository
    {
        private const string SqlTemplate = ";WITH _data AS (" +
                                           "" +
                                           "    SELECT " +
                                           "    CAST(p.PaymentId as varchar(36)) [Id], " +
                                           "    rp.Id [RequiredPaymentId], " +
                                           "    rp.CommitmentId [ApprenticeshipId], " +
                                           "    rp.CommitmentVersionId [ApprenticeshipVersion], " +
                                           "    rp.Ukprn, " +
                                           "    rp.Uln, " +
                                           "    rp.AccountId [EmployerAccountId], " +
                                           "    rp.AccountVersionId [EmployerAccountVersion], " +
                                           "    p.DeliveryMonth [DeliveryPeriodMonth], " +
                                           "    p.DeliveryYear [DeliveryPeriodYear], " +
                                           "    p.CollectionPeriodName [CollectionPeriodId], " +
                                           "    p.CollectionPeriodMonth, " +
                                           "    p.CollectionPeriodYear, " +
                                           "    rp.IlrSubmissionDateTime [EvidenceSubmittedOn], " +
                                           "    p.FundingSource, " +
                                           "    p.TransactionType, " +
                                           "    p.Amount, " +
                                           "    rp.StandardCode, " +
                                           "    rp.FrameworkCode, " +
                                           "    rp.ProgrammeType, " +
                                           "    rp.PathwayCode, " +
                                           "    rp.ApprenticeshipContractType [ContractType]" +
                                           "    FROM Payments.Payments p " +
                                           "    INNER JOIN PaymentsDue.RequiredPayments rp ON p.RequiredPaymentId = rp.Id " +
                                           "    " +
                                           "    /**where**/ " +
                                           "    " +
                                           "),      " +
                                           "    _count AS(" +
                                           "        SELECT COUNT(1) AS TotalCount FROM _data" +
                                           ") " +
                                           "" +
                                           "SELECT * FROM (" +
                                           "    SELECT * FROM _data CROSS APPLY _count " +
                                           "    ORDER BY p.PaymentId " +
                                           "    OFFSET @PageIndex * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY " +
                                           ") AS DATA " +
                                           "LEFT OUTER JOIN PaymentsDue.Earnings e " +
                                           "    ON e.RequiredPaymentId = DATA.RequiredPaymentId ";

        public async Task<PageOfEntities<PaymentEntity>> GetPayments(int page, int pageSize, string employerAccountId, int? collectionPeriodYear, int? collectionPeriodMonth, long? ukprn)
        {
            var sqlBuilder = new SqlBuilder();
            var query = sqlBuilder.AddTemplate(SqlTemplate);

            // Query parameters
            sqlBuilder.Where(collectionPeriodYear.HasValue && collectionPeriodMonth.HasValue,
                "p.CollectionPeriodYear = @CollectionPeriodYear AND p.CollectionPeriodMonth = @CollectionPeriodMonth",
                new {CollectionPeriodYear = collectionPeriodYear, CollectionPeriodMonth = collectionPeriodMonth});
            
            sqlBuilder.Where(!string.IsNullOrEmpty(employerAccountId),
                "rp.AccountId = @EmployerAccountId",
                new {EmployerAccountId = employerAccountId?.Replace("'", "''")});

            sqlBuilder.Where(ukprn.HasValue, "rp.Ukprn = @UkPrn", new {UkPrn = ukprn});

            // Add the paging info 
            // TODO: Consider putting this in a decorator??
            sqlBuilder.AddParameters(new {PageIndex = page, PageSize = pageSize});

            var results = await Query<PaymentEntity>(query.RawSql, query.Parameters).ConfigureAwait(false);

            var returnValue = new PageOfEntities<PaymentEntity>
            {
                PageNumber = page,
                TotalNumberOfPages = 0,
                Items = results,
            };

            if (results.Length != 0)
            {
                returnValue.TotalNumberOfPages = (int)Math.Ceiling(((dynamic)results[0]).TotalCount / (float)pageSize);
            }

            return returnValue;
        }
    }
}
