using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Dapper.Contrib.Extensions;

namespace SFA.DAS.Provider.Events.Api.IntegrationTestsV2.RawEntities
{
    //[Table("Payments.Payments")]
    //class ItPayment
    //{
    //    [Dapper.Contrib.Extensions.Key]
    //    public Guid PaymentId { get; set; }

    //    public Guid RequiredPaymentId { get; set; }

    //    [Range(1, 12)]
    //    public int DeliveryMonth { get; set; }
    //    [Range(2017, 2019)]
    //    public int DeliveryYear { get; set; }

    //    [StringLength(8)]
    //    public string CollectionPeriodName => CalculateCollectionPeriodName();
    //    [Range(1, 12)]
    //    public int CollectionPeriodMonth { get; set; }
    //    [Range(2017, 2019)]
    //    public int CollectionPeriodYear { get; set; }
    //    [Range(1, 4)]
    //    public int FundingSource { get; set; }
    //    [Range(1, 15)]
    //    public int TransactionType { get; set; }
    //    [Range(100, 10000)]
    //    public decimal Amount { get; set; }

    //    public List<ItEarning> Earnings { get; set; }

    //    string CalculateCollectionPeriodName()
    //    {
    //        var useStartYear = CollectionPeriodMonth >= 8;
    //        int year;
    //        if (useStartYear)
    //        {
    //            year = CollectionPeriodYear - 2000;
    //        }
    //        else
    //        {
    //            year = CollectionPeriodYear - 1 - 2000;
    //        }
    //        var collectionPeriodName = $"{year}{year + 1}-R{CollectionPeriodMonth:D2}";
    //        return collectionPeriodName;
    //    }
    //}

    [Table("Payments2.Payment")]
    public class ItPayment
    {
        [Dapper.Contrib.Extensions.Key]
        public long Id { get; set; }
        public Guid EventId { get; set; }
        public Guid EarningEventId { get; set; }
        public Guid FundingSourceEventId { get; set; }
        public Guid? RequiredPaymentEventId { get; set; }
        public DateTimeOffset EventTime { get; set; }
        public long Ukprn { get; set; }

        [StringLength(50)]
        public string LearnerReferenceNumber { get; set; }
        public long LearnerUln { get; set; }

        [StringLength(50)]
        public string PriceEpisodeIdentifier { get; set; }
        public decimal Amount { get; set; }

        [Range(1, 14)]
        public byte CollectionPeriod { get; set; }

        [Range(1111, 9999)]
        public short AcademicYear { get; set; }

        [Range(1, 12)]
        public byte DeliveryPeriod { get; set; }

        [StringLength(8)]
        public string LearningAimReference { get; set; }
        public int LearningAimProgrammeType { get; set; }
        public int LearningAimStandardCode { get; set; }
        public int LearningAimFrameworkCode { get; set; }
        public int LearningAimPathwayCode { get; set; }

        [StringLength(100)]
        public string LearningAimFundingLineType { get; set; }
        [Range(1, 2)]
        public byte ContractType { get; set; }
        [Range(1, 15)]
        public byte TransactionType { get; set; }
        [Range(1, 5)]
        public byte FundingSource { get; set; }
        public DateTime IlrSubmissionDateTime { get; set; }

        [Range(0, 1)]
        public decimal SfaContributionPercentage { get; set; }

        [StringLength(255)]
        public string AgreementId { get; set; }
        public long JobId { get; set; }
        public long? AccountId { get; set; }
        public long? TransferSenderAccountId { get; set; }
        public DateTime EarningsStartDate { get; set; }
        public DateTime? EarningsPlannedEndDate { get; set; }
        public DateTime? EarningsActualEndDate { get; set; }
        public byte? EarningsCompletionStatus { get; set; }
        public decimal? EarningsCompletionAmount { get; set; }
        public decimal? EarningsInstalmentAmount { get; set; }
        public short? EarningsNumberOfInstalments { get; set; }
        public DateTime? LearningStartDate { get; set; }
        public long? ApprenticeshipId { get; set; }
        public long? ApprenticeshipPriceEpisodeId { get; set; }

        [Range(1, 2)]
        public byte ApprenticeshipEmployerType { get; set; }

        [StringLength(120)]
        public string ReportingAimFundingLineType { get; set; }
        public byte NonPaymentReason { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public int? DuplicateNumber { get; set; }
    }

    //[DebuggerDisplay("Collection: {AcademicYear}-R{Period}")]
    //public class CollectionPeriod
    //{
    //    public short AcademicYear { get; set; }
    //    public byte Period { get; set; }
    //    //public CollectionPeriod Clone()
    //    //{
    //    //    return (CollectionPeriod)MemberwiseClone();
    //    //}
    //}
    public enum ContractType : byte
    {
        None = byte.MaxValue,
        Act1 = 1,
        Act2 = 2,
    }

    public enum TransactionType : byte
    {
        Learning = 1,
        Completion = 2,
        Balancing = 3,
        First16To18EmployerIncentive = 4,
        First16To18ProviderIncentive = 5,
        Second16To18EmployerIncentive = 6,
        Second16To18ProviderIncentive = 7,
        OnProgramme16To18FrameworkUplift = 8,
        Completion16To18FrameworkUplift = 9,
        Balancing16To18FrameworkUplift = 10,
        FirstDisadvantagePayment = 11,
        SecondDisadvantagePayment = 12,
        OnProgrammeMathsAndEnglish = 13,
        BalancingMathsAndEnglish = 14,
        LearningSupport = 15,
        CareLeaverApprenticePayment = 16,
    }

    public enum FundingSourceType : byte
    {
        Levy = 1,
        CoInvestedSfa = 2,
        CoInvestedEmployer = 3,
        FullyFundedSfa = 4,
        Transfer = 5,
    }

    public enum ApprenticeshipEmployerType : byte
    {
        NonLevy = 0,
        Levy = 1
    }
}
