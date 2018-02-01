using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Provider.Events.Application.Data.Entities
{
    public class DataLockPriceEpisodeMatchEntity
    {
        public long Ukprn { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public string LearnRefNumber { get; set; }
        public long AimSeqNumber { get; set; }
        public long CommitmentId { get; set; }
        public bool IsSuccess { get; set; }
    }
}
