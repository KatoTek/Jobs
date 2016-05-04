using System.Configuration;

namespace Jobs.Runner.Configuration
{
    public class JobRunnerConfigurationSection : ConfigurationSection
    {
        #region properties

        [ConfigurationProperty("pluginpaths")]
        public PluginPaths PluginPaths => this["pluginpaths"] as PluginPaths;

        #endregion

        #region methods

        public static JobRunnerConfigurationSection GetSection(string section) => ConfigurationManager.GetSection(section) as JobRunnerConfigurationSection;

        #endregion
    }
}
