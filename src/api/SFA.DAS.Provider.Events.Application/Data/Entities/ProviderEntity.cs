using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Provider.Events.Application.Data.Entities
{
    public class ProviderEntity
    {
        public long Ukprn { get;set; }
        public DateTime IlrSubmissionDateTime { get; set; }
        public string IlrFileName { get; set; }
        public bool RequiresInitialImport { get; set; }
        public int? HandledBy { get; set; }
    }
}
