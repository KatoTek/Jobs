using System.Linq;
using System.ServiceProcess;
using static System.Configuration.Install.ManagedInstallerClass;
using static System.Reflection.Assembly;
using static System.ServiceProcess.ServiceBase;

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
                    InstallHelper(new[] { INSTALL, location });
                else if (argsArray.Contains(UNINSTALL))
                    InstallHelper(new[] { UNINSTALL, location });
            }
            else
                Run(new ServiceBase[] { new Service() });
        }

        #endregion
    }
}
