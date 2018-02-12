using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Provider.Events.Application.Data.Entities
{
    public class DataLockCommitmentEntity
    {
        public long Ukprn { get; set; }
        public string LearnerReferenceNumber { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public long? CommitmentId { get; set; }
        public long? VersionId { get;set; }
    }
}
