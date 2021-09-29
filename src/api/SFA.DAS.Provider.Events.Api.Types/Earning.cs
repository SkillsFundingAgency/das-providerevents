﻿using System;

namespace SFA.DAS.Provider.Events.Api.Types
{
    public class Earning
    {
        public Guid RequiredPaymentId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public DateTime ActualEndDate { get; set; }
        public int CompletionStatus { get; set; }
        public decimal CompletionAmount { get; set; }
        public decimal MonthlyInstallment { get; set; }
        public int TotalInstallments { get; set; }
        public string EndpointAssessorId { get; set; }
    }
}
