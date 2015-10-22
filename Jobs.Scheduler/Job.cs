using System;
using System.Collections.Generic;

namespace Jobs.Scheduler
{
    public class Job
    {
        public int AlertAfterTries { get; set; }
        public string AppId { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public int JobId { get; set; }
        public IList<JobInstance> JobInstances { get; set; } = new List<JobInstance>();
        public string JobName { get; set; }
        public IList<JobSchedule> JobSchedules { get; set; } = new List<JobSchedule>();
        public DateTime LastRun { get; set; }
    }
}
