using System;
using SFA.DAS.Payments.DCFS;
using SFA.DAS.Payments.DCFS.Context;
using SFA.DAS.Payments.DCFS.Infrastructure.DependencyResolution;
using SFA.DAS.Provider.Events.DataLock.Domain;
using SFA.DAS.Provider.Events.DataLock.Infrastructure.Context;
using SFA.DAS.Provider.Events.DataLock.Infrastructure.DependencyResolution;

namespace SFA.DAS.Provider.Events.DataLock
{
    public class DataLockEventsTask : DcfsTask
    {
        private const string SubmissionsSchema = "DataLockEvents";

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

        protected override bool IsValidContext(ContextWrapper contextWrapper)
        {
            base.IsValidContext(contextWrapper);

            if (string.IsNullOrEmpty(contextWrapper.GetPropertyValue(DataLockContextPropertyKeys.DataLockEventsSource)))
            {
                throw new InvalidContextException("The context does not contain the data lock events source property.");
            }

            ValidateEventsSource(contextWrapper.GetPropertyValue(DataLockContextPropertyKeys.DataLockEventsSource));

            if (string.IsNullOrEmpty(contextWrapper.GetPropertyValue(DataLockContextPropertyKeys.YearOfCollection)))
            {
                throw new InvalidContextException("The context does not contain the year of collection property.");
            }

            ValidateYearOfCollection(contextWrapper.GetPropertyValue(DataLockContextPropertyKeys.YearOfCollection));

            return true;
        }

        private void ValidateEventsSource(string eventsSource)
        {
            EventSource source;

            if (!Enum.TryParse(eventsSource, true, out source))
            {
                throw new InvalidContextException("The context does not contain a valid data lock events source property.");
            }
        }

        private void ValidateYearOfCollection(string yearOfCollection)
        {
            int year1;
            int year2;

            if (yearOfCollection.Length != 4 ||
                !int.TryParse(yearOfCollection.Substring(0, 2), out year1) ||
                !int.TryParse(yearOfCollection.Substring(2, 2), out year2) ||
                (year2 != year1 + 1))
            {
                throw new InvalidContextException("The context does not contain a valid year of collection property.");
            }
        }
    }
}
