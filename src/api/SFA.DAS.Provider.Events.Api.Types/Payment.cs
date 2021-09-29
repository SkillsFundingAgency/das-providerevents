﻿using System;
using System.Collections.Generic;

namespace SFA.DAS.Provider.Events.Api.Types
{
    public class Payment
    {
        public string Id { get; set; }
        public long Ukprn { get; set; }
        public long Uln { get; set; }
        public string EmployerAccountId { get; set; }
        public long? ApprenticeshipId { get; set; }

        public CalendarPeriod DeliveryPeriod { get; set; }
        public NamedCalendarPeriod CollectionPeriod { get; set; }

        public DateTime EvidenceSubmittedOn { get; set; }
        public string EmployerAccountVersion { get; set; }
        public string ApprenticeshipVersion { get; set; }

        public FundingSource FundingSource { get; set; }
        public long? FundingAccountId { get; set; }
        public TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }

        public long? StandardCode { get; set; }
        public int? FrameworkCode { get; set; }
        public int? ProgrammeType { get; set; }
        public int? PathwayCode { get; set; }

        public ContractType ContractType { get; set; }
        public List<Earning> EarningDetails { get; set; }
    }
}
