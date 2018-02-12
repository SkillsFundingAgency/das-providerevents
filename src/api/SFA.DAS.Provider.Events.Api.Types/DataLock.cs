using System;
using System.Collections.Generic;

namespace SFA.DAS.Provider.Events.Api.Types
{
    public class DataLock
    {
        public long Ukprn { get; set; }
        public string LearnerReferenceNumber { get; set; }
        public string PriceEpisodeIdentifier { get; set; }

        public long? AimSequenceNumber { get; set; }

        public IList<string> ErrorCodes { get; set; }
        public IList<DataLockEventApprenticeship> CommitmentVersions { get; set; }

        public bool IsSuccess => ErrorCodes == null || ErrorCodes.Count == 0;

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