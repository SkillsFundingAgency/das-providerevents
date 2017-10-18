using System;

namespace SFA.DAS.Provider.Events.Application.Data.Entities
{
    public class PeriodEntity
    {
        public string Id { get; set; }
        public int CalendarMonth { get; set; }
        public int CalendarYear { get; set; }
        public DateTime? AccountDataValidAt { get; set; }
        public DateTime? CommitmentDataValidAt { get; set; }
        public DateTime CompletionDateTime { get; set; }
    }
}
