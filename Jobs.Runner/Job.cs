using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Encompass.Simple.Extensions;
using static System.GC;
using static System.String;

namespace Jobs.Runner
{
    public abstract class Job : IJob
    {
        private readonly List<JobExceptionThrownEventHandler> _exceptionThrownSubscribers = new List<JobExceptionThrownEventHandler>();
        private readonly List<Action<string>> _onLogSubscribers = new List<Action<string>>();
        private System.Configuration.Configuration _configuration;
        private bool _disposed;
        protected virtual bool RunnerIgnoresExceptions => false;

        public void Dispose()
        {
            Dispose(true);
            SuppressFinalize(this);
        }

        public event JobExceptionThrownEventHandler ExceptionThrown
        {
            add
            {
                if (_exceptionThrownSubscribers.Contains(value))
                    return;

                _exceptionThrown += value;
                _exceptionThrownSubscribers.Add(value);
            }
            remove
            {
                _exceptionThrown -= value;
                _exceptionThrownSubscribers.Remove(value);
            }
        }

        public virtual System.Configuration.Configuration GetConfiguration()
        {
            if (_configuration == null)
                _configuration = ConfigurationManager.OpenExeConfiguration(GetType().Assembly.Location);

            if (_configuration == null)
                throw new ConfigurationErrorsException("Configuration file is missing or cannot be read.");

            return _configuration;
        }

        public event Action<string> OnLog
        {
            add
            {
                if (_onLogSubscribers.Contains(value))
                    return;

                _onLog += value;
                _onLogSubscribers.Add(value);
            }
            remove
            {
                _onLog -= value;
                _onLogSubscribers.Remove(value);
            }
        }

        void IJob.Run(bool forceRun) => Run(forceRun);
        void IJob.Run() => Run(false);

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private event Action<string> _onLog;

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private event JobExceptionThrownEventHandler _exceptionThrown;

        ~Job()
        {
            Dispose(false);
        }

        public virtual void Run()
        {
            try
            {
                Run(false);
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }

        public abstract void Run(bool forceRun);

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            _disposed = true;
        }

        protected string GetConfigValue(string key, Type type = null)
        {
            if (IsNullOrWhiteSpace(key))
                return null;

            if (type == null)
                type = GetType();

            var value = ConfigurationManager.AppSettings[$"{type.FullName}.{key}"];

            if (value == null && type.BaseType != typeof(object))
                value = GetConfigValue(key, type.BaseType);

            return value;
        }

        protected virtual JobExceptionThrownEventArguments GetJobExceptionThrownEventArgument(Exception exception)
            => new JobExceptionThrownEventArguments { Exception = exception, RunnerIgnoresExceptions = RunnerIgnoresExceptions, Job = this };

        protected void HandleException(Exception exception) => _exceptionThrown?.Invoke(this, GetJobExceptionThrownEventArgument(exception));
        protected void Log(string message) => _onLog?.Invoke(message);
        protected void Log(Exception exception) => Log(exception.ToText());
    }
}
