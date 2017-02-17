using SFA.DAS.Payments.DCFS.Infrastructure.Data;
using SFA.DAS.Provider.Events.Submission.Domain;
using SFA.DAS.Provider.Events.Submission.Domain.Data;

namespace SFA.DAS.Provider.Events.Submission.Infrastructure.Data
{
    public class DcfsIlrSubmissionRepository : DcfsRepository, IIlrSubmissionRepository
    {
        public DcfsIlrSubmissionRepository(string connectionString)
            : base(connectionString)
        {
        }

        public IlrDetails[] GetCurrentVersions()
        {
            return Query<IlrDetails>("SELECT * FROM Submissions.CurrentVersion");
        }

        public IlrDetails[] GetLastSeenVersions()
        {
            return Query<IlrDetails>("SELECT * FROM Submissions.LastSeenVersion");
        }

        public void StoreLastSeenVersion(IlrDetails details)
        {
            Execute("DELETE FROM Submissions.LastSeenVersion " +
                    "WHERE UKPRN = @UKPRN " +
                    "AND ULN = @ULN " +
                    "AND PriceEpisodeIdentifier = @PriceEpisodeIdentifier",
                    details);

            Execute("INSERT INTO Submissions.LastSeenVersion " +
                    "(IlrFileName,FileDateTime,SubmittedDateTime,ComponentVersionNumber,UKPRN,ULN,LearnRefNumber,AimSeqNumber," +
                    "PriceEpisodeIdentifier,StandardCode,ProgrammeType,FrameworkCode,PathwayCode,ActualStartDate,PlannedEndDate," +
                    "ActualEndDate,OnProgrammeTotalPrice,CompletionTotalPrice,NINumber,CommitmentId,AcademicYear,EmployerReferenceNumber) " +
                    "VALUES" +
                    "(@IlrFileName,@FileDateTime,@SubmittedDateTime,@ComponentVersionNumber,@UKPRN,@ULN,@LearnRefNumber,@AimSeqNumber,@" +
                    "PriceEpisodeIdentifier,@StandardCode,@ProgrammeType,@FrameworkCode,@PathwayCode,@ActualStartDate,@PlannedEndDate,@" +
                    "ActualEndDate,@OnProgrammeTotalPrice,@CompletionTotalPrice,@NINumber,@CommitmentId,@AcademicYear,@EmployerReferenceNumber)", 
                    details);
        }
    }
}
