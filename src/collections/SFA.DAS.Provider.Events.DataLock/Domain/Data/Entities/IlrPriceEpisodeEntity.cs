using System;

namespace SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities
{
    public class IlrPriceEpisodeEntity
    {
        public string IlrFileName { get; set; }
        public DateTime SubmittedTime { get; set; }
        public long Ukprn { get; set; }
        public long Uln { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public string LearnRefnumber { get; set; }
        public long AimSeqNumber { get; set; }
        public DateTime? IlrStartDate { get; set; }
        public long? IlrStandardCode { get; set; }
        public int? IlrProgrammeType { get; set; }
        public int? IlrFrameworkCode { get; set; }
        public int? IlrPathwayCode { get; set; }
        public decimal? IlrTrainingPrice { get; set; }
        public decimal? IlrEndpointAssessorPrice { get; set; }
    }
}