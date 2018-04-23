using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.Provider.Events.Api.IntegrationTests.RawEntities
{
    class ItTransfer
    {
        public Guid Id { get; set; }

        [Range(10000000, 10000100)]
        public long SendingAccountId { get; set; }

        public long RecievingAccountId { get; set; }

        public Guid RequiredPaymentId { get; set; }

        public long CommitmentId { get; set; }

        public decimal? Amount { get; set; }

        [Range(1, 15)]
        public int TransferType { get; set; }

        public DateTime TransferDate { get; set; }

        public string CollectionPeriodName { get; set; }
    }
}
