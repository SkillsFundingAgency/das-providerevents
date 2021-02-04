using System;
using System.Collections.Generic;

namespace SFA.DAS.Provider.Events.Application.Data.Entities
{
    public class PaymentEntity : IAmAPageableEntity
    {
        public string Id { get; set; }
        public Guid RequiredPaymentId { get; set; }
        public long Ukprn { get; set; }
        public long Uln { get; set; }
        public string EmployerAccountId { get; set; }
        public long? ApprenticeshipId { get; set; }

        public int DeliveryPeriodMonth { get; set; }
        public int DeliveryPeriodYear { get; set; }
        public string CollectionPeriodId { get; set; }
        public int CollectionPeriodMonth { get; set; }
        public int CollectionPeriodYear { get; set; }

        public DateTime EvidenceSubmittedOn { get; set; }
        public string EmployerAccountVersion { get; set; }
        public string ApprenticeshipVersion { get; set; }

        public int FundingSource { get; set; }
        public long? FundingAccountId { get; set; }
        public int TransactionType { get; set; }
        public decimal Amount { get; set; }

        public long? StandardCode { get; set; }
        public int? FrameworkCode { get; set; }
        public int? ProgrammeType { get; set; }
        public int? PathwayCode { get; set; }

        public int ContractType { get; set; }

        public int TotalCount { get; set; }

        public DateTime EarningsStartDate { get; set; }
        public DateTime? EarningsPlannedEndDate { get; set; }
        public DateTime? EarningsActualEndDate { get; set; }
        public byte? EarningsCompletionStatus { get; set; }
        public decimal? EarningsCompletionAmount { get; set; }
        public decimal? EarningsInstalmentAmount { get; set; }
        public short? EarningsNumberOfInstalments { get; set; }
    }
}
