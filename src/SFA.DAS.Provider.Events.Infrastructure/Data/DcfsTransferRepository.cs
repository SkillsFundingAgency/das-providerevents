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
    public class DcfsTransferRepository : DcfsRepository, ITransferRepository
    {
        private const string SqlTemplate = @"
                SELECT P.[Id]
                      ,P.[TransferSenderAccountId]
                      ,P.[AccountId]
                      ,P.[RequiredPaymentEventId]
                      ,P.[ApprenticeshipId]
                      ,P.[Amount]
                      ,P.[AcademicYear]
                      ,P.[CollectionPeriod]
                FROM [Payments2].[Payment] P
                /**where**/ -- Do not remove. Essential for SqlBuilder
                ORDER BY [Id]
                OFFSET (@PageIndex - 1) * @PageSize ROWS 
                FETCH NEXT @PageSize ROWS ONLY";

        private const string CountSqlTemplate = @"SELECT COUNT(1) [TotalCount] FROM [Payments2].[Payment] P /**where**/";

        public async Task<PageOfResults<TransferEntity>> GetTransfers(
            int page, int pageSize,
            long? senderAccountId = null,
            long? receiverAccountId = null,
            int? academicYear = null, 
            int? collectionPeriod = null)
        {
            using (var connection = await GetOpenConnection().ConfigureAwait(false))
            {
                var result = await GetTransfers(page, pageSize, senderAccountId, receiverAccountId, academicYear, collectionPeriod, connection);

                var count = await GetTransferCount(senderAccountId, receiverAccountId, academicYear, collectionPeriod, connection);

                var pagedResults = AddPagingInformation(result.ToList(), page, pageSize, count);

                return pagedResults;
            }
        }

        private async Task<IEnumerable<TransferEntity>> GetTransfers(int page, int pageSize, long? senderAccountId, long? receiverAccountId, int? academicYear, int? collectionPeriod, SqlConnection connection)
        {
            var sqlBuilder = new SqlBuilder();
            var query = sqlBuilder.AddTemplate(SqlTemplate);

            BuildQueryParameters(senderAccountId, receiverAccountId, academicYear, collectionPeriod, sqlBuilder);

            AddPagingQueryParameters(page, pageSize, sqlBuilder);

            var result = await connection.QueryAsync<TransferEntity>(query.RawSql, query.Parameters).ConfigureAwait(false);

            return result;
        }

        private static async Task<int> GetTransferCount(long? senderAccountId, long? receiverAccountId, int? academicYear, int? collectionPeriod, SqlConnection connection)
        {
            var countSqlBuilder = new SqlBuilder();
            var countQuery = countSqlBuilder.AddTemplate(CountSqlTemplate);

            BuildQueryParameters(senderAccountId, receiverAccountId, academicYear, collectionPeriod, countSqlBuilder);

            var count = await connection.ExecuteScalarAsync<int>(
                countQuery.RawSql,
                countQuery.Parameters
            ).ConfigureAwait(false);

            return count;
        }

        private static void AddPagingQueryParameters(int page, int pageSize, SqlBuilder sqlBuilder)
        {
            sqlBuilder.AddParameters(new { PageIndex = page, PageSize = pageSize });
        }

        private static void BuildQueryParameters(long? senderAccountId, long? receiverAccountId, int? academicYear, int? collectionPeriod, SqlBuilder sqlBuilder)
        {
            sqlBuilder.Where(
                "P.AcademicYear = @AcademicYear AND P.CollectionPeriod = @CollectionPeriod",
                new { AcademicYear = academicYear, CollectionPeriod = collectionPeriod },
                includeIf: academicYear.HasValue && collectionPeriod.HasValue);

            sqlBuilder.Where(
                "P.AccountId = @receiverAccountId",
                new {receiverAccountId}, includeIf: receiverAccountId.HasValue);

            sqlBuilder.Where(
                "P.TransferSenderAccountId = @senderAccountId",
                new {senderAccountId}, includeIf: senderAccountId.HasValue);

            sqlBuilder.Where("P.FundingSource = 5 AND P.TransferSenderAccountId IS NOT NULL AND P.ApprenticeshipId IS NOT NULL AND P.AccountId IS NOT NULL");
        }
    }
}
