using System;

namespace Jobs.Runner
{
    public interface IJob : IDisposable
    {
        event JobExceptionThrownEventHandler ExceptionThrown;
        event Action<string> OnLog;

        System.Configuration.Configuration GetConfiguration();

        void Run();

        void Run(bool forceRun);
    }
}