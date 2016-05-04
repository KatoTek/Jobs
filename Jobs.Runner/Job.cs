using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Encompass.Simple.Extensions;
using static System.Configuration.ConfigurationManager;
using static System.GC;
using static System.String;

namespace Jobs.Runner
{
    public abstract class Job : IJob
    {
        #region fields

        readonly List<JobExceptionThrownEventHandler> _exceptionThrownSubscribers = new List<JobExceptionThrownEventHandler>();
        readonly List<Action<string>> _onLogSubscribers = new List<Action<string>>();
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

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        event JobExceptionThrownEventHandler _exceptionThrown;

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        event Action<string> _onLog;

        #endregion

        #region properties

        protected virtual bool RunnerIgnoresExceptions => false;

        #endregion

        #region methods

        public void Dispose()
        {
            Dispose(true);
            SuppressFinalize(this);
        }

        public virtual System.Configuration.Configuration GetConfiguration()
        {
            if (_configuration == null)
            {
                _configuration = OpenExeConfiguration(GetType()
                                                          .Assembly.Location);
            }

            if (_configuration == null)
                throw new ConfigurationErrorsException("Configuration file is missing or cannot be read.");

            return _configuration;
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

            var value = AppSettings[$"{type.FullName}.{key}"];

            if (value == null && type.BaseType != typeof(object))
                value = GetConfigValue(key, type.BaseType);

            return value;
        }

        protected virtual JobExceptionThrownEventArguments GetJobExceptionThrownEventArgument(Exception exception)
            => new JobExceptionThrownEventArguments { Exception = exception, RunnerIgnoresExceptions = RunnerIgnoresExceptions, Job = this };

        protected void HandleException(Exception exception) => _exceptionThrown?.Invoke(this, GetJobExceptionThrownEventArgument(exception));
        protected void Log(string message) => _onLog?.Invoke(message);
        protected void Log(Exception exception) => Log(exception.ToText());
        void IJob.Run(bool forceRun) => Run(forceRun);
        void IJob.Run() => Run(false);

        #endregion
    }
}
