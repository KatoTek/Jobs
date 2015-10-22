using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Encompass.Simple.Extensions;
using static System.Configuration.ConfigurationManager;
using static System.Configuration.ConfigurationSaveMode;
using static System.Configuration.ConfigurationUserLevel;
using static System.GC;
using static System.String;
using static Encompass.Concepts.Mail.Mailer;
using static Jobs.Runner.Runner;

#if !DEBUG
using System.Threading.Tasks;

#endif

namespace Jobs.Runner
{
    internal class Batch : IDisposable
    {
        private const string ERRORED = "ERRORED";
        private const string EXCEPTION_EMAIL_CONTENT_FORMAT = "<h1>{0}</h1>";
        private const string FINISHED = "FINISHED";
        private const string JOB_FORMAT = "---- JOB {1} {0} ----";
        private const string STARTED = "STARTED";
        private readonly List<Action<string>> _onLogSubscribers = new List<Action<string>>();
        private List<KeyValueConfigurationElement> _appSettings = new List<KeyValueConfigurationElement>();
        private List<ConnectionStringSettings> _connectionStrings = new List<ConnectionStringSettings>();
        private bool _disposed;
        private List<IJob> _jobs = new List<IJob>();

        public void Dispose()
        {
            Dispose(true);
            SuppressFinalize(this);
        }

        ~Batch()
        {
            Dispose(false);
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private event Action<string> _onLog;

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

        internal void Run()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Batch));

            try
            {
                LoadConfiguration();

#if DEBUG
                _jobs.ForEach(RunJob);
#else
                Parallel.ForEach(_jobs, RunJob);
#endif
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

        private void Dispose(bool disposing)
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

        private void LoadConfiguration()
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

        private void Log(string message) => _onLog?.Invoke(message);
        private void Log(Exception exception) => Log(exception.ToText());

        private void OnExceptionThrown(object sender, JobExceptionThrownEventArguments args)
        {
            if (!args.RunnerIgnoresExceptions)
                SendException(args.Exception, args.Job);
        }

        private void RunJob(IJob job)
        {
            job.ExceptionThrown += OnExceptionThrown;
            job.OnLog += _onLog;

            try
            {
                Log(Format(JOB_FORMAT, STARTED, job.GetType().FullName));

                job.Run();
            }
            catch (Exception exception)
            {
                Log(Format(JOB_FORMAT, ERRORED, job.GetType().FullName));

                SendException(exception, job);
            }
            finally
            {
                job.ExceptionThrown -= OnExceptionThrown;
                job.OnLog -= _onLog;

                Log(Format(JOB_FORMAT, FINISHED, job.GetType().FullName));

                job.Dispose();
            }
        }

        private void SendException(Exception exception, IJob job)
        {
            Log(exception);
            Send(EXCEPTION_EMAIL_CONTENT_FORMAT.FormatWith(job.GetType().FullName), exception, MailSection);
        }

        private bool TryAppSettings(System.Configuration.Configuration configuration, out List<KeyValueConfigurationElement> safeSettings)
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

        private bool TryConfiguration(System.Configuration.Configuration configuration)
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

        private bool TryConnectionStrings(System.Configuration.Configuration configuration, out List<ConnectionStringSettings> safeConnectionStrings)
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

        private void UnloadConfiguration()
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
    }
}
