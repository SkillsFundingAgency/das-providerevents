using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data.Entities;
using SFA.DAS.Provider.Events.Application.Repositories;
using SFA.DAS.Provider.Events.Infrastructure.Extensions;
using SFA.DAS.Sql.Dapper;

namespace SFA.DAS.Provider.Events.Infrastructure.Data
{
    public class DcfsPaymentRepository : DcfsRepository, IPaymentRepository
    {
        private const string SqlTemplate = "WITH _data AS (" +
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
                                           "    /**where**/ " + // Do not remove. Essential for SqlBuilder
                                           "    " +
                                           "),      " +
                                           "    _count AS(" +
                                           "        SELECT COUNT(1) AS TotalCount FROM _data" +
                                           ") " +
                                           "" +
                                           "SELECT * FROM (" +
                                           "    SELECT * FROM _data CROSS APPLY _count " +
                                           "    ORDER BY p.PaymentId " +
                                           "    OFFSET (@PageIndex - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY " +
                                           ") AS DATA " +
                                           "LEFT OUTER JOIN PaymentsDue.Earnings e " +
                                           "    ON e.RequiredPaymentId = DATA.RequiredPaymentId ";

        public async Task<PageOfResults<PaymentEntity>> GetPayments(int page, int pageSize, string employerAccountId, int? collectionPeriodYear, int? collectionPeriodMonth, long? ukprn)
        {
            var sqlBuilder = new SqlBuilder();
            var query = sqlBuilder.AddTemplate(SqlTemplate);

            BuildQueryParameters(employerAccountId, collectionPeriodYear, collectionPeriodMonth, ukprn, sqlBuilder);

            // TODO: Consider putting this in a decorator??
            AddPagingInformation(page, pageSize, sqlBuilder);

            using (var connection = await GetOpenConnection().ConfigureAwait(false))
            {
                var mapper = new ParentChildrenMapper<PaymentEntity, PaymentsDueEarningEntity>();
                var lookup = new Dictionary<object, PaymentEntity>();

                var results = (await connection.QueryAsync(
                            query.RawSql,
                            param: query.Parameters,
                            map: mapper.Map(lookup, x => x.Id, x => x.PaymentsDueEarningEntities))
                        .ConfigureAwait(false))
                    .ToList();

                var returnValue = new PageOfResults<PaymentEntity>
                {
                    PageNumber = page,
                    TotalNumberOfPages = 0,
                    Items = results.ToArray(),
                };

                if (results.Count != 0)
                {
                    returnValue.TotalNumberOfPages = NumberOfPages(((dynamic)results[0]).TotalCount, pageSize);
                }

                return returnValue;
            }
        }

        private static void AddPagingInformation(int page, int pageSize, SqlBuilder sqlBuilder)
        {
            sqlBuilder.AddParameters(new {PageIndex = page, PageSize = pageSize});
        }

        private static void BuildQueryParameters(string employerAccountId, int? collectionPeriodYear,
            int? collectionPeriodMonth, long? ukprn, SqlBuilder sqlBuilder)
        {
            sqlBuilder.Where(
                "p.CollectionPeriodYear = @CollectionPeriodYear AND p.CollectionPeriodMonth = @CollectionPeriodMonth",
                new {CollectionPeriodYear = collectionPeriodYear, CollectionPeriodMonth = collectionPeriodMonth},
                includeIf: collectionPeriodYear.HasValue && collectionPeriodMonth.HasValue);

            sqlBuilder.Where(
                "rp.AccountId = @EmployerAccountId",
                new {EmployerAccountId = employerAccountId?.Replace("'", "''")},
                includeIf: !string.IsNullOrEmpty(employerAccountId));

            sqlBuilder.Where("rp.Ukprn = @UkPrn", new {UkPrn = ukprn}, includeIf: ukprn.HasValue);
        }
    }
}
