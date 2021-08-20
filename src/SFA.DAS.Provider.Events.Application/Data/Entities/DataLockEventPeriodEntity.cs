using System;

namespace SFA.DAS.Provider.Events.Application.Data.Entities
{
    public class DataLockEventPeriodEntity
    {
        public Guid DataLockEventId { get; set; }
        public string ApprenticeshipVersion { get; set; }
        public string CollectionPeriodId { get; set; }
        public int CollectionPeriodMonth { get; set; }
        public int CollectionPeriodYear { get; set; }
        public bool IsPayable { get; set; }
        public int TransactionType { get; set; }
    }
}