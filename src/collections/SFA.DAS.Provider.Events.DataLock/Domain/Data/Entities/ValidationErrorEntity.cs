namespace SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities
{
    public class ValidationErrorEntity
    {
        public long Ukprn { get; set; }
        public string LearnRefnumber { get; set; }
        public long AimSeqNumber { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public string RuleId { get; set; }
    }
}