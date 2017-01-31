namespace SFA.DAS.Provider.Events.Submission.Domain.Data
{
    public interface ISubmissionEventRepository
    {
        void StoreSubmissionEvent(SubmissionEvent @event);
    }
}