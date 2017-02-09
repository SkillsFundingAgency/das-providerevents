using System;

namespace SFA.DAS.Provider.Events.Api.Types
{
    public class DataLockEvent
    {
        public long Id { get; set; }
        public DateTime ProcessDateTime { get; set; }
        public string IlrFileName { get; set; }
        public long Ukprn { get; set; }
        public long Uln { get; set; }
        public string LearnRefNumber { get; set; }
        public long AimSeqNumber { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public long CommitmentId { get; set; }
        public long CommitmentVersion { get; set; }
        public long EmployerAccountId { get; set; }
        public EventSource EventSource { get; set; }
        public bool HasErrors { get; set; }
        public DateTime? IlrStartDate { get; set; }
        public long? IlrStandardCode { get; set; }
        public int? IlrProgrammeType { get; set; }
        public int? IlrFrameworkCode { get; set; }
        public int? IlrPathwayCode { get; set; }
        public decimal? IlrTrainingPrice { get; set; }
        public decimal? IlrEndpointAssessorPrice { get; set; }
        public DateTime? CommitmentStartDate { get; set; }
        public long? CommitmentStandardCode { get; set; }
        public int? CommitmentProgrammeType { get; set; }
        public int? CommitmentFrameworkCode { get; set; }
        public int? CommitmentPathwayCode { get; set; }
        public decimal? CommitmentNegotiatedPrice { get; set; }
        public DateTime? CommitmentEffectiveDate { get; set; }

        public DataLockEventError[] Errors { get; set; }
    }
}