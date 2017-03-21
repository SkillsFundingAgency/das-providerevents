using System.Data.SqlClient;
using Dapper;
using SFA.DAS.Provider.Events.Submission.IntegrationTests.Data.Entities;
using SFA.DAS.Provider.Events.Submission.IntegrationTests.TestContext;

namespace SFA.DAS.Provider.Events.Submission.IntegrationTests.Data
{
    public class LearnerEmploymentStatusRepository
    {
        public static void Create(LearnerEmploymentStatusEntity employmentStatus)
        {
            using (var connection = new SqlConnection(GlobalTestContext.Current.TransientDatabaseConnectionString))
            {
                connection.Execute("INSERT INTO Valid.LearnerEmploymentStatus (LearnRefNumber, EmpStat, DateEmpStatApp, EmpId) " +
                                   "VALUES " +
                                   "(@LearnRefNumber, @EmploymentStatus, @EmploymentStatusDate, @EmployerId)",
                                   employmentStatus);
            }
        }

        public static void Clean()
        {
            using (var connection = new SqlConnection(GlobalTestContext.Current.TransientDatabaseConnectionString))
            {
                connection.Execute("DELETE FROM Valid.LearnerEmploymentStatus");
            }
        }
    }
}