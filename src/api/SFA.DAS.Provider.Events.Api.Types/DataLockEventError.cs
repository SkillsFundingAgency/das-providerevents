namespace SFA.DAS.Provider.Events.Api.Types
{
    public class DataLockEventError
    {
        public long DataLockEventId { get; set; }
        public string ErrorCode { get; set; }
        public string SystemDescription { get; set; }
    }
}