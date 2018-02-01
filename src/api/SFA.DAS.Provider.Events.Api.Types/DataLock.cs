using System.Collections.Generic;

namespace SFA.DAS.Provider.Events.Api.Types
{
    public class DataLock
    {
        public long Ukprn { get; set; }
        public string LearnerReferenceNumber { get; set; }
        public long? AimSequenceNumber { get; set; }
        public string PriceEpisodeIdentifier { get; set; }

        public IList<string> ErrorCodes { get; set; }
        public IList<long> Commitments { get; set; }

        public bool IsSuccess => ErrorCodes == null || ErrorCodes.Count == 0;
    }
}