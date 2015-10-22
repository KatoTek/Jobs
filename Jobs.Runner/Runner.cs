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
        private const string BATCH_FORMAT = "==== BATCH {1} {0} ====";
        private const string FINISHED = "JOBS.RUNNER FINISHED";
        private const string MAIL_SECTION = "jobs.runner.mail";
        private const string RUNNER_SECTION = "jobs.runner";
        private const string STARTED = "JOBS.RUNNER STARTED";
        private readonly CompositionContainer _compositionContainer;
        private readonly List<DirectoryCatalog> _directoryCatalogs = new List<DirectoryCatalog>();
        private readonly List<Action<string>> _onLogSubscribers = new List<Action<string>>();
        private bool _disposed;
        [ImportMany(typeof(IJob))] private IEnumerable<Lazy<IJob>> _lazyJobs;

        public Runner()
        {
            var aggregateCatalog = new AggregateCatalog(new AssemblyCatalog(GetExecutingAssembly()));

            var jobrunnerConfig = GetSection(RunnerSection);
            if (jobrunnerConfig != null)
            {
                foreach (var directoryCatalog in
                    from PluginPath pluginpath in jobrunnerConfig.PluginPaths
                    where Exists($@"{GetDirectoryName(GetExecutingAssembly().Location)}\{pluginpath.FolderPath}")
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

        public static string MailSection => MAIL_SECTION;
        internal static string RunnerSection => RUNNER_SECTION;

        public void Dispose()
        {
            Dispose(true);
            SuppressFinalize(this);
        }

        ~Runner()
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

        private IEnumerable<Batch> BatchJobs()
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

        private void Dispose(bool disposing)
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

        private void HandleException(Exception exception)
        {
            Log(exception);
            Send(exception, MailSection);
        }

        private void Log(string message) => _onLog?.Invoke(message);
        private void Log(Exception exception) => Log(exception.ToText());

        private Batch NewBatch(IJob job = null)
        {
            var batch = new Batch();
            batch.OnLog += _onLog;
            if (job != null)
                batch.TryAdd(job);

            return batch;
        }
    }
}
