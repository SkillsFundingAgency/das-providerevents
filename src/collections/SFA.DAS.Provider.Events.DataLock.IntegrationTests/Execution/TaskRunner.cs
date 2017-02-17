using CS.Common.External.Interfaces;
using SFA.DAS.Provider.Events.DataLock.Domain;

namespace SFA.DAS.Provider.Events.DataLock.IntegrationTests.Execution
{
    public static class TaskRunner
    {
        public static void RunTask(IExternalContext taskContext = null, EventSource eventsSource = EventSource.Submission)
        {
            if (taskContext == null)
            {
                taskContext = new TestTaskContext(eventsSource);
            }

            var task = new DataLockEventsTask();
            task.Execute(taskContext);
        }
    }
}
