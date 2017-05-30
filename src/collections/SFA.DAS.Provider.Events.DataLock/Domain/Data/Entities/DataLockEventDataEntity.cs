using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities
{
    public class DataLockEventDataEntity
    {
        public long Ukprn { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public string LearnRefNumber { get; set; }
        public int AimSeqNumber { get; set; }
        public long CommitmentId { get; set; }
        public bool IsSuccess { get; set; }
        public string IlrFilename { get; set; }
        public DateTime SubmittedTime { get; set; }
        public long Uln { get; set; }
        public DateTime IlrStartDate { get; set; }
        public long? IlrStandardCode { get; set; }
        public int? IlrProgrammeType { get; set; }
        public int? IlrFrameworkCode { get; set; }
        public int? IlrPathwayCode { get; set; }
        public decimal IlrTrainingPrice { get; set; }
        public decimal IlrEndpointAssessorPrice { get; set; }
        public DateTime IlrPriceEffectiveFromDate { get; set; }
        public DateTime? IlrPriceEffectiveToDate { get; set; }
        public long CommitmentVersionId { get; set; }
        public int Period { get; set; }
        public bool Payable { get; set; }
        public int TransactionType { get; set; }
        public long EmployerAccountId { get; set; }
        public DateTime CommitmentStartDate { get; set; }
        public long? CommitmentStandardCode { get; set; }
        public int? CommitmentProgrammeType { get; set; }
        public int? CommitmentFrameworkCode { get; set; }
        public int? CommitmentPathwayCode { get; set; }
        public decimal CommitmentNegotiatedPrice { get; set; }
        public DateTime CommitmentEffectiveDate { get; set; }
        public string RuleId { get; set; }
    }
}
