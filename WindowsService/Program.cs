using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using Jobs.WindowsService.Configuration;

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
            if (args.Any())
            {
                var argsArray = (from arg in args
                                 select arg.Trim()
                                           .ToLowerInvariant()).ToArray();
                var location = Assembly.GetExecutingAssembly()
                    .Location;
                if (argsArray.Contains(INSTALL))
                {
                    var jobsServiceConfig = JobsServiceConfigurationSection.GetSection(Service.ServiceSection);
                    var eventLog = new EventLog { Source = jobsServiceConfig.Log.Source, Log = jobsServiceConfig.Log.Name };
                    if (!EventLog.SourceExists(eventLog.Source))
                        EventLog.CreateEventSource(eventLog.Source, eventLog.Log);

                    eventLog.WriteEntry("Jobs.Service Log Created");

                    ManagedInstallerClass.InstallHelper(new[] { INSTALL, location });
                }
                else if (argsArray.Contains(UNINSTALL))
                    ManagedInstallerClass.InstallHelper(new[] { UNINSTALL, location });
            }
            else
                ServiceBase.Run(new ServiceBase[] { new Service() });
        }

        #endregion
    }
}
