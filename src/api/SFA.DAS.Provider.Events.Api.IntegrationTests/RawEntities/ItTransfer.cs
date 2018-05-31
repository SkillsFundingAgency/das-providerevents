using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.Provider.Events.Api.IntegrationTests.RawEntities
{
    class ItTransfer
    {
        public long TransferId {get; set; }

        [Range(10000000, 10000100)]
        public long SendingAccountId { get; set; }

        public long ReceivingAccountId { get; set; }

        public Guid RequiredPaymentId { get; set; }

        public long CommitmentId { get; set; }

        public decimal? Amount { get; set; }

        [Range(1, 15)]
        public int TransferType { get; set; }

        public string CollectionPeriodName { get; set; }

        [Range(1, 12)]
        public int CollectionPeriodMonth { get; set; }

        [Range(2016, 2020)]
        public int CollectionPeriodYear { get; set; }
    }
}
