namespace SFA.DAS.Provider.Events.Domain
{
    public class DataLockEventError
    {
        public long DataLockEventId { get; set; }
        public string ErrorCode { get; set; }
        public string SystemDescription { get; set; }
    }
}