namespace SFA.DAS.Provider.Events.Domain.Data.Entities
{
    public class DataLockEventPeriodEntity
    {
        public long ApprenticeshipVersion { get; set; }
        public string CollectionPeriodId { get; set; }
        public int CollectionPeriodMonth { get; set; }
        public int CollectionPeriodYear { get; set; }
        public bool Payable { get; set; }
    }
}