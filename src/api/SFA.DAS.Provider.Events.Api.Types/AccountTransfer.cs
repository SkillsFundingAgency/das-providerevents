using System;

namespace SFA.DAS.Provider.Events.Api.Types
{
    public class AccountTransfer
    {
        public long TransferId { get;set; }
        public long SenderAccountId { get; set; }
        public long ReceiverAccountId { get; set; }
        public Guid RequiredPaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public long CommitmentId { get; set; }
        public string CollectionPeriodName {get; set; }
    }
}
