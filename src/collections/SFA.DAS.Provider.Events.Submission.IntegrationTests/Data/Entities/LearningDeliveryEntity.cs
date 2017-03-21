using System;

namespace SFA.DAS.Provider.Events.Submission.IntegrationTests.Data.Entities
{
    public class LearningDeliveryEntity
    {
        public long Ukprn { get; set; }
        public string LearnRefNumber { get; set; }
        public long AimSeqNumber { get; set; }
        public int ProgType { get; set; }
        public int FworkCode { get; set; }
        public int PwayCode { get; set; }
        public int StdCode { get; set; }
        public DateTime LearnStartDate { get; set; }
        public DateTime LearnPlanEndDate { get; set; }
        public DateTime? LearnActEndDate { get; set; }
    }
}
