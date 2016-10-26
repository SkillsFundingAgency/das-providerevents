namespace SFA.DAS.Payments.Events.Domain.Data.Entities
{
    public class PeriodEntity
    {
        public string Id { get; set; }
        public int CalendarMonth { get; set; }
        public int CalendarYear { get; set; }
    }
}
