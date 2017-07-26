using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities
{
    public class CollectionPeriodEntity
    {
        public int PeriodId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string Name { get; set; }
    }
}
