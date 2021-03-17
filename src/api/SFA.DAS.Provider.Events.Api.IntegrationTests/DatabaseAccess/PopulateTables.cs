using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using FastMember;
using SFA.DAS.Provider.Events.Api.IntegrationTests.RawEntities;

namespace SFA.DAS.Provider.Events.Api.IntegrationTests.DatabaseAccess
{
    internal class PopulateTables
    {
        public static async Task<bool> IsSubmissionEventsTablePopulated()
        {
            using (var conn = DatabaseConnection.Connection())
            {
                const string sql = "SELECT Count(1) FROM [Submissions].[SubmissionEvents]";
                var result = await conn.ExecuteScalarAsync<int>(sql).ConfigureAwait(false);
                return result > 0;
            }
        }

        public static async Task BulkInsertSubmissionEvents(List<ItSubmissionEvent> submissionEvents)
        {
            using (var conn = DatabaseConnection.Connection())
            {
                await conn.OpenAsync();
                using (var bcp = new SqlBulkCopy(conn))
                using (var reader = ObjectReader.Create(submissionEvents, "Id", "IlrFileName",
                    "FileDateTime", "SubmittedDateTime", "ComponentVersionNumber", "Ukprn",
                    "Uln", "LearnRefNumber", "AimSeqNumber", "PriceEpisodeIdentifier",
                    "StandardCode", "ProgrammeType", "FrameworkCode", "PathwayCode",
                    "ActualStartDate", "PlannedEndDate", "ActualEndDate",
                    "OnProgrammeTotalPrice",
                    "CompletionTotalPrice", "NiNumber",
                    "CommitmentId", "AcademicYear",
                    "EmployerReferenceNumber", "EPAOrgId", "GivenNames", "FamilyName", "CompStatus"))
                {
                    bcp.DestinationTableName = "[Submissions].[SubmissionEvents]";
                    await bcp.WriteToServerAsync(reader);
                }
            }
        }
    }
}
