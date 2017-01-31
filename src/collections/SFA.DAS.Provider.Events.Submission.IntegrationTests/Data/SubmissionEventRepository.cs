using System.Data.SqlClient;
using System.Linq;
using Dapper;
using SFA.DAS.Provider.Events.Submission.IntegrationTests.Data.Entities;
using SFA.DAS.Provider.Events.Submission.IntegrationTests.TestContext;

namespace SFA.DAS.Provider.Events.Submission.IntegrationTests.Data
{
    public class SubmissionEventRepository
    {
        public static SubmissionEventEntity[] GetSubmissionEventsForProvider(long ukprn)
        {
            using (var connection = new SqlConnection(GlobalTestContext.Current.TransientDatabaseConnectionString))
            {
                return connection.Query<SubmissionEventEntity>("SELECT * FROM Submissions.SubmissionEvents WHERE UKPRN=@Ukprn", new { ukprn }).ToArray();
            }
        }
    }
}
