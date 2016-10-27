using System;

namespace SFA.DAS.Payments.Events.Api.Types
{
    public class ReferenceDataDetails
    {
        public DateTime AccountDataValidAt { get; set; }
        public DateTime CommitmentDataValidAt { get; set; }
    }
}