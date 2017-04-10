using System;

namespace SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities
{
    public class DataLockEventPeriodEntity
    {
        public Guid DataLockEventId { get; set; }
        public string CollectionPeriodName { get; set; }
        public int CollectionPeriodMonth { get; set; }
        public int CollectionPeriodYear { get; set; }
        public long CommitmentVersion { get; set; }
        public bool IsPayable { get; set; }
        public int TransactionType { get; set; }
    }
}