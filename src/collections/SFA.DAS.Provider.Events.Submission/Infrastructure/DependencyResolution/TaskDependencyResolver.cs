using System;
using SFA.DAS.Payments.DCFS.Context;
using SFA.DAS.Payments.DCFS.StructureMap.Infrastructure.DependencyResolution;
using StructureMap;

namespace SFA.DAS.Provider.Events.Submission.Infrastructure.DependencyResolution
{
    public class TaskDependencyResolver : StructureMapDependencyResolver
    {
        protected override Registry CreateRegistry(Type taskType, ContextWrapper contextWrapper)
        {
            return new SubmissionEventsRegistry(taskType);
        }

        protected override void AddPolicies(ConfigurationExpression config, Type taskType, ContextWrapper contextWrapper)
        {
            base.AddPolicies(config, taskType, contextWrapper);
            config.Policies.Add(new YearOfCollectionPolicy(contextWrapper));
        }
    }
}
