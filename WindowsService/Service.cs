using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using Encompass.Concepts.Mail;
using Jobs.Runner;
using Jobs.WindowsService.Configuration;

namespace Jobs.WindowsService
{
    public partial class Service : ServiceBase
    {
        #region fields

        const string WAIT = "wait";
        const string MAIL_SECTION = "jobs.service.mail";
        internal static string ServiceSection = "jobs.service";
        readonly List<JobExceptionThrownEventHandler> _jobExceptionThrownEventHandlers = new List<JobExceptionThrownEventHandler>();
        readonly JobsServiceConfigurationSection _jobsServiceConfig;
        readonly List<Action<string>> _logHandlers = new List<Action<string>>();
        readonly ManualResetEvent _shutdownEvent = new ManualResetEvent(false);
        bool _disposed;
        EventLog _eventLog;
        Task _task;
        bool _wait;

        #endregion

        #region constructors

        public Service()
        {
            InitializeComponent();

            _jobsServiceConfig = JobsServiceConfigurationSection.GetSection(ServiceSection);
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
            foreach (var arg in args.TakeWhile(_ => !_wait))
                _wait = arg.Equals(WAIT, StringComparison.CurrentCultureIgnoreCase);

            _eventLog = new EventLog { Source = _jobsServiceConfig.Log.Source, Log = _jobsServiceConfig.Log.Name };

            if (!EventLog.SourceExists(_eventLog.Source))
                EventLog.CreateEventSource(_eventLog.Source, _eventLog.Log);

            ExceptionThrown += InvokeExceptionThrown;
            ExceptionThrown += MailException;
            Log += InvokeLog;
            Log += _eventLog.WriteEntry;

            InvokeLog("Service Started");
            _task = Task.Factory.StartNew(DoWork);

            if (_wait)
                _task.Wait();
        }

        protected override void OnStop()
        {
            _shutdownEvent.Set();
            Task.WaitAll(_task);
            InvokeLog("Service Stopped");

            ExceptionThrown -= InvokeExceptionThrown;
            ExceptionThrown -= MailException;
            Log -= InvokeLog;
            Log -= _eventLog.WriteEntry;
        }

        static void MailException(object sender, JobExceptionThrownEventArguments args) => Mailer.Send(args.Exception, MAIL_SECTION);

        void DoWork()
        {
            while (!_shutdownEvent.WaitOne(0))
            {
                Thread.Sleep(1000);

                using (var runner = new Runner.Runner())
                {
                    try
                    {
                        runner.Log += InvokeLog;
                        runner.ExceptionThrown += InvokeExceptionThrown;
                        runner.Run();
                    }
                    finally
                    {
                        runner.ExceptionThrown -= InvokeExceptionThrown;
                        runner.Log -= InvokeLog;
                    }
                }
            }
        }

        void InvokeExceptionThrown(object sender, JobExceptionThrownEventArguments args) => _exceptionThrown?.Invoke(sender, args);
        void InvokeLog(string message) => _log?.Invoke(message);

        #endregion
    }
}
