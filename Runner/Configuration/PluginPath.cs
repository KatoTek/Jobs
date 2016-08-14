using System.Configuration;

namespace Jobs.Runner.Configuration
{
    public class PluginPath : ConfigurationElement
    {
        #region properties

        [ConfigurationProperty("folderpath", IsRequired = true)]
        public string FolderPath => base["folderpath"] as string;

        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name => base["name"] as string;

        [ConfigurationProperty("searchpattern", IsRequired = false, DefaultValue = "*.dll")]
        public string SearchPattern => base["searchpattern"] as string;

        #endregion
    }
}
