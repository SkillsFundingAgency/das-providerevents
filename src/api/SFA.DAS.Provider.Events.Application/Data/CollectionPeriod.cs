using System;

namespace SFA.DAS.Provider.Events.Application.Data
{
    public class CollectionPeriod
    {
        public string Id { get; set; }
        public string PeriodName { get; set; }
        public short AcademicYear { get; set; }
        public byte Period { get; set; }
        public int CalendarMonth { get; set; }
        public int CalendarYear { get; set; }
        public DateTime? AccountDataValidAt { get; set; }
        public DateTime? CommitmentDataValidAt { get; set; }
        public DateTime CompletionDateTime { get; set; }
    }
}
