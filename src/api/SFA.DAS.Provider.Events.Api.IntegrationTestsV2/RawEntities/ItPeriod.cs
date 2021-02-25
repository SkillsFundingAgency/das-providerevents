using System;

namespace SFA.DAS.Provider.Events.Api.IntegrationTestsV2.RawEntities
{
    public class ItPeriod
    {
        public long Id { get; set; }
        public short AcademicYear { get; set; }
        public byte Period { get; set; }
        public DateTime? ReferenceDataValidationDate { get; set; }
        public DateTime CompletionDate { get; set; }
    }
}
