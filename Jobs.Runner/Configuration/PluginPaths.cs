using System.Configuration;

namespace Jobs.Runner.Configuration
{
    [ConfigurationCollection(typeof(PluginPath))]
    public class PluginPaths : ConfigurationElementCollection
    {
        #region methods

        protected override ConfigurationElement CreateNewElement() => new PluginPath();
        protected override object GetElementKey(ConfigurationElement element) => ((PluginPath)element).Name;

        #endregion
    }
}
