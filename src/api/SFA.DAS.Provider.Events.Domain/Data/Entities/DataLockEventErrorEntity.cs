namespace SFA.DAS.Provider.Events.Domain.Data.Entities
{
    public class DataLockEventErrorEntity
    {
        public long DataLockEventId { get; set; }
        public string ErrorCode { get; set; }
        public string SystemDescription { get; set; }
    }
}