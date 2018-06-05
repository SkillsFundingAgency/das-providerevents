using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using Dapper;
using FastMember;
using SFA.DAS.Provider.Events.Api.IntegrationTests.RawEntities;

namespace SFA.DAS.Provider.Events.Api.IntegrationTests.DatabaseAccess
{
    class PopulateTables
    {
        private readonly DatabaseConnection _connection;

        public PopulateTables(DatabaseConnection connection)
        {
            _connection = connection;
        }

        public async Task<bool> AreTablesPopulated()
        {
            using (var conn = DatabaseConnection.Connection())
            {
                var sql = "SELECT Count(1) FROM [PaymentsDue].[RequiredPayments]";
                var result = await conn.ExecuteScalarAsync<int>(sql).ConfigureAwait(false);
                return result > 10000;
            }
        }

        public async Task<bool> IsSubmissionEventsTablePopulated()
        {
            using (var conn = DatabaseConnection.Connection())
            {
                var sql = "SELECT Count(1) FROM [Submissions].[SubmissionEvents]";
                var result = await conn.ExecuteScalarAsync<int>(sql).ConfigureAwait(false);
                return result > 0;
            }
        }

        public async Task CreatePeriods()
        {
            await _connection.RunScriptfile(Path.Combine("SetupScripts", "DataSetup-Payments.Periods"))
                .ConfigureAwait(false);
        }

        public async Task BulkInsertPayments(List<ItPayment> payments)
        {
            using (var conn = DatabaseConnection.Connection())
            {
                await conn.OpenAsync().ConfigureAwait(false);
                using (var bcp = new SqlBulkCopy(conn))
                using (var reader = ObjectReader.Create(payments, "PaymentId", "RequiredPaymentId", "DeliveryMonth", 
                    "DeliveryYear", "CollectionPeriodName", "CollectionPeriodMonth", "CollectionPeriodYear", 
                    "FundingSource", "TransactionType", "Amount"))
                {
                    bcp.DestinationTableName = "[Payments].[Payments]";
                    await bcp.WriteToServerAsync(reader).ConfigureAwait(false);
                }
            }
        }

        public async Task BulkInsertTransfers(List<ItTransfer> transfers)
        {
            using (var conn = DatabaseConnection.Connection())
            {
                await conn.OpenAsync().ConfigureAwait(false);
                using (var bcp = new SqlBulkCopy(conn))
                using (var reader = ObjectReader.Create(transfers, "TransferId", "SendingAccountId", "ReceivingAccountId", 
                    "RequiredPaymentId", "CommitmentId",  "Amount", "TransferType", "CollectionPeriodName", "CollectionPeriodMonth", "CollectionPeriodYear"))
                {
                    bcp.DestinationTableName = "[AccountTransfers].[TransferPayments]";
                    await bcp.WriteToServerAsync(reader).ConfigureAwait(false);
                }
            }
        }

        public async Task BulkInsertEarnings(List<ItEarning> earnings)
        {
            using (var conn = DatabaseConnection.Connection())
            {
                await conn.OpenAsync().ConfigureAwait(false);
                using (var bcp = new SqlBulkCopy(conn))
                using (var reader = ObjectReader.Create(earnings, "RequiredPaymentId", "StartDate",
                    "PlannedEndDate", "ActualEndDate", "CompletionStatus", "CompletionAmount",
                    "MonthlyInstallment", "TotalInstallments", "EndpointAssessorId"))
                {
                    bcp.DestinationTableName = "[PaymentsDue].[Earnings]";
                    await bcp.WriteToServerAsync(reader).ConfigureAwait(false);
                }
            }
        }

        public async Task BulkInsertRequiredPayments(List<ItRequiredPayment> requiredPayments)
        {
            using (var conn = DatabaseConnection.Connection())
            {
                await conn.OpenAsync().ConfigureAwait(false);
                using (var bcp = new SqlBulkCopy(conn))
                using (var reader = ObjectReader.Create(requiredPayments, "Id", "CommitmentId",
                    "CommitmentVersionId", "AccountId", "AccountVersionId", "Uln",
                    "LearnRefNumber", "AimSeqNumber", "Ukprn", "IlrSubmissionDateTime", 
                    "PriceEpisodeIdentifier", "StandardCode", "ProgrammeType", "FrameworkCode",
                    "PathwayCode", "ApprenticeshipContractType", "DeliveryMonth", "DeliveryYear",
                    "CollectionPeriodName", "CollectionPeriodMonth", "CollectionPeriodYear",
                    "TransactionType", "AmountDue", "SfaContributionPercentage", "FundingLineType",
                    "UseLevyBalance", "LearnAimRef", "LearningStartDate"))
                {
                    bcp.DestinationTableName = "[PaymentsDue].[RequiredPayments]";
                    await bcp.WriteToServerAsync(reader).ConfigureAwait(false);
                }
            }
        }

        public async Task BulkInsertSubmissionEvents(List<ItSubmissionEvent> submissionEvents)
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
                    "EmployerReferenceNumber", "EPAOrgId", "GivenNames", "FamilyName"))
                {
                    bcp.DestinationTableName = "[Submissions].[SubmissionEvents]";
                    await bcp.WriteToServerAsync(reader);
                }
            }
        }
    }
}
