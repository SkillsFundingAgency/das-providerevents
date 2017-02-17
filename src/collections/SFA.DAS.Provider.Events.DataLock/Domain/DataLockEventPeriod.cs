using SFA.DAS.Payments.DCFS.Domain;

namespace SFA.DAS.Provider.Events.DataLock.Domain
{
    public class DataLockEventPeriod
    {
        public long DataLockEventId { get; set; }
        public CollectionPeriod CollectionPeriod { get; set; }
        public long CommitmentVersion { get; set; }
        public bool IsPayable { get; set; }
        public TransactionType TransactionType { get; set; }
    }
}