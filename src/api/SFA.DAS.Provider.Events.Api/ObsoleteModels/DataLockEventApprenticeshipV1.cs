using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA.DAS.Provider.Events.Api.ObsoleteModels
{
    public class DataLockEventApprenticeshipV1
    {
      
        public long Version { get; set; }
        public DateTime StartDate { get; set; }
        public long? StandardCode { get; set; }
        public int? ProgrammeType { get; set; }
        public int? FrameworkCode { get; set; }
        public int? PathwayCode { get; set; }
        public decimal NegotiatedPrice { get; set; }
        public DateTime EffectiveDate { get; set; }
        
    }
}