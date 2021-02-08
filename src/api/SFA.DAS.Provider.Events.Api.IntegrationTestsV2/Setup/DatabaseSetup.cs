using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Ploeh.AutoFixture;
using SFA.DAS.Provider.Events.Api.IntegrationTestsV2.DatabaseAccess;
using SFA.DAS.Provider.Events.Api.IntegrationTestsV2.EntityBuilders.Customisations;
using SFA.DAS.Provider.Events.Api.IntegrationTestsV2.RawEntities;

namespace SFA.DAS.Provider.Events.Api.IntegrationTestsV2.Setup
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

            if (await _populate.IsSubmissionEventsTablePopulated())
            {
                await ReadSubmissionEvents();
                await ReadSubmissionEventsForUlnCheck();
            }
            else
                await PopulateSubmissionEvents();
        }

        private async Task PopulateSubmissionEvents()
        {
            Debug.WriteLine("Populating submission events table, this could take a while");
            
            await PopulateEventsForByUln(1002105691, 2);
            await PopulateEventsForByUln(1002105888, 1);

            TestData.SubmissionEvents = new Fixture().CreateMany<ItSubmissionEvent>(10).ToList();
            foreach (var @event in TestData.SubmissionEvents)
            {
                @event.Uln = 0;
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

        private async Task PopulateEventsForByUln(long uln, int count)
        {
            TestData.SubmissionEventsForUln = new Fixture().CreateMany<ItSubmissionEvent>(count).ToList();
            foreach (var @event in TestData.SubmissionEventsForUln)
            {
                @event.Uln = uln;
                // these fields are just date's in the db
                @event.ActualStartDate = @event.ActualStartDate.Value.Date;
                @event.ActualEndDate = @event.ActualEndDate.Value.Date;
                @event.PlannedEndDate = @event.PlannedEndDate.Value.Date;
                // these fields are datetime's in the db, so can't match the precision of c#'s datetime, so we chop off the milliseconds
                @event.SubmittedDateTime =
                    @event.SubmittedDateTime.AddTicks(-(@event.SubmittedDateTime.Ticks % TimeSpan.TicksPerSecond));
                @event.FileDateTime = @event.FileDateTime.AddTicks(-(@event.FileDateTime.Ticks % TimeSpan.TicksPerSecond));
            }
            await _populate.BulkInsertSubmissionEvents(TestData.SubmissionEventsForUln);
        }

        private async Task PopulateAllData()
        {
            Debug.WriteLine("Populating tables, this could take a while");

            var data = _populate;

            var requiredPayments = new IEnumerable<ItRequiredPayment>[8];
            var fixture = new Fixture().Customize(new IntegrationTestCustomisation());
            Parallel.For(0, 8, i => requiredPayments[i] = fixture.CreateMany<ItRequiredPayment>(2500));

            TestData.RequiredPayments = requiredPayments.SelectMany(p => p).ToList();
            TestData.Payments = TestData.RequiredPayments.SelectMany(x => x.Payments).ToList();
            //TestData.Earnings = TestData.Payments.SelectMany(x => x.Earnings).ToList();
            TestData.Transfers = TestData.RequiredPayments.SelectMany(x => x.Transfers).ToList();

            for (int i = 0; i < TestData.Transfers.Count; i++)
                TestData.Transfers[i].TransferId = i;

            await data.BulkInsertPayments(TestData.Payments).ConfigureAwait(false);
            await data.BulkInsertEarnings(TestData.Earnings).ConfigureAwait(false);
            await data.BulkInsertRequiredPayments(TestData.RequiredPayments).ConfigureAwait(false);
            await data.BulkInsertTransfers(TestData.Transfers).ConfigureAwait(false);
            await data.CreatePeriods().ConfigureAwait(false);
        }

        private async Task ReadSubmissionEvents()
        {
            const string submissionEventsSql = "SELECT * FROM [Submissions].[SubmissionEvents] WHERE Uln = 0";

            using (var conn = DatabaseConnection.Connection())
            {
                await conn.OpenAsync();
                TestData.SubmissionEvents = (await conn.QueryAsync<ItSubmissionEvent>(submissionEventsSql)).ToList();
            }
        }

        private async Task ReadSubmissionEventsForUlnCheck()
        {
            const string submissionEventsSql = "SELECT * FROM [Submissions].[SubmissionEvents] WHERE Uln != 0";

            using (var conn = DatabaseConnection.Connection())
            {
                await conn.OpenAsync();
                TestData.SubmissionEventsForUln = (await conn.QueryAsync<ItSubmissionEvent>(submissionEventsSql)).ToList();
            }
        }

        private async Task ReadAllData()
        {
            var paymentsSql = "SELECT * FROM [Payments2].[Payment]";
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
