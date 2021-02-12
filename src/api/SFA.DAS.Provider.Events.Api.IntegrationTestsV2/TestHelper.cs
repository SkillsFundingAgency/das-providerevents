using Dapper;
using SFA.DAS.Provider.Events.Api.IntegrationTestsV2.DatabaseAccess;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Provider.Events.Api.IntegrationTestsV2
{
    internal static class TestHelper
    {
        internal static async Task<int?> GetPaymentCount()
        {
            var sql = "SELECT Count(*)  FROM [Payments2].[Payment];";
            var result = await ExecuteSqlAsync<int?>(sql);
            return result.First();
        }

        internal static async Task<int?> GetPaymentWithRequiredPaymentCount()
        {
            var sql = "SELECT Count(*)  FROM [Payments2].[Payment] WHERE RequiredPaymentEventId IS NOT NUll;";
            var result = await ExecuteSqlAsync<int?>(sql);
            return result.First();
        }

        private static async Task<IEnumerable<TReturnType>> ExecuteSqlAsync<TReturnType>(string sql)
        {
            using (var connection = DatabaseConnection.Connection())
            {
                await connection.OpenAsync().ConfigureAwait(false);

                return await connection
                    .QueryAsync<TReturnType>(sql)
                    .ConfigureAwait(false);
            }
        }
    }
}