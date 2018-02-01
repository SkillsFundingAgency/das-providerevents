using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using StructureMap;

namespace SFA.DAS.Provider.Events.DataLockEventWorker
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent _runCompleteEvent = new ManualResetEvent(false);
        private IContainer _container;
        private IDataLockProcessor _dataLockProcessor;

        public override void Run()
        {
            try
            {
                RunAsync(_cancellationTokenSource.Token).Wait();
            }
            finally
            {
                _runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            _container = ConfigureIocContainer();
            _dataLockProcessor = new DataLockProcessor();
            bool result = base.OnStart();

            Trace.TraceInformation("SFA.DAS.Provider.Events.DataLockEventWorker has been started");

            return result;
        }

        public override void OnStop()
        {
            _cancellationTokenSource.Cancel();
            _runCompleteEvent.WaitOne();

            base.OnStop();
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                await Task.Delay(60000);
            }
        }
        private IContainer ConfigureIocContainer()
        {
            var container = new Container(c =>
            {
                c.AddRegistry<DefaultRegistry>();
            });
            return container;
        }

    }
}
