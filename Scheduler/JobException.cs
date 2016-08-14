using System;

namespace Jobs.Scheduler
{
    public class JobException
    {
        #region properties

        public int? ChildId { get; set; }
        public DateTime? Date { get; set; }
        public string HelpLink { get; set; }
        public JobException InnerJobException { get; set; }
        public int JobExceptionId { get; set; }
        public JobInstance JobInstance { get; set; }
        public long JobInstanceId { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string StackTrace { get; set; }
        public string Type { get; set; }

        #endregion
    }
}
