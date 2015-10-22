using System;
using System.Runtime.Serialization;

namespace Jobs.Runner.Configuration.Exceptions
{
    public class ConfigurationSectionMissingException : Exception
    {
        private const string DEFAULT_MESSAGE_FORMAT = "Configuration section \"{0}\" is either missing or corrupt";

        public ConfigurationSectionMissingException(string section)
            : base(string.Format(DEFAULT_MESSAGE_FORMAT, section))
        {
            Section = section;
        }

        public ConfigurationSectionMissingException(string section, string alternateMessage)
            : base(alternateMessage)
        {
            Section = section;
        }

        public ConfigurationSectionMissingException(string section, Exception innerException)
            : base(string.Format(DEFAULT_MESSAGE_FORMAT, section), innerException)
        {
            Section = section;
        }

        public ConfigurationSectionMissingException(string section, string alternateMessage, Exception innerException)
            : base(alternateMessage, innerException)
        {
            Section = section;
        }

        public ConfigurationSectionMissingException(string section, SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Section = section;
        }

        public string Section { get; set; }
    }
}
