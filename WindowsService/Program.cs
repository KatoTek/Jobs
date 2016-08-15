using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using static System.Configuration.Install.ManagedInstallerClass;
using static System.Diagnostics.EventLog;
using static System.Reflection.Assembly;
using static System.ServiceProcess.ServiceBase;
using static System.StringComparer;
using static Jobs.WindowsService.Configuration.JobsServiceConfigurationSection;
using static Jobs.WindowsService.Service;

namespace Jobs.WindowsService
{
    static class Program
    {
        #region fields

        const string INSTALL = "/i";
        const string UNINSTALL = "/u";

        #endregion

        #region methods

        static void Main(string[] args)
        {
            var location = GetExecutingAssembly()
                .Location;

            if (args.Contains(INSTALL, InvariantCultureIgnoreCase))
            {
                var jobsServiceConfig = GetSection(ServiceSection);
                var eventLog = new EventLog { Source = jobsServiceConfig.Log.Source, Log = jobsServiceConfig.Log.Name };
                if (!SourceExists(eventLog.Source))
                    CreateEventSource(eventLog.Source, eventLog.Log);

                eventLog.WriteEntry("Jobs.Service Log Created");

                InstallHelper(new[] { INSTALL, location });
            }
            else if (args.Contains(UNINSTALL, InvariantCultureIgnoreCase))
                InstallHelper(new[] { UNINSTALL, location });
            else
                Run(new ServiceBase[] { new Service() });
        }

        #endregion
    }
}
