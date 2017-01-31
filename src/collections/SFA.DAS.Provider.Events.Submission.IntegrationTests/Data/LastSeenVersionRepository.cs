using System.Data.SqlClient;
using System.Linq;
using Dapper;
using SFA.DAS.Provider.Events.Submission.IntegrationTests.Data.Entities;
using SFA.DAS.Provider.Events.Submission.IntegrationTests.TestContext;

namespace SFA.DAS.Provider.Events.Submission.IntegrationTests.Data
{
    public static class LastSeenVersionRepository
    {
        public static LastSeenVersionEntity[] GetLastestVersionsForProvider(long ukprn)
        {
            using (var connection = new SqlConnection(GlobalTestContext.Current.TransientDatabaseConnectionString))
            {
                return connection.Query<LastSeenVersionEntity>("SELECT * FROM Submissions.LastSeenVersion WHERE UKPRN=@Ukprn", new { ukprn }).ToArray();
            }
        }

        public static void Create(LastSeenVersionEntity lastSeenVersion)
        {
            using (var connection = new SqlConnection(GlobalTestContext.Current.TransientDatabaseConnectionString))
            {
                connection.Execute("INSERT INTO Submissions.LastSeenVersion VALUES (@IlrFileName,@FileDateTime,@SubmittedDateTime," +
                                   "@ComponentVersionNumber,@UKPRN,@ULN,@LearnRefNumber,@AimSeqNumber,@PriceEpisodeIdentifier,@StandardCode," +
                                   "@ProgrammeType,@FrameworkCode,@PathwayCode,@ActualStartDate,@PlannedEndDate,@ActualEndDate," +
                                   "@OnProgrammeTotalPrice,@CompletionTotalPrice,@NINumber)", lastSeenVersion);
            }
        }
    }
}
