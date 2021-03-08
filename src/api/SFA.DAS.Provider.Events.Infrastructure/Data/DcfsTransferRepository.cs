using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data.Entities;
using SFA.DAS.Provider.Events.Application.Repositories;
using SFA.DAS.Provider.Events.Infrastructure.Extensions;


namespace SFA.DAS.Provider.Events.Infrastructure.Data
{
    public class DcfsTransferRepository : DcfsRepository, ITransferRepository
    {
        // Using CTE for 2 reasons:
        //  Get the column count at the same time as the query
        //  Restrict the query to PageSize rows for the main query before joining the earnings data
        private const string SqlTemplate = @"
            WITH _data AS (
                SELECT [Id]
                      ,[TransferSenderAccountId]
                      ,[AccountId]
                      ,[RequiredPaymentEventId]
                      ,[ApprenticeshipId]
                      ,[Amount]
                      ,[AcademicYear]
                      ,[CollectionPeriod]
                  FROM [Payments2].[Payment]
                /**where**/ -- Do not remove. Essential for SqlBuilder
            ),
            _count AS (
                SELECT COUNT(1) AS TotalCount FROM _data
            ) 
            SELECT * FROM 
            (
                SELECT * FROM _data CROSS APPLY _count 
                ORDER BY [Id]
                OFFSET (@PageIndex - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY 
            ) AS DATA ";

        private static void BuildQueryParameters(long? senderAccountId, long? receiverAccountId, int? academicYear, int? collectionPeriod, SqlBuilder sqlBuilder)
        {
            //Migration App has this filter for Transfer payments Do we need to add them here as well?
            //if (paymentModel.TransferSenderAccountId.HasValue && 
            //paymentModel.ApprenticeshipId.HasValue && 
            //paymentModel.AccountId.HasValue &&
            //paymentModel.FundingSource == 5)
            
            sqlBuilder.Where("FundingSource == 5");

            sqlBuilder.Where(
                "p.AcademicYear = @AcademicYear AND p.CollectionPeriod = @CollectionPeriod",
                new { AcademicYear = academicYear, CollectionPeriod = collectionPeriod },
                includeIf: academicYear.HasValue && collectionPeriod.HasValue);

            sqlBuilder.Where(
                "SendingAccountId = @senderAccountId",
                new {senderAccountId},
                includeIf: senderAccountId.HasValue);

            sqlBuilder.Where(
                "ReceivingAccountId = @receiverAccountId",
                new {receiverAccountId},
                includeIf: receiverAccountId.HasValue);
        }

        public async Task<PageOfResults<TransferEntity>> GetTransfers(
            int page, int pageSize,
            long? senderAccountId = null,
            long? receiverAccountId = null,
            int? academicYear = null, 
            int? collectionPeriod = null)
        {
            var sqlBuilder = new SqlBuilder();
            var query = sqlBuilder.AddTemplate(SqlTemplate);

            BuildQueryParameters(senderAccountId, receiverAccountId, academicYear, collectionPeriod, sqlBuilder);
            sqlBuilder.AddParameters(new {PageIndex = page, PageSize = pageSize});

            var result = await Query<TransferEntity>(query.RawSql, query.Parameters).ConfigureAwait(false);
            var pagedResults = PageResults(result.ToList(), page, pageSize);
            return pagedResults;
        }
    }
}
