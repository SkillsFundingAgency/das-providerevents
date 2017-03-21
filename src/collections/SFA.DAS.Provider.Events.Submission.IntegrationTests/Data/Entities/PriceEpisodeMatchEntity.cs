namespace SFA.DAS.Provider.Events.Submission.IntegrationTests.Data.Entities
{
    public class PriceEpisodeMatchEntity
    {
        public long Ukprn { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public string LearnRefNumber { get; set; }
        public int AimSeqNumber { get; set; }
        public long CommitmentId { get; set; }
        public bool IsSuccess { get; set; } 
    }
}