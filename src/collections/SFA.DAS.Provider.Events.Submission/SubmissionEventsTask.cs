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
        protected override bool IsValidContext(ContextWrapper contextWrapper)
        {
            base.IsValidContext(contextWrapper);


            if (string.IsNullOrEmpty(contextWrapper.GetPropertyValue(SubmissionEventsContextPropertyKeys.YearOfCollection)))
            {
                throw new InvalidContextException("The context does not contain the year of collection property.");
            }

            ValidateYearOfCollection(contextWrapper.GetPropertyValue(SubmissionEventsContextPropertyKeys.YearOfCollection));

            return true;
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
