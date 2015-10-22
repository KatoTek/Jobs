using Jobs.Runner;

namespace Jobs.Scheduler
{
    public sealed class ScheduledJobExceptionThrownEventArguments : JobExceptionThrownEventArguments
    {
        public int JobExceptionId
        {
            get;
            set;
        }
    }
}