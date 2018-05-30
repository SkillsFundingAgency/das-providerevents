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
                SELECT [TransferId]
                      ,[SendingAccountId]
                      ,[ReceivingAccountId]
                      ,[RequiredPaymentId]
                      ,[CommitmentId]
                      ,[Amount]
                      ,[TransferType]
                      ,[CollectionPeriodName]
                  FROM [TransferPayments].[AccountTransfers]
                /**where**/ -- Do not remove. Essential for SqlBuilder
            ),
            _count AS (
                SELECT COUNT(1) AS TotalCount FROM _data
            ) 
            SELECT * FROM 
            (
                SELECT * FROM _data CROSS APPLY _count 
                ORDER BY [TransferId]
                OFFSET (@PageIndex - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY 
            ) AS DATA ";

        private static void BuildQueryParameters(long? senderAccountId, long? receiverAccountId, string collectionPeriodName, SqlBuilder sqlBuilder)
        {
            sqlBuilder.Where(
                "CollectionPeriodName = @collectionPeriodName",
                new {collectionPeriodName},
                includeIf: !string.IsNullOrEmpty(collectionPeriodName));

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
            string collectionPeriodName = null)
        {
            var sqlBuilder = new SqlBuilder();
            var query = sqlBuilder.AddTemplate(SqlTemplate);

            BuildQueryParameters(senderAccountId, receiverAccountId, collectionPeriodName, sqlBuilder);
            sqlBuilder.AddParameters(new {PageIndex = page, PageSize = pageSize});

            var result = await Query<TransferEntity>(query.RawSql, query.Parameters).ConfigureAwait(false);
            var pagedResults = PageResults(result.ToList(), page, pageSize);
            return pagedResults;
        }
    }
}
