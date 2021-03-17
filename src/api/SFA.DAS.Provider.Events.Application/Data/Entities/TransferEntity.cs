using System;

namespace SFA.DAS.Provider.Events.Application.Data.Entities
{
    public class TransferEntity : IAmAPageableEntity
    {
        public long Id {get; set; }
        public long TransferSenderAccountId {get; set; }
        public long AccountId {get; set; }
        public Guid RequiredPaymentEventId {get; set; }
        public decimal Amount {get; set; }
        public string ApprenticeshipId {get; set; }
        public short AcademicYear { get; set; }
        public byte CollectionPeriod { get; set; }
        public int TotalCount { get; }
    }
}
