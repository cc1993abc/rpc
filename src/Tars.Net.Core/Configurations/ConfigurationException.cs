using System;
using System.Collections.Generic;
using System.Text;

namespace Tars.Net.Configurations
{
    public class ConfigurationException : Exception
    {
        public ConfigurationException(Exception innerException) : base("error occurred on load server config", innerException)
        {

        }
        public ConfigurationException(string key, string value) : base($"contains invalid config|key={key}, value={value}")
        {
        }
    }
}
