using System;

namespace SFA.DAS.Provider.Events.DataLock.Domain
{
    public class DataLockEventError
    {
        public Guid DataLockEventId { get; set; }
        public string ErrorCode { get; set; }
        public string SystemDescription { get; set; }
    }
}