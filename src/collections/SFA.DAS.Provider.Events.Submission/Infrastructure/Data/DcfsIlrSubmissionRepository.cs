using System.Data.SqlClient;
using System.Linq;
using System.Text;
using SFA.DAS.Payments.DCFS.Infrastructure.Data;
using SFA.DAS.Provider.Events.Submission.Domain;
using SFA.DAS.Provider.Events.Submission.Domain.Data;

namespace SFA.DAS.Provider.Events.Submission.Infrastructure.Data
{
    public class DcfsIlrSubmissionRepository : DcfsRepository, IIlrSubmissionRepository
    {
        private readonly string _connectionString;

        public DcfsIlrSubmissionRepository(string connectionString)
            : base(connectionString)
        {
            _connectionString = connectionString;
        }

        public IlrDetails[] GetCurrentVersions()
        {
            return Query<IlrDetails>("SELECT * FROM Submissions.CurrentVersion");
        }

        public IlrDetails[] GetLastSeenVersions()
        {
            return Query<IlrDetails>("SELECT * FROM Submissions.LastSeenVersion");
        }

        public void StoreLastSeenVersions(IlrDetails[] details)
        {
            const int batchSize = 100;
            var skip = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                while (skip < details.Length)
                {
                    var batch = details.Skip(skip).Take(batchSize).ToArray();
                    var deleteBatch = batch.Select(IlrToDeleteStatement)
                                           .Aggregate((x, y) => $"{x} OR {y}");
                    var insertBatch = batch.Select(IlrToInsertBlock)
                                           .Aggregate((x, y) => $"{x}, {y}");
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = $"DELETE FROM Submissions.LastSeenVersion WHERE {deleteBatch}";
                        command.ExecuteNonQuery();

                        command.CommandText = "INSERT INTO Submissions.LastSeenVersion " +
                                              "(IlrFileName,FileDateTime,SubmittedDateTime,ComponentVersionNumber,UKPRN,ULN,LearnRefNumber,AimSeqNumber," +
                                              "PriceEpisodeIdentifier,StandardCode,ProgrammeType,FrameworkCode,PathwayCode,ActualStartDate,PlannedEndDate," +
                                              "ActualEndDate,OnProgrammeTotalPrice,CompletionTotalPrice,NINumber,CommitmentId,AcademicYear,EmployerReferenceNumber) " +
                                              $"VALUES {insertBatch}";
                        command.ExecuteNonQuery();
                    }

                    skip += batchSize;
                }
            }
        }


        private string IlrToDeleteStatement(IlrDetails ilr)
        {
            return $"(UKPRN = {ilr.Ukprn} AND LearnRefNumber = '{ilr.LearnRefNumber}' AND PriceEpisodeIdentifier = '{ilr.PriceEpisodeIdentifier}')";
        }
        private string IlrToInsertBlock(IlrDetails ilr)
        {
            var block = new StringBuilder();
            block.Append("(");

            block.Append($"'{ilr.IlrFileName}',");
            block.Append($"'{ilr.FileDateTime:yyyy-MM-dd HH:mm:ss}',");
            block.Append($"'{ilr.SubmittedDateTime:yyyy-MM-dd HH:mm:ss}',");
            block.Append($"'{ilr.ComponentVersionNumber}',");
            block.Append($"{ilr.Ukprn},");
            block.Append($"{ilr.Uln},");
            block.Append($"'{ilr.LearnRefNumber}',");
            block.Append($"{ilr.AimSeqNumber},");
            block.Append($"'{ilr.PriceEpisodeIdentifier}',");
            block.Append($"{(ilr.StandardCode.HasValue ? ilr.StandardCode.Value.ToString() : "NULL")},");
            block.Append($"{(ilr.ProgrammeType.HasValue ? ilr.ProgrammeType.Value.ToString() : "NULL")},");
            block.Append($"{(ilr.FrameworkCode.HasValue ? ilr.FrameworkCode.Value.ToString() : "NULL")},");
            block.Append($"{(ilr.PathwayCode.HasValue ? ilr.PathwayCode.Value.ToString() : "NULL")},");
            block.Append($"'{ilr.ActualStartDate:yyyy-MM-dd HH:mm:ss}',");
            block.Append($"'{ilr.PlannedEndDate:yyyy-MM-dd HH:mm:ss}',");
            block.Append($"{(ilr.ActualEndDate.HasValue ? "'" + ilr.ActualEndDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'" : "NULL")},");
            block.Append($"'{ilr.OnProgrammeTotalPrice}',");
            block.Append($"'{ilr.CompletionTotalPrice}',");
            block.Append($"{(ilr.NiNumber != null ? "'" + ilr.NiNumber + "'" : "NULL")},");
            block.Append($"{(ilr.CommitmentId != null ? ilr.CommitmentId.ToString() : "NULL")},");
            block.Append($"{(ilr.AcademicYear != null ? "'" + ilr.AcademicYear + "'" : "NULL")},");
            block.Append($"{ilr.EmployerReferenceNumber}");

            block.Append(")");
            return block.ToString();
        }
    }
}
