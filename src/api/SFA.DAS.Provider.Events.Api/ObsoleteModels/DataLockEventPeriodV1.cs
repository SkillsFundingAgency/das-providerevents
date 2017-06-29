
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.Api.ObsoleteModels
{
    public class DataLockEventPeriodV1
    {
        public string ApprenticeshipVersion { get; set; }
        public NamedCalendarPeriod Period { get; set; }
        public bool IsPayable { get; set; }
        public TransactionType TransactionType { get; set; }
    }
}