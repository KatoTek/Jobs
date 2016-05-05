using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using static System.Configuration.Install.ManagedInstallerClass;
using static System.Reflection.Assembly;
using static System.ServiceProcess.ServiceBase;
using static System.Diagnostics.EventLog;
using static Jobs.Service.Configuration.JobsServiceConfigurationSection;

namespace Jobs.Service
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
            if (args.Any())
            {
                var argsArray = (from arg in args
                                 select arg.Trim()
                                           .ToLowerInvariant()).ToArray();
                var location = GetExecutingAssembly()
                    .Location;
                if (argsArray.Contains(INSTALL))
                {
                    var jobsServiceConfig = GetSection(Service.ServiceSection);
                    var eventLog = new EventLog { Source = jobsServiceConfig.Log.Source, Log = jobsServiceConfig.Log.Name };
                    if (!SourceExists(eventLog.Source))
                        CreateEventSource(eventLog.Source, eventLog.Log);

                    eventLog.WriteEntry("Jobs.Service Log Created");

                    InstallHelper(new[] { INSTALL, location });
                }
                else if (argsArray.Contains(UNINSTALL))
                    InstallHelper(new[] { UNINSTALL, location });
            }
            else
                Run(new ServiceBase[] { new Service() });
        }

        #endregion
    }
}
