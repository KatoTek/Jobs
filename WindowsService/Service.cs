using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using Jobs.Runner;
using Jobs.WindowsService.Configuration;
using static System.Diagnostics.EventLog;
using static System.Threading.Tasks.Task;
using static Jobs.WindowsService.Configuration.JobsServiceConfigurationSection;

namespace Jobs.WindowsService
{
    public partial class Service : ServiceBase, IExceptionThrown, IILog
    {
        #region fields

        internal static string ServiceSection = "jobs.windowsservice";
        readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        readonly List<JobExceptionThrownEventHandler> _jobExceptionThrownEventHandlers = new List<JobExceptionThrownEventHandler>();
        readonly JobsServiceConfigurationSection _jobsServiceConfig;
        readonly List<Action<string>> _logHandlers = new List<Action<string>>();
        bool _disposed;
        EventLog _eventLog;
        Task _task;

        #endregion

        #region constructors

        public Service()
        {
            InitializeComponent();

            CanPauseAndContinue = true;
            CanShutdown = true;

            _jobsServiceConfig = GetSection(ServiceSection);
        }

        #endregion

        #region events

        public event JobExceptionThrownEventHandler ExceptionThrown
        {
            add
            {
                if (_jobExceptionThrownEventHandlers.Contains(value))
                    return;

                _exceptionThrown += value;
                _jobExceptionThrownEventHandlers.Add(value);
            }
            remove
            {
                _exceptionThrown -= value;
                _jobExceptionThrownEventHandlers.Remove(value);
            }
        }

        public event Action<string> Log
        {
            add
            {
                if (_logHandlers.Contains(value))
                    return;

                _log += value;
                _logHandlers.Add(value);
            }
            remove
            {
                _log -= value;
                _logHandlers.Remove(value);
            }
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        event JobExceptionThrownEventHandler _exceptionThrown;

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        event Action<string> _log;

        #endregion

        #region methods

        public void Break(string state = "Stopped")
        {
            _cancellationTokenSource.Cancel();
            _task?.Wait();

            InvokeLog($"Service {state}");

            if (_eventLog != null)
                Log -= _eventLog.WriteEntry;
        }

        [SuppressMessage("ReSharper", "FunctionNeverReturns")]
        public void Launch(string state = "Started")
        {
            if (_eventLog != null)
                Log += _eventLog.WriteEntry;

            InvokeLog($"Service {state}");
            _task = Task.Run(async () =>
                                   {
                                       while (!_cancellationTokenSource.IsCancellationRequested)
                                       {
                                           DoWork(_cancellationTokenSource.Token);
                                           try
                                           {
                                               await Delay(_jobsServiceConfig.SecondsInterval*1000, _cancellationTokenSource.Token);
                                           }
                                           catch (OperationCanceledException) {}
                                       }
                                   });
        }

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

        protected override void OnContinue()
        {
            Launch("Continued");
            base.OnContinue();
        }

        protected override void OnPause()
        {
            Break("Paused");
            base.OnPause();
        }

        protected override void OnShutdown()
        {
            Break();
            base.OnShutdown();
        }

        protected override void OnStart(string[] args)
        {
            _eventLog = new EventLog { Source = _jobsServiceConfig.Log.Source, Log = _jobsServiceConfig.Log.Name };

            if (!SourceExists(_eventLog.Source))
                CreateEventSource(_eventLog.Source, _eventLog.Log);

            Launch();
            base.OnStart(args);
        }

        protected override void OnStop()
        {
            Break();
            base.OnStop();
        }

        void DoWork(CancellationToken cancellationToken)
        {
            using (var runner = new Runner.Runner())
            {
                try
                {
                    runner.Log += InvokeLog;
                    runner.ExceptionThrown += InvokeExceptionThrown;
                    runner.Run(cancellationToken);
                }
                finally
                {
                    runner.ExceptionThrown -= InvokeExceptionThrown;
                    runner.Log -= InvokeLog;
                }
            }
        }

        void InvokeExceptionThrown(object sender, JobExceptionThrownEventArguments args) => _exceptionThrown?.Invoke(sender, args);
        void InvokeLog(string message) => _log?.Invoke(message);

        #endregion
    }
}
