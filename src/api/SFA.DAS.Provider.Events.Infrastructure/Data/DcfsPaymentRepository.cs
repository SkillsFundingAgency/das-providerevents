using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data.Entities;
using SFA.DAS.Provider.Events.Application.Repositories;
using SFA.DAS.Provider.Events.Infrastructure.Extensions;


namespace SFA.DAS.Provider.Events.Infrastructure.Data
{
    public class DcfsPaymentRepository : DcfsRepository, IPaymentRepository
    {
	    private const string SqlTemplate = @"
                SELECT
                    p.Id,
		            P.EventId,
		            P.RequiredPaymentEventId,
		            P.ApprenticeshipId, 
		            P.Ukprn, 
		            P.LearnerUln, 
		            P.AccountId, 
		            P.DeliveryPeriod, 
		            P.AcademicYear, 
		            P.CollectionPeriod, 
		            P.IlrSubmissionDateTime, 
		            P.FundingSource, 
		            P.TransactionType, 
		            P.Amount, 
		            P.LearningAimStandardCode, 
		            P.LearningAimFrameworkCode, 
		            P.LearningAimProgrammeType, 
		            P.LearningAimPathwayCode, 
		            P.ContractType,
                    P.EarningsStartDate,
					P.EarningsPlannedEndDate,
					P.EarningsActualEndDate,
					P.EarningsCompletionStatus,
					P.EarningsCompletionAmount,
					P.EarningsInstalmentAmount,
					P.EarningsNumberOfInstalments
	            FROM [Payments2].[Payment] P
                /**where**/ -- Do not remove. Essential for SqlBuilder
                ORDER BY Id
		        OFFSET (@PageIndex - 1) * @PageSize ROWS
		        FETCH NEXT @PageSize ROWS ONLY";

        private const string CountSqlTemplate = @"SELECT COUNT(1) [TotalCount] FROM [Payments2].[Payment] P /**where**/";

        public async Task<PaymentStatistics> GetStatistics()
        {
	        using (var connection = await GetOpenConnection().ConfigureAwait(false))
	        {
		        const string sql = "SELECT count(EventId) as TotalNumberOfPayments, count(RequiredPaymentEventId) as TotalNumberOfPaymentsWithRequiredPayment FROM Payments2.Payment";

		        return connection.Query<PaymentStatistics>(sql).FirstOrDefault();
	        }
        }

        public async Task<PageOfResults<PaymentEntity>> GetPayments(int page, int pageSize, string employerAccountId, int? academicYear, int? collectionPeriod, long? ukprn)
        {
            using (var connection = await GetOpenConnection().ConfigureAwait(false))
            {
	            var unPaginatedPayments = await GetUnPaginatedPayments(page, pageSize, employerAccountId, academicYear, collectionPeriod, ukprn, connection);

	            var count = await GetPaymentCount(employerAccountId, academicYear, collectionPeriod, ukprn, connection);

	            var pagedResults = PageResults(unPaginatedPayments.ToList(), page, pageSize, count);

                return pagedResults;
            }
        }

        private static async Task<IEnumerable<PaymentEntity>> GetUnPaginatedPayments(int page, int pageSize, string employerAccountId, int? academicYear, int? collectionPeriod, long? ukprn, SqlConnection connection)
        {
	        var sqlBuilder = new SqlBuilder();
	        var query = sqlBuilder.AddTemplate(SqlTemplate);

	        BuildQueryParameters(employerAccountId, academicYear, collectionPeriod, ukprn, sqlBuilder);

	        AddPagingQueryParameters(page, pageSize, sqlBuilder);

	        var result = await connection.QueryAsync<PaymentEntity>(
		        query.RawSql,
		        query.Parameters
	        ).ConfigureAwait(false);
	        
	        return result;
        }

        private static async Task<int> GetPaymentCount(string employerAccountId, int? academicYear, int? collectionPeriod, long? ukprn, SqlConnection connection)
        {
	        var countSQlBuilder = new SqlBuilder();
	        var countQuery = countSQlBuilder.AddTemplate(CountSqlTemplate);

	        BuildQueryParameters(employerAccountId, academicYear, collectionPeriod, ukprn, countSQlBuilder);

	        var count = await connection.QueryFirstOrDefaultAsync<int>(
		        countQuery.RawSql,
		        countQuery.Parameters
	        ).ConfigureAwait(false);

	        return count;
        }

        private static void AddPagingQueryParameters(int page, int pageSize, SqlBuilder sqlBuilder)
        {
            sqlBuilder.AddParameters(new { PageIndex = page, PageSize = pageSize });
        }

        private static void BuildQueryParameters(string accountId, int? academicYear, int? collectionPeriod, long? ukprn, SqlBuilder sqlBuilder)
        {
            sqlBuilder.Where("p.AcademicYear = @AcademicYear AND p.CollectionPeriod = @CollectionPeriod",
                new { AcademicYear = academicYear, CollectionPeriod = collectionPeriod },
                includeIf: academicYear.HasValue && collectionPeriod.HasValue);

            sqlBuilder.Where("p.AccountId = @accountId", 
				new { accountId = accountId?.Replace("'", "''") }, 
				includeIf: !string.IsNullOrEmpty(accountId));

            sqlBuilder.Where("p.Ukprn = @UkPrn", 
	            new { UkPrn = ukprn }, 
	            includeIf: ukprn.HasValue);
        }
    }
}
