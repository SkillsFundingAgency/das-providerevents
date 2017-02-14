using System;
using NLog.LayoutRenderers;

namespace SFA.DAS.Provider.Events.DataLock.Domain
{
    public class DataLockEvent
    {
        public long Id { get; set; }
        public DateTime ProcessDateTime { get; set; }

        public string IlrFileName { get; set; }
        public DateTime SubmittedDateTime { get; set; }
        public string AcademicYear { get; set; }
        public long Ukprn { get; set; }
        public long Uln { get; set; }
        public string LearnRefnumber { get; set; }
        public long AimSeqNumber { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public long CommitmentId { get; set; }
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

        public DataLockEventPeriod[] Periods { get; set; }
        public DataLockEventError[] Errors { get; set; }
        public DataLockEventCommitmentVersion[] CommitmentVersions { get; set; }
    }
}