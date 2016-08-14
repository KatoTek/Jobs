using System;

namespace Jobs.Runner
{
    public class JobExceptionThrownEventArguments
    {
        #region properties

        public Exception Exception { get; set; }
        public IJob Job { get; set; }

        #endregion
    }
}
