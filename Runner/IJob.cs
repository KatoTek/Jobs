using System;
using System.Threading;
using System.Threading.Tasks;

namespace Jobs.Runner
{
    public interface IJob : IExceptionThrown, IILog, IDisposable
    {
        #region methods

        System.Configuration.Configuration GetConfiguration();
        Task RunAsync(CancellationToken cancellationToken);
        Task RunAsync(bool forceRun, CancellationToken cancellationToken);

        #endregion
    }
}
