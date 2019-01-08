using System;
using System.Collections.Generic;
using System.Text;

namespace Tars.Net.Configurations
{
    public class ConfigurationManager
    {
        public ServerConfig ServerConfig { set; get; }
        public HostConfig HostConfig { set; get; }
        public ClientConfig ClientConfig { set; get; }

        public ConfigurationManager() : this(Environment.GetEnvironmentVariable("config")) { }
        public ConfigurationManager(string configPath)
        {
            LoadConfig(configPath);
        }

        private void LoadConfig(string configPath)
        {
            string key = null, value = null;
            ServerConfig cfg = null;
            try
            {
                cfg = new ServerConfig().Load(Config.ParseFile(configPath));
            }
            catch (Exception ex)
            {
                throw new ConfigurationException(ex);
            }
            key = "app";
            value = cfg.Application;
            CheckValue(key, value);
            key = "server";
            value = cfg.ServerName;
            CheckValue(key, value);
            key = "localip";
            value = cfg.LocalIP;
            CheckValue(key, value);
            key = "logpath";
            value = cfg.LogPath;
            CheckValue(key, value);
            key = "logLevel";
            value = cfg.LogLevel;
            CheckValue(key, value);
            key = "datapath";
            value = cfg.DataPath;
            CheckValue(key, value);
            this.ServerConfig = cfg;
            this.ClientConfig = cfg.ClientConfig;
            this.HostConfig = cfg.HostConfig;
        }
        private void CheckValue(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ConfigurationException(key, value);
        }
    }
}
