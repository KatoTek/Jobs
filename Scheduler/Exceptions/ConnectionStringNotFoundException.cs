using System;

namespace Jobs.Scheduler.Exceptions
{
    public class ConnectionStringNotFoundException : Exception
    {
        #region constructors

        public ConnectionStringNotFoundException()
            : base("JobSchedulerConnection ConnectionString was not found in the configuration file.") {}

        public ConnectionStringNotFoundException(Exception innerException)
            : base("JobSchedulerConnection ConnectionString was not found in the configuration file.", innerException) {}

        #endregion
    }
}
