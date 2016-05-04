using System;
using System.Collections.Generic;

namespace Jobs.Scheduler
{
    public class JobInstance
    {
        #region properties

        public DateTime? End { get; set; }
        public Job Job { get; set; }
        public IList<JobException> JobExceptions { get; set; } = new List<JobException>();
        public int JobId { get; set; }
        public long JobInstanceId { get; set; }
        public DateTime Start { get; set; }

        #endregion
    }
}
