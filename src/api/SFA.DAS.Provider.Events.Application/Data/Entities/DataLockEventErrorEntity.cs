using System;

namespace SFA.DAS.Provider.Events.Application.Data.Entities
{
    public class DataLockEventErrorEntity
    {
        public Guid DataLockEventId { get; set; }
        public string ErrorCode { get; set; }
        public string SystemDescription { get; set; }
    }
}