using System;
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
        public DcfsPaymentRepository()
            : base("PaymentsV2ConnectionString")
        {
        }

        // Using CTE for 2 reasons:
        //  Get the column count at the same time as the query
        //  Restrict the query to PageSize rows for the main query before joining the earnings data
        
        private const string SqlTemplate = @"
            WITH Payments AS (
	            SELECT 
		            CAST(P.EventId as varchar(36)) [Id],
		            P.RequiredPaymentEventId [RequiredPaymentId],
		            P.ApprenticeshipId [ApprenticeshipId], 
		            '' [ApprenticeshipVersion], 
		            P.Ukprn, 
		            P.LearnerUln [ULN], 
		            P.AccountId [EmployerAccountId], 
		            '' [EmployerAccountVersion], 
		            CASE WHEN DeliveryPeriod > 5 THEN DeliveryPeriod - 5 ELSE DeliveryPeriod + 7 END [DeliveryPeriodMonth], 
		            CAST(CASE WHEN DeliveryPeriod > 5 THEN 2000 + SUBSTRING(CAST(P.AcademicYear AS NVARCHAR), 3, 2) ELSE 2000 + SUBSTRING(CAST(P.AcademicYear AS NVARCHAR), 1, 2) END AS INT) [DeliveryPeriodYear], 
		            CONCAT(CAST(P.AcademicYear AS NVARCHAR), '-R', CASE WHEN P.CollectionPeriod < 10 THEN '0' END, CAST(P.CollectionPeriod AS NVARCHAR)) [CollectionPeriodId], 
		            CASE WHEN P.CollectionPeriod > 12 THEN P.CollectionPeriod - 4 WHEN P.CollectionPeriod > 5 THEN P.CollectionPeriod - 5 ELSE P.CollectionPeriod + 7 END [CollectionPeriodMonth], 
		            CAST(CASE WHEN P.CollectionPeriod > 5 THEN 2000 + SUBSTRING(CAST(P.AcademicYear AS NVARCHAR), 3, 2) ELSE 2000 + SUBSTRING(CAST(P.AcademicYear AS NVARCHAR), 1, 2) END AS INT) [CollectionPeriodYear], 
		            P.IlrSubmissionDateTime [EvidenceSubmittedOn], 
		            P.FundingSource, 
		            NULL AS FundingAccountId, 
		            P.TransactionType, 
		            P.Amount, 
		            P.LearningAimStandardCode [StandardCode], 
		            P.LearningAimFrameworkCode [FrameworkCode], 
		            P.LearningAimProgrammeType [ProgrammeType], 
		            P.LearningAimPathwayCode [PathwayCode], 
		            P.ContractType [ContractType],
                    P.EarningsStartDate,
					P.EarningsPlannedEndDate,
					P.EarningsActualEndDate,
					P.EarningsCompletionStatus,
					P.EarningsCompletionAmount,
					P.EarningsInstalmentAmount,
					P.EarningsNumberOfInstalments

	            FROM [Payments2].[Payment] P
                /**where**/ -- Do not remove. Essential for SqlBuilder
            )
            , _count AS (
	            SELECT COUNT(1) [TotalCount] FROM Payments
            )

            SELECT *
            FROM Payments 
            CROSS APPLY _count

            ORDER BY Id
            OFFSET (@PageIndex - 1) * @PageSize ROWS
            FETCH NEXT @PageSize ROWS ONLY";

        public async Task<PageOfResults<PaymentEntity>> GetPayments(int page, int pageSize, string employerAccountId, int? academicYear, int? collectionPeriod, long? ukprn)
        {
            var sqlBuilder = new SqlBuilder();
            var query = sqlBuilder.AddTemplate(SqlTemplate);

            BuildQueryParameters(employerAccountId, academicYear, collectionPeriod, ukprn, sqlBuilder);

            // TODO: Consider putting this in a decorator??
            AddPagingInformation(page, pageSize, sqlBuilder);

            using (var connection = await GetOpenConnection().ConfigureAwait(false))
            {
                var result = await connection.QueryAsync<PaymentEntity>(
                        query.RawSql,
                        query.Parameters
                    ).ConfigureAwait(false);

                var pagedResults = PageResults(result.ToList(), page, pageSize);
                return pagedResults;
            }
        }

        public async Task<PaymentStatistics> GetStatistics()
        {
            using (var connection = await GetOpenConnection().ConfigureAwait(false))
            {
                var sql =
                    "SELECT count(EventId) as TotalNumberOfPayments, count(RequiredPaymentEventId) as TotalNumberOfPaymentsWithRequiredPayment FROM Payments2.Payment";

                return connection.Query<PaymentStatistics>(sql).FirstOrDefault();
            }
        }

        private static void AddPagingInformation(int page, int pageSize, SqlBuilder sqlBuilder)
        {
            sqlBuilder.AddParameters(new { PageIndex = page, PageSize = pageSize });
        }

        private static void BuildQueryParameters(string employerAccountId, int? academicYear, int? collectionPeriod, long? ukprn, SqlBuilder sqlBuilder)
        {
            sqlBuilder.Where(
                "p.AcademicYear = @AcademicYear AND p.CollectionPeriod = @CollectionPeriod",
                new { AcademicYear = academicYear, CollectionPeriod = collectionPeriod },
                includeIf: academicYear.HasValue && collectionPeriod.HasValue);

            sqlBuilder.Where(
                "p.AccountId = @EmployerAccountId",
                new { EmployerAccountId = employerAccountId?.Replace("'", "''") },
                includeIf: !string.IsNullOrEmpty(employerAccountId));

            sqlBuilder.Where("p.Ukprn = @UkPrn", new { UkPrn = ukprn }, includeIf: ukprn.HasValue);
        }
    }
}
