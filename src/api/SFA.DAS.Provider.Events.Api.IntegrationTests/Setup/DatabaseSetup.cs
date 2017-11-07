using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Ploeh.AutoFixture;
using SFA.DAS.Provider.Events.Api.IntegrationTests.DatabaseAccess;
using SFA.DAS.Provider.Events.Api.IntegrationTests.EntityBuilders.Customisations;
using SFA.DAS.Provider.Events.Api.IntegrationTests.RawEntities;

namespace SFA.DAS.Provider.Events.Api.IntegrationTests.Setup
{
    class DatabaseSetup
    {
        private readonly CreateDatabase _create;
        private readonly PopulateTables _populate;

        public DatabaseSetup(
            CreateDatabase create, 
            PopulateTables populate)
        {
            _create = create;
            _populate = populate;
        }

        public async Task PopulateTestData()
        {
            if (await _populate.AreTablesPopulated().ConfigureAwait(false))
            {
                await ReadAllData().ConfigureAwait(false);
            }
            else
            {
                await PopulateAllData().ConfigureAwait(false);
            }
        }

        private async Task PopulateAllData()
        {
            Debug.WriteLine("Creating and populating tables, this could take a while");
            var database = _create;
            await database.Create().ConfigureAwait(false);

            var data = _populate;

            var fixture = new Fixture().Customize(new IntegrationTestCustomisation());
            var requiredPayments = fixture.CreateMany<ItRequiredPayment>(20000).ToList();

            TestData.RequiredPayments = requiredPayments;
            TestData.Payments = requiredPayments.SelectMany(x => x.Payments).ToList();
            TestData.Earnings = TestData.Payments.SelectMany(x => x.Earnings).ToList();

            await data.BulkInsertPayments(TestData.Payments).ConfigureAwait(false);
            await data.BulkInsertEarningsAsync(TestData.Earnings).ConfigureAwait(false);
            await data.BulkInsertRequiredPayments(TestData.RequiredPayments).ConfigureAwait(false);
            await data.CreatePeriods().ConfigureAwait(false);
        }

        private async Task ReadAllData()
        {
            var paymentsSql = "SELECT * FROM [Payments].[Payments]";
            var earningsSql = "SELECT * FROM [PaymentsDue].[Earnings]";
            var requiredPaymentsSql = "SELECT * FROM [PaymentsDue].[RequiredPayments]";

            using (var conn = DatabaseConnection.Connection())
            {
                await conn.OpenAsync().ConfigureAwait(false);
                var payments = await conn.QueryAsync<ItPayment>(paymentsSql)
                    .ConfigureAwait(false);
                var earnings = await conn.QueryAsync<ItEarning>(earningsSql)
                    .ConfigureAwait(false);
                var requiredPayments = await conn.QueryAsync<ItRequiredPayment>(requiredPaymentsSql)
                    .ConfigureAwait(false);

                TestData.Earnings = earnings.ToList();
                TestData.Payments = payments.ToList();
                TestData.RequiredPayments = requiredPayments.ToList();
            }
        }
    }
}
