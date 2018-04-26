using System;

namespace SFA.DAS.Provider.Events.Application.Data.Entities
{
    public class TransferEntity : IAmAPageableEntity
    {
        public long SendingAccountId {get; set; }
        public long ReceivingAccountId {get; set; }
        public Guid RequiredPaymentId {get; set; }
        public decimal Amount {get; set; }
        public int TransferType {get; set; }
        public string CommitmentId {get; set; }
        public string CollectionPeriodName {get; set; }
        public int TotalCount { get; }
    }
}
