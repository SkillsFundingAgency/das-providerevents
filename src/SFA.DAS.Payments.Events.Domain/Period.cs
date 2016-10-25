namespace SFA.DAS.Payments.Events.Domain
{
    public class Period
    {
        public string Id { get; set; }
        public int CalendarMonth { get; set; }
        public int CalendarYear { get; set; }
    }
}
