using System;

namespace SFA.DAS.Provider.Events.Application.Data.Entities
{
    public class DataLockEventApprenticeshipEntity
    {
        public Guid DataLockEventId { get; set; }
        public string Version { get; set; }
        public DateTime StartDate { get; set; }
        public long? StandardCode { get; set; }
        public int? ProgrammeType { get; set; }
        public int? FrameworkCode { get; set; }
        public int? PathwayCode { get; set; }
        public decimal NegotiatedPrice { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}