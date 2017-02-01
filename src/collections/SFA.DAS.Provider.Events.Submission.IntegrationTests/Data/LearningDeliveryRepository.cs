using System.Data.SqlClient;
using Dapper;
using SFA.DAS.Provider.Events.Submission.IntegrationTests.Data.Entities;
using SFA.DAS.Provider.Events.Submission.IntegrationTests.TestContext;

namespace SFA.DAS.Provider.Events.Submission.IntegrationTests.Data
{
    public class LearningDeliveryRepository
    {
        public static void Create(LearningDeliveryEntity learningDelivery)
        {
            using (var connection = new SqlConnection(GlobalTestContext.Current.TransientDatabaseConnectionString))
            {
                connection.Execute("INSERT INTO Reference.LearningDeliveries VALUES (@UKPRN,@LearnRefNumber,@AimSeqNumber,@ULN," +
                                   "@NINumber,@ProgType,@FworkCode,@PwayCode,@StdCode,@LearnStartDate,@LearnPlanEndDate,@LearnActEndDate)", 
                                   learningDelivery);
            }
        }
    }
}
