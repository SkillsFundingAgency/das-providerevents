using System.Data.SqlClient;
using Dapper;
using SFA.DAS.Provider.Events.Submission.IntegrationTests.Data.Entities;
using SFA.DAS.Provider.Events.Submission.IntegrationTests.TestContext;

namespace SFA.DAS.Provider.Events.Submission.IntegrationTests.Data
{
    public static class LearnerRepository
    {
        public static void Create(LearnerEntity learner)
        {
            using (var connection = new SqlConnection(GlobalTestContext.Current.TransientDatabaseConnectionString))
            {
                connection.Execute("INSERT INTO Valid.Learner (LearnRefNumber, ULN, NINumber, LLDDHealthProb, Ethnicity, Sex) VALUES (@learnRefNumber, @uln, @niNumber, 2, 31, 'M')", learner);
            }
        }

        public static void Clean()
        {
            using (var connection = new SqlConnection(GlobalTestContext.Current.TransientDatabaseConnectionString))
            {
                connection.Execute("DELETE FROM Valid.Learner");
            }
        }
    }
}