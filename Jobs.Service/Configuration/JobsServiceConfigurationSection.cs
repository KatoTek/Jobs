using System.Configuration;

namespace Jobs.Service.Configuration
{
    public class JobsServiceConfigurationSection : ConfigurationSection
    {
        #region properties

        [ConfigurationProperty("log")]
        public Log Log => this["log"] as Log;

        #endregion

        #region methods

        public static JobsServiceConfigurationSection GetSection(string section) => ConfigurationManager.GetSection(section) as JobsServiceConfigurationSection;

        #endregion
    }
}
