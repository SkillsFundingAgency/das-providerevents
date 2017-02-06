using System;

namespace SFA.DAS.Provider.Events.Domain.Data.Entities
{
    public class SubmissionEventEntity
    {
        public long Id { get; set; }
        public string IlrFileName { get; set; }
        public DateTime FileDateTime { get; set; }
        public DateTime SubmittedDateTime { get; set; }
        public int ComponentVersionNumber { get; set; }
        public long Ukprn { get; set; }
        public long Uln { get; set; }
        public long? StandardCode { get; set; }
        public int? ProgrammeType { get; set; }
        public int? FrameworkCode { get; set; }
        public int? PathwayCode { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public decimal? OnProgrammeTotalPrice { get; set; }
        public decimal? CompletionTotalPrice { get; set; }
        public string NiNumber { get; set; }
        public string CommitmentId { get; set; }
    }
}
