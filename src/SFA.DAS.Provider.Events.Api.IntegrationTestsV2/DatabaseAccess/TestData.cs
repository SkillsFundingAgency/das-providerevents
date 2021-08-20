﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.Provider.Events.Api.IntegrationTestsV2.RawEntities;

namespace SFA.DAS.Provider.Events.Api.IntegrationTestsV2.DatabaseAccess
{
    public static class TestData
    {
        public static long EmployerAccountId { get; set; }
        public static byte CollectionPeriod { get; set; }
        public static short AcademicYear { get; set; }

        /// <summary>
        /// List of test payments inserted into the [Payments2].[Payment] table during this test run.
        /// </summary>
        public static List<ItPayment> Payments { get; set; }

        public static List<ItPayment> TransferPayments
        {
            get
            {
                return Payments.Where(x => x.TransferSenderAccountId.HasValue).ToList();
            }
        }

        public static async Task<int?> GetPaymentCount()
        {
            var sql = "SELECT Count(1) FROM [Payments2].[Payment];";
            var result = await ExecuteSqlAsync<int?>(sql);
            return result.First();
        }

        public static async Task<int?> GetPeriodCount()
        {
            var sql = "SELECT Count(1) FROM [Payments2].[CollectionPeriod];";
            var result = await ExecuteSqlAsync<int?>(sql);
            return result.First();
        }

        public static async Task<int?> GetPaymentWithRequiredPaymentCount()
        {
            var sql = "SELECT Count(1) FROM [Payments2].[Payment] WHERE RequiredPaymentEventId IS NOT NUll;";
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
