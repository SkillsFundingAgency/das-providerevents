using System;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Provider.Events.Api.IntegrationTestsV2.DatabaseAccess
{
    public static class DatabaseCleanUp
    {
        public static async Task DeleteTestPaymentRecords()
        {
            var batches = new List<string>();

            var totalGuidsToDelete = TestData.Payments
                .Select(x => $"'{x.EventId}'")
                .ToArray();

            var batchSizeLimit = 1000;
            var remainder = totalGuidsToDelete.Length % batchSizeLimit;
            var totalNumberOfBatchesNeeded = totalGuidsToDelete.Length / batchSizeLimit + (remainder == 0 ? 0 : 1);

            await ExecuteSqlAsync($"DELETE FROM [Payments2].[CollectionPeriod] WHERE Period = {TestData.CollectionPeriod} AND AcademicYear = {TestData.AcademicYear}");

            for (var i = 1; i <= totalNumberOfBatchesNeeded; i++)
            {
                var recordsToSkip = (i - 1) * batchSizeLimit;

                var batchGuidsToDelete = totalGuidsToDelete
                    .Skip(recordsToSkip)
                    .Take(batchSizeLimit)
                    .ToArray();

                var csv = string.Join(",", batchGuidsToDelete);
                var sql = $"DELETE FROM [Payments2].[Payment] WHERE [EventId] IN ({csv});";

                batches.Add(sql);
            }

            await Task.WhenAll(batches.Select(ExecuteSqlAsync));
        }

        private static async Task ExecuteSqlAsync(string sql)
        {
            using (var connection = DatabaseConnection.Connection())
            {
                await connection.OpenAsync().ConfigureAwait(false);
                await connection.ExecuteAsync(sql);
            }
        }
    }
}