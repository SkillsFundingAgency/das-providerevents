using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Provider.Events.Application.Data.Entities
{
    public class DataLockEntity
    {
        public long Ukprn { get; set; }
        public string LearnerReferenceNumber { get; set; }
        public long? AimSequenceNumber { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public string ErrorCodes { get; set; }
        public string Commitments { get; set; }
    }
}
