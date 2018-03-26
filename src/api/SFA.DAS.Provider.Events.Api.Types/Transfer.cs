using System;

namespace SFA.DAS.Provider.Events.Api.Types
{
    public class Transfer
    {
        public long SenderAccountId { get; set; }
        public long ReceiverAccountId { get; set; }
        public long CommitmentId { get; set; }
        public decimal Amount { get; set; }
        public TransferType Type { get; set; }
        public DateTime TransferDate { get; set; }
    }
}
