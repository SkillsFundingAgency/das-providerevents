using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using FastMember;
using SFA.DAS.Provider.Events.Api.IntegrationTestsV2.RawEntities;

namespace SFA.DAS.Provider.Events.Api.IntegrationTestsV2.DatabaseAccess
{
    public class PopulateTables
    {
        public async Task BulkInsertPayments(List<ItPayment> payments)
        {
            using (var conn = DatabaseConnection.Connection())
            {
                await conn.OpenAsync().ConfigureAwait(false);
                using (var bcp = new SqlBulkCopy(conn))
                using (var reader = ObjectReader.Create(payments, "Id", "EventId", "EarningEventId", 
                    "FundingSourceEventId", "RequiredPaymentEventId", "EventTime", "JobId", "DeliveryPeriod",
                    "CollectionPeriod", "AcademicYear", "Ukprn", "LearnerReferenceNumber",
                    "LearnerUln", "PriceEpisodeIdentifier", "Amount",
                    "LearningAimReference", "LearningAimProgrammeType", "LearningAimStandardCode", 
                    "LearningAimFrameworkCode", "LearningAimPathwayCode", "LearningAimFundingLineType", 
                    "ContractType", "TransactionType", "FundingSource", "IlrSubmissionDateTime", 
                    "SfaContributionPercentage", "AgreementId",  "AccountId", "TransferSenderAccountId", "CreationDate",
                    "EarningsStartDate", "EarningsPlannedEndDate", "EarningsActualEndDate",
                    "EarningsCompletionStatus", "EarningsCompletionAmount",
                    "EarningsInstalmentAmount", "EarningsNumberOfInstalments", "LearningStartDate",
                    "ApprenticeshipId", "ApprenticeshipPriceEpisodeId", "ApprenticeshipEmployerType", 
                    "ReportingAimFundingLineType", "NonPaymentReason", "DuplicateNumber"
                    ))
                {
                    bcp.DestinationTableName = "[Payments2].[Payment]";
                    await bcp.WriteToServerAsync(reader).ConfigureAwait(false);
                }
            }
        }
    }
}
