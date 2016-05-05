﻿using System;

namespace Jobs.Runner
{
    public interface IJob : IDisposable
    {
        #region events

        event JobExceptionThrownEventHandler ExceptionThrown;
        event Action<string> Log;

        #endregion

        #region methods

        System.Configuration.Configuration GetConfiguration();
        void Run();
        void Run(bool forceRun);

        #endregion
    }
}
