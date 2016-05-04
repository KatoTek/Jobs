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
        #region fields

        readonly IContainer _Components = null;
        ServiceInstaller _serviceInstaller;
        ServiceProcessInstaller _serviceProcessInstaller;

        #endregion

        #region constructors

        public JobsServiceInstaller()
        {
            InitializeComponent();
        }

        #endregion

        #region methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _Components?.Dispose();

            base.Dispose(disposing);
        }

        string GetAppSetting(string key) => OpenExeConfiguration(GetAssembly(GetType())
                                                                     .Location)
            .AppSettings.Settings[key].Value;

        void InitializeComponent()
        {
            _serviceProcessInstaller = new ServiceProcessInstaller();
            _serviceInstaller = new ServiceInstaller();

            _serviceProcessInstaller.Password = null;
            _serviceProcessInstaller.Username = null;

            _serviceInstaller.Description = GetAppSetting("Description");
            _serviceInstaller.DisplayName = GetAppSetting("DisplayName");
            _serviceInstaller.ServiceName = GetAppSetting("ServiceName");
            _serviceInstaller.StartType = Automatic;

            Installers.AddRange(new Installer[] { _serviceProcessInstaller, _serviceInstaller });
        }

        #endregion
    }
}
