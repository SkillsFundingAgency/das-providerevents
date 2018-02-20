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
        public long CommitmentId { get; set; }
        public string ErrorCodes { get; set; }
        public long EmployerAccountId { get; set; }
        public DateTime? DeletedUtc { get; set; }

        public long Uln { get; set; }
        public DateTime? IlrStartDate { get; set; }
        public long? IlrStandardCode { get; set; }
        public int? IlrProgrammeType { get; set; }
        public int? IlrFrameworkCode { get; set; }
        public int? IlrPathwayCode { get; set; }

        public decimal? IlrTrainingPrice { get; set; }
        public decimal? IlrEndpointAssessorPrice { get; set; }
        public DateTime? IlrPriceEffectiveFromDate { get; set; }
        public DateTime? IlrPriceEffectiveToDate { get; set; }
    }
}
