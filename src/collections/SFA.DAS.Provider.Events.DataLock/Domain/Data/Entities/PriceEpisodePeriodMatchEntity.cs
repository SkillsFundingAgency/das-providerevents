namespace SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities
{
    public class PriceEpisodePeriodMatchEntity
    {
        public long Ukprn { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public string LearnRefnumber { get; set; }
        public long AimSeqNumber { get; set; }
        public long CommitmentId { get; set; }
        public long VersionId { get; set; }
        public int Period { get; set; }
        public bool Payable { get; set; }
        public int TransactionType { get; set; }
    }
}