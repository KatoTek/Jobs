using System;

namespace Jobs.Runner
{
    public class JobExceptionThrownEventArguments
    {
        public Exception Exception { get; set; }
        public IJob Job { get; set; }
        public bool RunnerIgnoresExceptions { get; set; }
    }
}
