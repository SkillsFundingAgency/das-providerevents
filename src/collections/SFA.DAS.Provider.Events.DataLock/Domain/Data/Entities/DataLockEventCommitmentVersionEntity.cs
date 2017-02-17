using System;

namespace SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities
{
    public class DataLockEventCommitmentVersionEntity
    {
        public long DataLockEventId { get; set; }
        public long CommitmentVersion { get; set; }
        public DateTime CommitmentStartDate { get; set; }
        public long? CommitmentStandardCode { get; set; }
        public int? CommitmentProgrammeType { get; set; }
        public int? CommitmentFrameworkCode { get; set; }
        public int? CommitmentPathwayCode { get; set; }
        public decimal CommitmentNegotiatedPrice { get; set; }
        public DateTime CommitmentEffectiveDate { get; set; }
    }
}