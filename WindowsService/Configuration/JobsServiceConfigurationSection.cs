using System.Configuration;
using static System.Int32;

namespace Jobs.WindowsService.Configuration
{
    public class JobsServiceConfigurationSection : ConfigurationSection
    {
        #region properties

        [ConfigurationProperty("description")]
        public string Description => this["description"] as string;

        [ConfigurationProperty("displayname")]
        public string DisplayName => this["displayname"] as string;

        [ConfigurationProperty("log")]
        public Log Log => this["log"] as Log;

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name => this["name"] as string;

        [ConfigurationProperty("secondsinterval", DefaultValue = "10")]
        public int SecondsInterval
        {
            get
            {
                int secondsInterval;
                return TryParse(this["secondsinterval"] as string, out secondsInterval)
                           ? secondsInterval
                           : 10;
            }
        }

        #endregion

        #region methods

        public static JobsServiceConfigurationSection GetSection(string section) => ConfigurationManager.GetSection(section) as JobsServiceConfigurationSection;

        #endregion
    }
}
