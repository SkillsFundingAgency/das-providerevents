using System;

namespace SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities
{
    public class CommitmentEntity
    {
        public long CommitmentId { get; set; }
        public long CommitmentVersion { get; set; }
        public long EmployerAccountId { get; set; }
        public DateTime StartDate { get; set; }
        public long? StandardCode { get; set; }
        public int? ProgrammeType { get; set; }
        public int? FrameworkCode { get; set; }
        public int? PathwayCode { get; set; }
        public decimal NegotiatedPrice { get; set; }
        public DateTime EffectiveDate { get; set; } 
    }
}