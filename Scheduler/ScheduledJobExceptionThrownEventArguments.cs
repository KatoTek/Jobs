using Jobs.Runner;

namespace Jobs.Scheduler
{
    public sealed class ScheduledJobExceptionThrownEventArguments : JobExceptionThrownEventArguments
    {
        #region properties

        public int JobExceptionId { get; set; }

        #endregion
    }
}
