using System.Diagnostics;
using static System.Diagnostics.EventLog;
using static Jobs.Service.Configuration.JobsServiceConfigurationSection;

namespace Jobs.EventLogCreator
{
    class Program
    {
        #region fields

        internal static string ServiceSection = "jobsservice";

        #endregion

        #region methods

        static void Main()
        {
            var jobsServiceConfig = GetSection(ServiceSection);
            var eventLog = new EventLog { Source = jobsServiceConfig.Log.Source, Log = jobsServiceConfig.Log.Name };

            if (!SourceExists(eventLog.Source))
                CreateEventSource(eventLog.Source, eventLog.Log);

            eventLog.WriteEntry("JobServiceLog Created");
        }

        #endregion
    }
}
