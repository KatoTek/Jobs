using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using static System.Configuration.ConfigurationManager;
using static System.GC;
using static System.String;
using static System.Threading.Tasks.Task;

namespace Jobs.Runner
{
    public abstract class Job : IJob
    {
        #region fields

        readonly List<JobExceptionThrownEventHandler> _jobExceptionThrownEventHandlers = new List<JobExceptionThrownEventHandler>();
        readonly List<Action<string>> _logHandlers = new List<Action<string>>();
        System.Configuration.Configuration _configuration;
        bool _disposed;

        #endregion

        #region constructors

        ~Job()
        {
            Dispose(false);
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

        public void Dispose()
        {
            Dispose(true);
            SuppressFinalize(this);
        }

        public System.Configuration.Configuration GetConfiguration()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Job));

            if (_configuration == null)
            {
                _configuration = OpenExeConfiguration(GetType()
                                                          .Assembly.Location);
            }

            if (_configuration == null)
                throw new ConfigurationErrorsException("Configuration file is missing or cannot be read.");

            return _configuration;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Job));

            try
            {
                await RunAsync(false, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                InvokeExceptionThrown(new JobExceptionThrownEventArguments { Exception = exception, Job = this });
            }
        }

        public virtual async Task RunAsync(bool forceRun, CancellationToken cancellationToken) => await Delay(1000, cancellationToken);

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            _disposed = true;
        }

        protected string GetConfigValue(string key, Type type = null)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Job));

            if (IsNullOrWhiteSpace(key))
                return null;

            if (type == null)
                type = GetType();

            var value = AppSettings[$"{type.FullName}.{key}"];

            if (value == null && type.BaseType != typeof(object))
                value = GetConfigValue(key, type.BaseType);

            return value;
        }

        protected void InvokeExceptionThrown(JobExceptionThrownEventArguments args) => _exceptionThrown?.Invoke(this, args);
        protected void InvokeLog(string message) => _log?.Invoke(message);
        async Task IJob.RunAsync(bool forceRun, CancellationToken cancellationToken) => await RunAsync(forceRun, cancellationToken);
        async Task IJob.RunAsync(CancellationToken cancellationToken) => await RunAsync(false, cancellationToken);

        #endregion
    }
}
