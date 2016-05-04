using System.Collections.Generic;

namespace Jobs.Scheduler
{
    public class JobScheduleType
    {
        #region properties

        public IList<JobSchedule> JobSchedules { get; set; } = new List<JobSchedule>();
        public int JobScheduleTypeId { get; set; }
        public string Type { get; set; }

        #endregion
    }
}
