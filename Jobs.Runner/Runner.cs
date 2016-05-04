using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Encompass.Simple.Extensions;
using Jobs.Runner.Configuration;
using Jobs.Runner.Configuration.Exceptions;
using static System.GC;
using static System.IO.Directory;
using static System.IO.Path;
using static System.Reflection.Assembly;
using static System.String;
using static Encompass.Concepts.Mail.Mailer;
using static Jobs.Runner.Configuration.JobRunnerConfigurationSection;

namespace Jobs.Runner
{
    public sealed class Runner : IDisposable
    {
        #region fields

        const string BATCH_FORMAT = "==== BATCH {1} {0} ====";
        const string FINISHED = "JOBS.RUNNER FINISHED";
        const string MAIL_SECTION = "jobs.runner.mail";
        const string RUNNER_SECTION = "jobs.runner";
        const string STARTED = "JOBS.RUNNER STARTED";
        readonly CompositionContainer _compositionContainer;
        readonly List<DirectoryCatalog> _directoryCatalogs = new List<DirectoryCatalog>();
        readonly List<Action<string>> _onLogSubscribers = new List<Action<string>>();
        bool _disposed;
        [ImportMany(typeof(IJob))] IEnumerable<Lazy<IJob>> _lazyJobs;

        #endregion

        #region constructors

        public Runner()
        {
            var aggregateCatalog = new AggregateCatalog(new AssemblyCatalog(GetExecutingAssembly()));

            var jobrunnerConfig = GetSection(RunnerSection);
            if (jobrunnerConfig != null)
            {
                foreach (var directoryCatalog in
                    from PluginPath pluginpath in jobrunnerConfig.PluginPaths
                    where Exists($@"{GetDirectoryName(GetExecutingAssembly()
                                                          .Location)}\{pluginpath.FolderPath}")
                    select new DirectoryCatalog(pluginpath.FolderPath, pluginpath.SearchPattern))
                {
                    _directoryCatalogs.Add(directoryCatalog);
                    aggregateCatalog.Catalogs.Add(directoryCatalog);
                }
            }
            else
                throw new ConfigurationSectionMissingException(RunnerSection);

            _compositionContainer = new CompositionContainer(aggregateCatalog);

            try
            {
                _compositionContainer.ComposeParts(this);
            }
            catch (Exception exception)
            {
                HandleException(exception);
                throw;
            }
        }

        ~Runner()
        {
            Dispose(false);
        }

        #endregion

        #region events

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
        event Action<string> _onLog;

        #endregion

        #region properties

        public static string MailSection => MAIL_SECTION;
        internal static string RunnerSection => RUNNER_SECTION;

        #endregion

        #region methods

        public void Dispose()
        {
            Dispose(true);
            SuppressFinalize(this);
        }

        public void Run()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Runner));

            _directoryCatalogs.ForEach(directoryCatalog => directoryCatalog.Refresh());

            var batchNum = 0;

            Log(STARTED);
            foreach (var batch in BatchJobs())
            {
                try
                {
                    batchNum++;
                    Log(Format(BATCH_FORMAT, STARTED, batchNum));

                    batch.Run();
                }
                finally
                {
                    batch.OnLog -= _onLog;
                    batch.Dispose();

                    Log(Format(BATCH_FORMAT, FINISHED, batchNum));
                }
            }
            Log(FINISHED);
        }

        IEnumerable<Batch> BatchJobs()
        {
            var batches = new List<Batch> { NewBatch() };

            foreach (var lazyJob in _lazyJobs)
            {
                var job = lazyJob.Value;

                var jobAdded = false;
                foreach (var batch in batches)
                {
                    jobAdded = batch.TryAdd(job);
                    if (jobAdded)
                        break;
                }

                if (!jobAdded)
                    batches.Add(NewBatch(job));
            }

            return batches;
        }

        void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _compositionContainer?.Dispose();

                if (_lazyJobs != null)
                {
                    foreach (var lazyJob in _lazyJobs.Where(lazyJob => lazyJob.IsValueCreated))
                        lazyJob.Value.Dispose();

                    _lazyJobs = null;
                }
            }

            _disposed = true;
        }

        void HandleException(Exception exception)
        {
            Log(exception);
            Send(exception, MailSection);
        }

        void Log(string message) => _onLog?.Invoke(message);
        void Log(Exception exception) => Log(exception.ToText());

        Batch NewBatch(IJob job = null)
        {
            var batch = new Batch();
            batch.OnLog += _onLog;
            if (job != null)
                batch.TryAdd(job);

            return batch;
        }

        #endregion
    }
}
