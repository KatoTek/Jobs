using System;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;

namespace JobsService
{
    [RunInstaller(true)]
    public class JobsServiceInstaller : Installer
    {
        private readonly IContainer _Components = null;
        private ServiceInstaller _ServiceInstaller;
        private ServiceProcessInstaller _ServiceProcessInstaller;

        public JobsServiceInstaller()
        {
            this.InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this._Components != null))
                this._Components.Dispose();
            base.Dispose(disposing);
        }

        private string GetAppSetting(string key)
        {
            var config = ConfigurationManager.OpenExeConfiguration(Assembly.GetAssembly(this.GetType()).Location);
            return config.AppSettings.Settings[key].Value;
        }

        private void InitializeComponent()
        {
            this._ServiceProcessInstaller = new ServiceProcessInstaller();
            this._ServiceInstaller = new ServiceInstaller();
     
            this._ServiceProcessInstaller.Password = null;
            this._ServiceProcessInstaller.Username = null;
         
            this._ServiceInstaller.Description = this.GetAppSetting("Description");
            this._ServiceInstaller.DisplayName = this.GetAppSetting("DisplayName");
            this._ServiceInstaller.ServiceName = this.GetAppSetting("ServiceName");
            this._ServiceInstaller.StartType = ServiceStartMode.Automatic;

            this.Installers.AddRange(new Installer[]
                                     {
                                         this._ServiceProcessInstaller,
                                         this._ServiceInstaller
                                     });
        }
    }
}