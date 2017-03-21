using System;

namespace SFA.DAS.Provider.Events.Submission.IntegrationTests.Data.Entities
{
    public class PriceEpisodeEntity
    {
        public string PriceEpisodeIdentifier { get; set; }
        public long Ukprn { get; set; }
        public string LearnRefNumber { get; set; }
        public int PriceEpisodeAimSeqNumber { get; set; }
        public DateTime EpisodeEffectiveTnpStartDate { get; set; }
        public decimal Tnp1 { get; set; }
        public decimal Tnp2 { get; set; }
        public decimal Tnp3 { get; set; }
        public decimal Tnp4 { get; set; }
    }
}
