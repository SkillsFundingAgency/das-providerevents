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
                connection.Execute("INSERT INTO Valid.LearningDelivery (LearnRefNumber, LearnAimRef, AimType, AimSeqNumber, LearnStartDate, LearnPlanEndDate, LearnActEndDate, FundModel, ProgType, FworkCode, PwayCode, StdCode) " +
                                   "VALUES " +
                                   "(@LearnRefNumber, 'ZPROG001', 1, @AimSeqNumber, @LearnStartDate, @LearnPlanEndDate, @LearnActEndDate, 36, @ProgType, @FworkCode, @PwayCode, @StdCode)", 
                                   learningDelivery);
            }
        }

        public static void Clean()
        {
            using (var connection = new SqlConnection(GlobalTestContext.Current.TransientDatabaseConnectionString))
            {
                connection.Execute("DELETE FROM Valid.LearningDelivery");
            }
        }
    }
}
