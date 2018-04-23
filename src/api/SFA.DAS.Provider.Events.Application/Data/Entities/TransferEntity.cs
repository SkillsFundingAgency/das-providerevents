using System;

namespace SFA.DAS.Provider.Events.Application.Data.Entities
{
    public class TransferEntity : IAmAPageableEntity
    {
        public Guid Id {get; set; }
        public long SendingAccountId {get; set; }
        public long RecievingAccountId {get; set; }
        public Guid RequiredPaymentId {get; set; }
        public decimal Amount {get; set; }
        public int TransferType {get; set; }
        public DateTime TransferDate {get; set; }

        public string CommitmentId {get; set; }
        public string CollectionPeriodName {get; set; }
        public int TotalCount { get; }
    }
}
