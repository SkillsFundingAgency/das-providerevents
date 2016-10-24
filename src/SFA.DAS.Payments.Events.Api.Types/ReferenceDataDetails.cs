using System;

namespace SFA.DAS.Payments.Events.Api.Types
{
    public class ReferenceDataDetails
    {
        public DateTime AccountDataValidTill { get; set; }
        public DateTime CommitmentDataValidTill { get; set; }
    }
}