using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Provider.Events.DataLockEventWorker
{
    public interface IDataLockProcessor
    {
        Task ProcessDataLocks();
    }
}
