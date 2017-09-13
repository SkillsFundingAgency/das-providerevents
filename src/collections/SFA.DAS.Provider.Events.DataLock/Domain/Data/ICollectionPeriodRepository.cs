using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.DataLock.Domain.Data
{
    public interface ICollectionPeriodRepository
    {
        CollectionPeriodEntity GetCurrentCollectionPeriod();
    }
}
