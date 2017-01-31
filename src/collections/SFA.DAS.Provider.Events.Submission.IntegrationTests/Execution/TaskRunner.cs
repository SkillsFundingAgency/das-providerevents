using CS.Common.External.Interfaces;

namespace SFA.DAS.Provider.Events.Submission.IntegrationTests.Execution
{
    public static class TaskRunner
    {
        public static void RunTask(IExternalContext taskContext = null)
        {
            if (taskContext == null)
            {
                taskContext = new TestTaskContext();
            }
            var task = new SubmissionEventsTask();
            task.Execute(taskContext);
        }
    }
}
