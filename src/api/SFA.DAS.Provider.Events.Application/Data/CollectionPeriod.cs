using System;

namespace SFA.DAS.Provider.Events.Application.Data
{
    //TODO This can be updated to represent the data on the new CollectionPeriod table in payments v2, although it is a DTO for passing between our layers not a DB entity
    public class CollectionPeriod
    {
        public string Id { get; set; }
        public int CalendarMonth { get; set; }
        public int CalendarYear { get; set; }
        public string PeriodName { get; set; }
        public DateTime? AccountDataValidAt { get; set; }
        public DateTime? CommitmentDataValidAt { get; set; }
        public DateTime CompletionDateTime { get; set; }
        public short AcademicYear { get; set; }
        public byte Period { get; set; }
    }
}
