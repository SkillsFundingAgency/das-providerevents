using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SFA.DAS.Provider.Events.DataLockEventWorker.AcceptanceTests
{
    public abstract class DataLockProcessorTestBase
    {
        private WorkerRole _workerRole;
        //private ILogger _logger = new NullLogger(new LogFactory());

        [OneTimeSetUp]
        public void FixtureSetUp()
        {
            _workerRole = new WorkerRole();
            _workerRole.OnStart();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _workerRole.OnStop();
        }

        [SetUp]
        public virtual void SetupBase()
        {
            Task.WaitAll(
                Task.Run(() => TestDataHelperDeds.Clean()),
                Task.Run(() => TestDataHelperDataLockEventsDatabase.Clean())
            );
        }

        protected void Act(Func<bool> isComplete, int timeoutSeconds = 30)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            Task.Run(() => _workerRole.Run(), cancellationToken);
            var start = DateTime.Now;

            while (!isComplete())
            {
                Thread.Sleep(500);
                if (DateTime.Now.Subtract(start).TotalSeconds > timeoutSeconds)
                {
                    cancellationTokenSource.Cancel();
                    Assert.Fail("Test timeout expired");
                }
            }
        }

    }
}
