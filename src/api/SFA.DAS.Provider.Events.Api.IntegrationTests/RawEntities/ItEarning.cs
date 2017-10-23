using System;
using System.ComponentModel.DataAnnotations;


namespace SFA.DAS.Provider.Events.Api.IntegrationTests.RawEntities
{
    class ItEarning
    {
        public Guid RequiredPaymentId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public DateTime ActualEndDate { get; set; }
        [Range(1, 5)]
        public int CompletionStatus { get; set; }
        [Range(100, 10000)]
        public decimal CompletionAmount { get; set; }
        [Range(100, 35000)]
        public decimal MonthlyInstallment { get; set; }
        [Range(1, 12)]
        public int TotalInstallments { get; set; }
    }
}
