using System.Configuration;

namespace Jobs.Service.Configuration
{
    public class Log : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name => base["name"] as string;

        [ConfigurationProperty("source", IsRequired = true)]
        public string Source => base["source"] as string;
    }
}
