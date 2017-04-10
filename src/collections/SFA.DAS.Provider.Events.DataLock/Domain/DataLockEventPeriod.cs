using SFA.DAS.Payments.DCFS.Domain;
using System;

namespace SFA.DAS.Provider.Events.DataLock.Domain
{
    public class DataLockEventPeriod
    {
        public Guid DataLockEventId { get; set; }
        public CollectionPeriod CollectionPeriod { get; set; }
        public long CommitmentVersion { get; set; }
        public bool IsPayable { get; set; }
        public TransactionType TransactionType { get; set; }
    }
}