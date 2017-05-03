namespace SFA.DAS.Provider.Events.Submission.Domain.Data
{
    public interface ISubmissionEventRepository
    {
        void StoreSubmissionEvents(SubmissionEvent[] events);
    }
}