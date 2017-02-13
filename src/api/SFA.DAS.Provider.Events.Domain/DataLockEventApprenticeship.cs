using System;

namespace SFA.DAS.Provider.Events.Domain
{
    public class DataLockEventApprenticeship
    {
        public long Version { get; set; }
        public DateTime StartDate { get; set; }
        public long? StandardCode { get; set; }
        public int? ProgrammeType { get; set; }
        public int? FrameworkCode { get; set; }
        public int? PathwayCode { get; set; }
        public decimal NegotiatedPrice { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}