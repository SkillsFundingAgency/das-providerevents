using System;
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
        private readonly PopulateTables _populate;

        public DatabaseSetup(PopulateTables populate)
        {
            _populate = populate;
        }

        public async Task PopulateTestData()
        {
            if (await _populate.AreTablesPopulated().ConfigureAwait(false))
                await ReadAllData().ConfigureAwait(false);
            else
                await PopulateAllData().ConfigureAwait(false);

            if (await _populate.IsSubmissionEventsTablePopulatedAsync())
                await ReadSubmissionEventsAsync();
            else
                await PopulateSubmissionEventsAsync();
        }

        private async Task PopulateSubmissionEventsAsync()
        {
            Debug.WriteLine("Populating submission events table, this could take a while");

            TestData.SubmissionEvents = new Fixture().CreateMany<ItSubmissionEvent>(10).ToList();
            foreach (var @event in TestData.SubmissionEvents)
            {
                // these fields are just date's in the db
                @event.ActualStartDate = @event.ActualStartDate.Value.Date;
                @event.ActualEndDate = @event.ActualEndDate.Value.Date;
                @event.PlannedEndDate = @event.PlannedEndDate.Value.Date;
                // these fields are datetime's in the db, so can't match the precision of c#'s datetime, so we chop off the milliseconds
                @event.SubmittedDateTime = @event.SubmittedDateTime.AddTicks(-(@event.SubmittedDateTime.Ticks % TimeSpan.TicksPerSecond));
                @event.FileDateTime = @event.FileDateTime.AddTicks(-(@event.FileDateTime.Ticks % TimeSpan.TicksPerSecond));
            }
            await _populate.BulkInsertSubmissionEvents(TestData.SubmissionEvents);
        }

        private async Task PopulateAllData()
        {
            Debug.WriteLine("Populating tables, this could take a while");

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

        private async Task ReadSubmissionEventsAsync()
        {
            const string submissionEventsSql = "SELECT * FROM [Submissions].[SubmissionEvents]";

            using (var conn = DatabaseConnection.Connection())
            {
                await conn.OpenAsync();
                TestData.SubmissionEvents = (await conn.QueryAsync<ItSubmissionEvent>(submissionEventsSql)).ToList();
            }
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
