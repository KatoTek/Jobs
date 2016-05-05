using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Jobs.Runner.Configuration;
using Jobs.Runner.Configuration.Exceptions;
using static System.GC;
using static System.IO.Directory;
using static System.IO.Path;
using static System.Reflection.Assembly;
using static Jobs.Runner.Configuration.JobRunnerConfigurationSection;

namespace Jobs.Runner
{
    public sealed class Runner : IDisposable
    {
        #region fields

        const string RUNNER_SECTION = "jobs.runner";
        readonly CompositionContainer _compositionContainer;
        readonly List<DirectoryCatalog> _directoryCatalogs = new List<DirectoryCatalog>();
        readonly List<JobExceptionThrownEventHandler> _jobExceptionThrownEventHandlers = new List<JobExceptionThrownEventHandler>();
        readonly List<Action<string>> _logHandlers = new List<Action<string>>();
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
            _compositionContainer.ComposeParts(this);
        }

        ~Runner()
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

        #region properties

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

            InvokeLog("= JOBS.RUNNER STARTED =");
            foreach (var batch in BatchJobs())
            {
                try
                {
                    InvokeLog($"== JOBS.RUNNER BATCH {batchNum++} STARTED ==");

                    batch.Run();
                }
                finally
                {
                    batch.ExceptionThrown -= InvokeExceptionThrown;
                    batch.Log -= InvokeLog;
                    batch.Dispose();

                    InvokeLog($"== JOBS.RUNNER BATCH {batchNum} FINISHED ==");
                }
            }
            InvokeLog("= JOBS.RUNNER FINISHED =");
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

        void InvokeExceptionThrown(object sender, JobExceptionThrownEventArguments args) => _exceptionThrown?.Invoke(sender, args);
        void InvokeLog(string message) => _log?.Invoke(message);

        Batch NewBatch(IJob job = null)
        {
            var batch = new Batch();
            batch.Log += InvokeLog;
            batch.ExceptionThrown += InvokeExceptionThrown;
            if (job != null)
                batch.TryAdd(job);

            return batch;
        }

        #endregion
    }
}
