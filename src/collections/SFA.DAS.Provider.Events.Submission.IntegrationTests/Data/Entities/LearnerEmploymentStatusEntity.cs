using System;

namespace SFA.DAS.Provider.Events.Submission.IntegrationTests.Data.Entities
{
    public class LearnerEmploymentStatusEntity
    {
        public long  Ukprn { get; set; }
        public string LearnRefNumber { get; set; }
        public int EmploymentStatus { get; set; }
        public DateTime EmploymentStatusDate { get; set; }
        public int EmployerId { get; set; }
    }
}