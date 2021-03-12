using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Ploeh.AutoFixture;
using SFA.DAS.Provider.Events.Api.IntegrationTests.DatabaseAccess;
using SFA.DAS.Provider.Events.Api.IntegrationTests.RawEntities;

namespace SFA.DAS.Provider.Events.Api.IntegrationTests.Setup
{
    internal class DatabaseSetup
    {
        public async Task PopulateTestData()
        {
            if (await PopulateTables.IsSubmissionEventsTablePopulated())
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
            await PopulateTables.BulkInsertSubmissionEvents(TestData.SubmissionEvents);
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
            await PopulateTables.BulkInsertSubmissionEvents(TestData.SubmissionEventsForUln);
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
    }
}
