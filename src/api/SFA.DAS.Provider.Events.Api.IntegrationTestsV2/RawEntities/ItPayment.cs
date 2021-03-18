using System;
using System.ComponentModel.DataAnnotations;
using Dapper.Contrib.Extensions;

namespace SFA.DAS.Provider.Events.Api.IntegrationTestsV2.RawEntities
{
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

        [Range(2016, 2021)]
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
        [Range(1, 16)]
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
}
