using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.Provider.Events.Api.IntegrationTests.RawEntities
{
    class ItRequiredPayment
    {
        public Guid Id { get; set; }
        public long CommitmentId { get; set; }
        [StringLength(25)]
        public string CommitmentVersionId { get; set; }
        [StringLength(50)]
        public string AccountId { get; set; }
        [StringLength(50)]
        public string AccountVersionId { get; set; }
        [Range(1000000000, 1000001000)]
        public long Uln { get; set; }
        [StringLength(12)]
        public string LearnRefNumber { get; set; }
        public int AimSeqNumber { get; set; }
        [Range(10000000, 10000100)]
        public long Ukprn { get; set; }
        public DateTime IlrSubmissionDateTime { get; set; }
        [StringLength(25)]
        public string PriceEpisodeIdentifier { get; set; }
        public long StandardCode { get; set; }
        public int ProgrammeType { get; set; }
        public int FrameworkCode { get; set; }
        public int PathwayCode { get; set; }
        [Range(1, 2)]
        public int ApprenticeshipContractType { get; set; }
        [Range(1, 12)]
        public int DeliveryMonth { get; set; }
        [Range(2017, 2019)]
        public int DeliveryYear { get; set; }
        public string CollectionPeriodName => CalculateCollectionPeriodName();
        [Range(1, 12)]
        public int CollectionPeriodMonth { get; set; }
        [Range(2017, 2019)]
        public int CollectionPeriodYear { get; set; }
        [Range(1, 15)]
        public int TransactionType { get; set; }
        [Range(100, 20000)]
        public decimal AmountDue { get; set; }
        [Range(0, 1)]
        public decimal SfaContributionPercentage { get; set; }
        [StringLength(100)]
        public string FundingLineType { get; set; }
        public bool UseLevyBalance { get; set; }
        [StringLength(8)]
        public string LearnAimRef { get; set; }
        public DateTime LearningStartDate { get; set; }
        public List<ItPayment> Payments { get; set; }

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
