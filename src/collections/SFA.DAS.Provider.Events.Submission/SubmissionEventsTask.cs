using System;
using SFA.DAS.Payments.DCFS;
using SFA.DAS.Payments.DCFS.Context;
using SFA.DAS.Payments.DCFS.Infrastructure.DependencyResolution;

namespace SFA.DAS.Provider.Events.Submission
{
    public class SubmissionEventsTask : DcfsTask
    {
        private const string SubmissionsSchema = "Submissions";

        private IDependencyResolver _dependencyResolver;

        public SubmissionEventsTask() 
            : base(SubmissionsSchema)
        {
        }
        public SubmissionEventsTask(IDependencyResolver dependencyResolver)
            : base(SubmissionsSchema)
        {
            _dependencyResolver = dependencyResolver;
        }

        protected override void Execute(ContextWrapper context)
        {
            throw new NotImplementedException();
        }
    }
}
