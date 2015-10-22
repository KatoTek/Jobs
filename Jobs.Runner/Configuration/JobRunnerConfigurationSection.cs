using System.Configuration;

namespace Jobs.Runner.Configuration
{
    public class JobRunnerConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("pluginpaths")]
        public PluginPaths PluginPaths => this["pluginpaths"] as PluginPaths;

        public static JobRunnerConfigurationSection GetSection(string section) => ConfigurationManager.GetSection(section) as JobRunnerConfigurationSection;
    }
}
