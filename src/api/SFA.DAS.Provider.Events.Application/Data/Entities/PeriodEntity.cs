using System;

namespace SFA.DAS.Provider.Events.Application.Data.Entities
{
    public class PeriodEntity
    {
        public long Id { get; set; }
        public short AcademicYear { get; set; }
        public byte Period { get; set; }
        public DateTime? ReferenceDataValidationDate { get; set; }
        public DateTime CompletionDate { get; set; }
    }
}
