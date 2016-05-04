using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;
using static System.Configuration.ConfigurationManager;
using static System.Reflection.Assembly;
using static System.ServiceProcess.ServiceStartMode;

namespace Jobs.Service
{
    [RunInstaller(true)]
    public class JobsServiceInstaller : Installer
    {
        #region constructors

        public JobsServiceInstaller()
        {
            Installers.AddRange(new Installer[]
                                {
                                    new ServiceProcessInstaller { Password = null, Username = null },
                                    new ServiceInstaller
                                    {
                                        Description = GetAppSetting("Description"),
                                        DisplayName = GetAppSetting("DisplayName"),
                                        ServiceName = GetAppSetting("ServiceName"),
                                        StartType = Automatic
                                    }
                                });
        }

        #endregion

        #region methods

        string GetAppSetting(string key) => OpenExeConfiguration(GetAssembly(GetType())
                                                                     .Location)
            .AppSettings.Settings[key].Value;

        #endregion
    }
}
