using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using Jobs.Service.Configuration;
using static System.Diagnostics.EventLog;
using static System.StringComparison;
using static System.Threading.Tasks.Task;
using static System.Threading.Thread;

namespace Jobs.Service
{
    public partial class Service : ServiceBase
    {
        internal static string ServiceSection = "jobs.service";
        private const string DEBUG = "debug";
        private const string WAIT = "wait";
        private readonly JobsServiceConfigurationSection _jobsServiceConfig;
        private readonly ManualResetEvent _shutdownEvent = new ManualResetEvent(false);
        private bool _disposed;
        private EventLog _eventLog;
        private Task _task;
        private bool _wait;

        public Service()
        {
            InitializeComponent();

            _jobsServiceConfig = JobsServiceConfigurationSection.GetSection(ServiceSection);
        }

        protected event Action<string> OnLog;

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                components?.Dispose();

                _disposed = true;
            }

            base.Dispose(disposing);
        }

        protected override void OnStart(string[] args)
        {
            var debug = false;
            foreach (var arg in args)
            {
                if (!debug)
                    debug = arg.Equals(DEBUG, CurrentCultureIgnoreCase);

                if (!_wait)
                    _wait = arg.Equals(WAIT, CurrentCultureIgnoreCase);
            }

            if (debug)
            {
                _eventLog = new EventLog { Source = _jobsServiceConfig.Log.Source, Log = _jobsServiceConfig.Log.Name };

                if (!SourceExists(_eventLog.Source))
                    CreateEventSource(_eventLog.Source, _eventLog.Log);

                OnLog += _eventLog.WriteEntry;
            }

            Log("Service Started");
            _task = Factory.StartNew(DoWork);

            if (_wait)
                _task.Wait();
        }

        protected override void OnStop()
        {
            _shutdownEvent.Set();
            WaitAll(_task);
            Log("Service Stopped");
        }

        private void DoWork()
        {
            while (!_shutdownEvent.WaitOne(0))
            {
                Sleep(1000);

                using (var runner = new Runner.Runner())
                {
                    runner.OnLog += OnLog;
                    runner.Run();
                    runner.OnLog -= OnLog;
                }
            }
        }

        private void Log(string message) => OnLog?.Invoke(message);
    }
}
