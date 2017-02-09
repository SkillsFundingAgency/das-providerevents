namespace SFA.DAS.Provider.Events.Api.Types
{
    public class DataLockEventPeriod
    {
        public long ApprenticeshipVersion { get; set; }
        public NamedCalendarPeriod Period { get; set; }
        public bool Payable { get; set; }
    }
}