using SFA.DAS.Payments.DCFS.Infrastructure.Data;
using SFA.DAS.Provider.Events.Submission.Domain;
using SFA.DAS.Provider.Events.Submission.Domain.Data;

namespace SFA.DAS.Provider.Events.Submission.Infrastructure.Data
{
    public class DcfsSubmissionEventRepository : DcfsRepository, ISubmissionEventRepository
    {
        public DcfsSubmissionEventRepository(string connectionString)
            : base(connectionString)
        {
        }

        public void StoreSubmissionEvent(SubmissionEvent @event)
        {
          

            Execute("INSERT INTO Submissions.SubmissionEvents " +
                    "(IlrFileName,FileDateTime,SubmittedDateTime,ComponentVersionNumber,UKPRN,ULN,LearnRefNumber,AimSeqNumber," +
                    "PriceEpisodeIdentifier,StandardCode,ProgrammeType,FrameworkCode,PathwayCode,ActualStartDate,PlannedEndDate," +
                    "ActualEndDate,OnProgrammeTotalPrice,CompletionTotalPrice,NINumber,CommitmentId,AcademicYear,EmployerReferenceNumber) " +
                    "VALUES " +
                    "(@IlrFileName,@FileDateTime,@SubmittedDateTime,@ComponentVersionNumber,@UKPRN,@ULN,@LearnRefNumber,@AimSeqNumber,@" +
                    "PriceEpisodeIdentifier,@StandardCode,@ProgrammeType,@FrameworkCode,@PathwayCode,@ActualStartDate,@PlannedEndDate,@" +
                    "ActualEndDate,@OnProgrammeTotalPrice,@CompletionTotalPrice,@NINumber,@CommitmentId,@AcademicYear,@EmployerReferenceNumber)",
                    @event);
        }
    }
}
