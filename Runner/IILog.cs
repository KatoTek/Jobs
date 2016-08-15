using System;

namespace Jobs.Runner
{
    public interface IILog
    {
        #region events

        event Action<string> Log;

        #endregion
    }
}