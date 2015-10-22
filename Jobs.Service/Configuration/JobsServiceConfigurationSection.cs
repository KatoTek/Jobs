using System.Configuration;

namespace Jobs.Service.Configuration
{
    public class JobsServiceConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("log")]
        public Log Log => this["log"] as Log;

        public static JobsServiceConfigurationSection GetSection(string section) => ConfigurationManager.GetSection(section) as JobsServiceConfigurationSection;
    }
}
