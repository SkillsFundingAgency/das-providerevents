using System.Data.SqlClient;
using Dapper;
using SFA.DAS.Provider.Events.Submission.IntegrationTests.Data.Entities;
using SFA.DAS.Provider.Events.Submission.IntegrationTests.TestContext;

namespace SFA.DAS.Provider.Events.Submission.IntegrationTests.Data
{
    public static class FileDetailsRepository
    {
        public static void Create(FileDetailsEntity fileDetails)
        {
            using (var connection = new SqlConnection(GlobalTestContext.Current.TransientDatabaseConnectionString))
            {
                connection.Execute("INSERT INTO dbo.FileDetails (UKPRN, Filename, SubmittedTime) VALUES (@ukprn, @fileName, @submittedTime)", fileDetails);
            }
        }

        public static void Clean()
        {
            using (var connection = new SqlConnection(GlobalTestContext.Current.TransientDatabaseConnectionString))
            {
                connection.Execute("DELETE FROM dbo.FileDetails");
            }
        }
    }
}