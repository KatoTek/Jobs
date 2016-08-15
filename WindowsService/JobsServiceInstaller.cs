using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;
using static System.ServiceProcess.ServiceStartMode;
using static Jobs.WindowsService.Configuration.JobsServiceConfigurationSection;
using static Jobs.WindowsService.Service;

namespace Jobs.WindowsService
{
    [RunInstaller(true)]
    public class JobsServiceInstaller : Installer
    {
        #region constructors

        public JobsServiceInstaller()
        {
            var section = GetSection(ServiceSection);
            Installers.AddRange(new Installer[]
                                {
                                    new ServiceProcessInstaller { Password = null, Username = null },
                                    new ServiceInstaller
                                    {
                                        Description = section.Description,
                                        DisplayName = section.DisplayName,
                                        ServiceName = section.Name,
                                        StartType = Automatic,
                                        DelayedAutoStart = true
                                    }
                                });
        }

        #endregion
    }
}
