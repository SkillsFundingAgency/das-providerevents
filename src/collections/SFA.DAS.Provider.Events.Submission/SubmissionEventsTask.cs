using SFA.DAS.Payments.DCFS;
using SFA.DAS.Payments.DCFS.Context;
using SFA.DAS.Payments.DCFS.Infrastructure.DependencyResolution;
using SFA.DAS.Provider.Events.Submission.Infrastructure.DependencyResolution;

namespace SFA.DAS.Provider.Events.Submission
{
    public class SubmissionEventsTask : DcfsTask
    {
        public const int ComponentVersion = 1;
        private const string SubmissionsSchema = "Submissions";

        private readonly IDependencyResolver _dependencyResolver;

        public SubmissionEventsTask() 
            : base(SubmissionsSchema)
        {
            _dependencyResolver = new TaskDependencyResolver();
        }
        public SubmissionEventsTask(IDependencyResolver dependencyResolver)
            : base(SubmissionsSchema)
        {
            _dependencyResolver = dependencyResolver;
        }

        protected override void Execute(ContextWrapper context)
        {
            _dependencyResolver.Init(typeof(SubmissionEventsProcessor), context);

            var processor = _dependencyResolver.GetInstance<SubmissionEventsProcessor>();

            processor.Process();
        }
    }
}
