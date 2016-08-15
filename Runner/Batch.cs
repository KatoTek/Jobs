using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Encompass.Simple.Extensions;
using static System.Configuration.ConfigurationManager;
using static System.Configuration.ConfigurationSaveMode;
using static System.Configuration.ConfigurationUserLevel;
using static System.Environment;
using static System.GC;
using static System.Threading.Tasks.Task;

#if !DEBUG
#endif

namespace Jobs.Runner
{
    class Batch : IExceptionThrown, IILog, IDisposable
    {
        #region fields

        const string ERRORED = "Errored";
        const string FINISHED = "Finished";
        const string CANCELLED = "Cancelled";
        const string STARTED = "Started";
        readonly List<JobExceptionThrownEventHandler> _jobExceptionThrownEventHandlers = new List<JobExceptionThrownEventHandler>();
        readonly List<Action<string>> _logHandlers = new List<Action<string>>();
        List<KeyValueConfigurationElement> _appSettings = new List<KeyValueConfigurationElement>();
        List<ConnectionStringSettings> _connectionStrings = new List<ConnectionStringSettings>();
        bool _disposed;
        List<IJob> _jobs = new List<IJob>();

        #endregion

        #region constructors

        ~Batch()
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

        internal void Run(CancellationToken cancellationToken)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Batch));

            try
            {
                LoadConfiguration();

                WaitAll(_jobs.Select(_ => RunJobAsync(_, cancellationToken))
                             .ToArray());
            }
            finally
            {
                UnloadConfiguration();
            }
        }

        internal bool TryAdd(IJob job)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Batch));

            var configuration = job.GetConfiguration();
            if (!TryConfiguration(configuration))
                return false;

            _jobs.Add(job);
            return true;
        }

        void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (_jobs != null)
                {
                    foreach (var job in _jobs)
                        job.Dispose();

                    _jobs = null;
                }
            }

            _appSettings = null;
            _connectionStrings = null;

            _disposed = true;
        }

        void InvokeExceptionThrown(object sender, JobExceptionThrownEventArguments args) => _exceptionThrown?.Invoke(sender, args);
        void InvokeLog(string message) => _log?.Invoke(message);

        void LoadConfiguration()
        {
            var configuration = OpenExeConfiguration(None);

            foreach (var connectionString in _connectionStrings)
                configuration.ConnectionStrings.ConnectionStrings.Add(connectionString);

            foreach (var appSetting in _appSettings)
                configuration.AppSettings.Settings.Add(appSetting);

            configuration.Save(Modified);
            RefreshSection("connectionStrings");
            RefreshSection("appSettings");
        }

        async Task RunJobAsync(IJob job, CancellationToken cancellationToken)
        {
            var jobtype = job.GetType()
                             .FullName;

            job.Log += InvokeLog;
            job.ExceptionThrown += InvokeExceptionThrown;

            try
            {
                InvokeLog($"\t\tJob {jobtype} {STARTED}");
                await job.RunAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                InvokeLog($"\t\tJob {jobtype} {CANCELLED}");
            }
            catch (Exception exception)
            {
                InvokeLog($"\t\tJob {jobtype} {ERRORED}");
                InvokeLog($"{jobtype} Exception{NewLine}{exception.ToText()}");
                InvokeExceptionThrown(job, new JobExceptionThrownEventArguments { Exception = exception, Job = job });
            }
            finally
            {
                job.ExceptionThrown -= InvokeExceptionThrown;
                job.Log -= InvokeLog;

                InvokeLog($"\t\tJob {jobtype} {FINISHED}");

                job.Dispose();
            }
        }

        bool TryAppSettings(System.Configuration.Configuration configuration, out List<KeyValueConfigurationElement> safeSettings)
        {
            safeSettings = new List<KeyValueConfigurationElement>();
            foreach (KeyValueConfigurationElement setting in configuration.AppSettings.Settings)
            {
                var existingAppSetting = _appSettings.FirstOrDefault(firstAppSetting => firstAppSetting.Key == setting.Key);
                if (existingAppSetting != null)
                {
                    if (existingAppSetting.Value != setting.Value)
                        return false;
                }
                else
                    safeSettings.Add(setting);
            }
            return true;
        }

        bool TryConfiguration(System.Configuration.Configuration configuration)
        {
            if (configuration == null)
                return true;

            List<KeyValueConfigurationElement> safeSettings;
            if (!TryAppSettings(configuration, out safeSettings))
                return false;

            List<ConnectionStringSettings> safeConnectionStrings;
            if (!TryConnectionStrings(configuration, out safeConnectionStrings))
                return false;

            _appSettings.AddRange(safeSettings);
            _connectionStrings.AddRange(safeConnectionStrings);
            return true;
        }

        bool TryConnectionStrings(System.Configuration.Configuration configuration, out List<ConnectionStringSettings> safeConnectionStrings)
        {
            safeConnectionStrings = new List<ConnectionStringSettings>();
            foreach (ConnectionStringSettings connectionString in configuration.ConnectionStrings.ConnectionStrings)
            {
                var existingConnectionString = _connectionStrings.FirstOrDefault(firstConnectionString => firstConnectionString.Name == connectionString.Name);
                if (existingConnectionString != null)
                {
                    if ((existingConnectionString.ConnectionString != connectionString.ConnectionString) || (existingConnectionString.ProviderName != connectionString.ProviderName))
                        return false;
                }
                else
                    safeConnectionStrings.Add(connectionString);
            }
            return true;
        }

        void UnloadConfiguration()
        {
            var configuration = OpenExeConfiguration(None);

            foreach (var connectionString in _connectionStrings)
                configuration.ConnectionStrings.ConnectionStrings.Remove(connectionString.Name);

            foreach (var appSetting in _appSettings)
                configuration.AppSettings.Settings.Remove(appSetting.Key);

            configuration.Save(Modified);
            RefreshSection("connectionStrings");
            RefreshSection("appSettings");
        }

        #endregion
    }
}
