using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;

namespace SFA.DAS.Provider.Events.Api.IntegrationTests.RawEntities
{
    [Table("Payments.Payments")]
    class ItPayment
    {
        [Dapper.Contrib.Extensions.Key]
        public Guid PaymentId { get; set; }

        public Guid RequiredPaymentId { get; set; }

        [Range(1, 12)]
        public int DeliveryMonth { get; set; }
        [Range(2017, 2019)]
        public int DeliveryYear { get; set; }

        [StringLength(8)]
        public string CollectionPeriodName => CalculateCollectionPeriodName();
        [Range(1, 12)]
        public int CollectionPeriodMonth { get; set; }
        [Range(2017, 2019)]
        public int CollectionPeriodYear { get; set; }
        [Range(1, 4)]
        public int FundingSource { get; set; }
        [Range(1, 15)]
        public int TransactionType { get; set; }
        [Range(100, 10000)]
        public decimal Amount { get; set; }

        public List<ItEarning> Earnings { get; set; }

        string CalculateCollectionPeriodName()
        {
            var useStartYear = CollectionPeriodMonth >= 8;
            int year;
            if (useStartYear)
            {
                year = CollectionPeriodYear - 2000;
            }
            else
            {
                year = CollectionPeriodYear - 1 - 2000;
            }
            var collectionPeriodName = $"{year}{year + 1}-R{CollectionPeriodMonth:D2}";
            return collectionPeriodName;
        }
    }
}
