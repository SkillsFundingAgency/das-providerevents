using System.Data.SqlClient;
using System.Linq;
using System.Text;
using SFA.DAS.Payments.DCFS.Infrastructure.Data;
using SFA.DAS.Provider.Events.Submission.Domain;
using SFA.DAS.Provider.Events.Submission.Domain.Data;

namespace SFA.DAS.Provider.Events.Submission.Infrastructure.Data
{
    public class DcfsSubmissionEventRepository : DcfsRepository, ISubmissionEventRepository
    {
        private readonly string _connectionString;

        public DcfsSubmissionEventRepository(string connectionString)
            : base(connectionString)
        {
            _connectionString = connectionString;
        }

        public void StoreSubmissionEvents(SubmissionEvent[] events)
        {
            const int batchSize = 100;
            var skip = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                while (skip < events.Length)
                {
                    var batch = events.Skip(skip).Take(batchSize)
                        .Select(EventToInsertBlock)
                        .Aggregate((x, y) => $"{x}, {y}");
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO Submissions.SubmissionEvents " +
                                              "(IlrFileName,FileDateTime,SubmittedDateTime,ComponentVersionNumber,UKPRN,ULN,LearnRefNumber,AimSeqNumber," +
                                              "PriceEpisodeIdentifier,StandardCode,ProgrammeType,FrameworkCode,PathwayCode,ActualStartDate,PlannedEndDate," +
                                              "ActualEndDate,OnProgrammeTotalPrice,CompletionTotalPrice,NINumber,CommitmentId,AcademicYear,EmployerReferenceNumber) " +
                                              $"VALUES {batch}";
                        command.ExecuteNonQuery();
                    }

                    skip += batchSize;
                }
            }
        }

        private string EventToInsertBlock(SubmissionEvent @event)
        {
            var block = new StringBuilder();
            block.Append("(");

            block.Append($"'{@event.IlrFileName}',");
            block.Append($"'{@event.FileDateTime:yyyy-MM-dd HH:mm:ss}',");
            block.Append($"'{@event.SubmittedDateTime:yyyy-MM-dd HH:mm:ss}',");
            block.Append($"'{@event.ComponentVersionNumber}',");
            block.Append($"{@event.Ukprn},");
            block.Append($"{@event.Uln},");
            block.Append($"'{@event.LearnRefNumber}',");
            block.Append($"{@event.AimSeqNumber},");
            block.Append($"'{@event.PriceEpisodeIdentifier}',");
            block.Append($"{(@event.StandardCode.HasValue ? @event.StandardCode.Value.ToString() : "NULL")},");
            block.Append($"{(@event.ProgrammeType.HasValue ? @event.ProgrammeType.Value.ToString() : "NULL")},");
            block.Append($"{(@event.FrameworkCode.HasValue ? @event.FrameworkCode.Value.ToString() : "NULL")},");
            block.Append($"{(@event.PathwayCode.HasValue ? @event.PathwayCode.Value.ToString() : "NULL")},");
            block.Append($"{(@event.ActualStartDate.HasValue ? "'" + @event.ActualStartDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'" : "NULL")},");
            block.Append($"{(@event.PlannedEndDate.HasValue ? "'" + @event.PlannedEndDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'" : "NULL")},");
            block.Append($"{(@event.ActualEndDate.HasValue ? "'" + @event.ActualEndDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'" : "NULL")},");
            block.Append($"{(@event.OnProgrammeTotalPrice.HasValue ? @event.OnProgrammeTotalPrice.Value.ToString() : "NULL")},");
            block.Append($"{(@event.CompletionTotalPrice.HasValue ? @event.CompletionTotalPrice.Value.ToString() : "NULL")},");
            block.Append($"{(@event.NiNumber != null ? "'" + @event.NiNumber + "'" : "NULL")},");
            block.Append($"{(@event.CommitmentId != null ? @event.CommitmentId.ToString() : "NULL")},");
            block.Append($"{(@event.AcademicYear != null ? "'" + @event.AcademicYear + "'" : "NULL")},");
            block.Append($"{@event.EmployerReferenceNumber}");

            block.Append(")");
            return block.ToString();
        }
    }
}
