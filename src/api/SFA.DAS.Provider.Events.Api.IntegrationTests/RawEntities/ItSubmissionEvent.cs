using System;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.Provider.Events.Api.IntegrationTests.RawEntities
{
    public class ItSubmissionEvent
    {
        public long Id { get; set; }
        [StringLength(50)]
        public string IlrFileName { get; set; }
        public DateTime FileDateTime { get; set; }
        public DateTime SubmittedDateTime { get; set; }
        public int ComponentVersionNumber { get; set; }
        public long Ukprn { get; set; }
        [Range(1000000000, 1000001000)]
        public long Uln { get; set; }
        [StringLength(12)] // 100 in db table
        public string LearnRefNumber { get; set; } // not in SubmissionEvent
        public long AimSeqNumber { get; set; } // not in SubmissionEvent
        [StringLength(25)]
        public string PriceEpisodeIdentifier { get; set; } // not in SubmissionEvent
        public long? StandardCode { get; set; }
        public int? ProgrammeType { get; set; }
        public int? FrameworkCode { get; set; }
        public int? PathwayCode { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        [Range(100, 10000)]
        public decimal? OnProgrammeTotalPrice { get; set; } // TrainingPrice in SubmissionEvent
        public decimal? CompletionTotalPrice { get; set; } // EndpointAssessorPrice in SubmissionEvent
        [StringLength(9)]
        public string NiNumber { get; set; }
        public long? CommitmentId { get; set; } // ApprenticeshipId in SubmissionEvent
        [StringLength(4)]
        public string AcademicYear { get; set; }
        public int? EmployerReferenceNumber { get; set; }
        [StringLength(7)]
        public string EPAOrgId { get; set; }
        [StringLength(100)]
        public string GivenNames { get; set; }
        [StringLength(100)]
        public string FamilyName { get; set; }
        public int? CompStatus { get; set; }
    }
}
