using System.Configuration;

namespace Jobs.Runner.Configuration
{
    [ConfigurationCollection(typeof(PluginPath))]
    public class PluginPaths : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement() => new PluginPath();
        protected override object GetElementKey(ConfigurationElement element) => ((PluginPath)element).Name;
    }
}
