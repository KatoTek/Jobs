using System;
using System.Configuration;
using static System.Configuration.ConfigurationManager;
using static System.GC;
using static System.String;

namespace Jobs.Runner
{
    public abstract class Job : IJob
    {
        #region fields

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

        public event JobExceptionThrownEventHandler ExceptionThrown;
        public event Action<string> Log;

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
                OnExceptionThrown(new JobExceptionThrownEventArguments { Exception = exception, Job = this });
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

        protected void OnExceptionThrown(JobExceptionThrownEventArguments args) => ExceptionThrown?.Invoke(this, args);
        protected void OnLog(string message) => Log?.Invoke(message);
        void IJob.Run(bool forceRun) => Run(forceRun);
        void IJob.Run() => Run(false);

        #endregion
    }
}
