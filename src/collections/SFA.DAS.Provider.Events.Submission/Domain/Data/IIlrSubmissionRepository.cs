namespace SFA.DAS.Provider.Events.Submission.Domain.Data
{
    public interface IIlrSubmissionRepository
    {

        IlrDetails[] GetCurrentVersions();

        IlrDetails[] GetLastSeenVersions();

        void StoreLastSeenVersions(IlrDetails[] details);
    }
}
