using System;

namespace SFA.DAS.Provider.Events.Application.Data.Entities
{
    public class PaymentEntity
    {
        public long Id { get; set; }
        public Guid EventId { get; set; }
        public long Ukprn { get; set; }
        public long LearnerUln { get; set; }
        public string AccountId { get; set; }
        public long? ApprenticeshipId { get; set; }
        
        public byte DeliveryPeriod { get; set; }
        public short AcademicYear { get; set; }
        public byte CollectionPeriod { get; set; }
        
        public DateTime IlrSubmissionDateTime { get; set; }
        public int FundingSource { get; set; }
        public int TransactionType { get; set; }
        
        public decimal Amount { get; set; }

        public Guid RequiredPaymentEventId { get; set; }

        public long? LearningAimStandardCode { get; set; }
        public int? LearningAimFrameworkCode { get; set; }
        public int? LearningAimProgrammeType { get; set; }
        public int? LearningAimPathwayCode { get; set; }

        public int ContractType { get; set; }

        public DateTime EarningsStartDate { get; set; }
        public DateTime? EarningsPlannedEndDate { get; set; }
        public DateTime? EarningsActualEndDate { get; set; }
        public byte? EarningsCompletionStatus { get; set; }
        public decimal? EarningsCompletionAmount { get; set; }
        public decimal? EarningsInstalmentAmount { get; set; }
        public short? EarningsNumberOfInstalments { get; set; }
    }
}
