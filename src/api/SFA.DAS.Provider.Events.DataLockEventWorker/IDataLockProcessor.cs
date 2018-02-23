using System.Threading.Tasks;

namespace SFA.DAS.Provider.Events.DataLockEventWorker
{
    public interface IDataLockProcessor
    {
        Task ProcessDataLocks(int? pageSize = null);
    }
}
