using SFA.DAS.Payments.DCFS;
using SFA.DAS.Payments.DCFS.Context;
using SFA.DAS.Payments.DCFS.Infrastructure.DependencyResolution;
using SFA.DAS.Provider.Events.DataLock.Infrastructure.DependencyResolution;

namespace SFA.DAS.Provider.Events.DataLock
{
    public class DataLockEventsTask : DcfsTask
    {
        private const string SubmissionsSchema = "DataLock";

        private readonly IDependencyResolver _dependencyResolver;

        public DataLockEventsTask() 
            : base(SubmissionsSchema)
        {
            _dependencyResolver = new TaskDependencyResolver();
        }
        public DataLockEventsTask(IDependencyResolver dependencyResolver)
            : base(SubmissionsSchema)
        {
            _dependencyResolver = dependencyResolver;
        }

        protected override void Execute(ContextWrapper context)
        {
            _dependencyResolver.Init(typeof(DataLockEventsProcessor), context);

            var processor = _dependencyResolver.GetInstance<DataLockEventsProcessor>();

            processor.Process();
        }
    }
}
