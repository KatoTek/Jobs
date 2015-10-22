using System;
using System.Diagnostics;
using System.Linq;
using Jobs.Service.Configuration;

namespace JobServiceEventLogCreator
{
    internal class Program
    {
        internal static string ServiceSection = "jobsservice";

        private static void Main(string[] args)
        { 
            var jobsServiceConfig = JobsServiceConfigurationSection.GetSection(ServiceSection);
            var eventLog = new EventLog()
                           {
                               Source = jobsServiceConfig.Log.Source,
                               Log = jobsServiceConfig.Log.Name
                           };

            if (!EventLog.SourceExists(eventLog.Source)) 
                EventLog.CreateEventSource(eventLog.Source, eventLog.Log);

            eventLog.WriteEntry("JobServiceLog Created");
        }
    }
}