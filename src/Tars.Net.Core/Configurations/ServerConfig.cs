using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tars.Net.Configurations
{
    public class ServerConfig
    {
        public string Node { set; get; }
        public string Application { set; get; }
        public string ServerName { set; get; }
        public string Log { set; get; }
        public string LogPath { set; get; }
        public string LogLevel { set; get; }
        public int LogRate { set; get; }
        public Endpoint Local { set; get; }
        public string BasePath { set; get; }
        public string Config { set; get; }
        public string Notify { set; get; }
        public string DataPath { set; get; }
        public string LocalIP { set; get; }
        public Encoding Charset { set; get; }
        public int UdpBufferSize { get; set; }
        public bool TcpNoDelay { get; set; }
        public Dictionary<string, ServantAdapterConfig> ServantAdapters { set; get; }
        //宿主容器信息
        public HostConfig HostConfig { set; get; }
        //tas组件与平台rpc配置
        public ClientConfig ClientConfig { set; get; }


        public ServerConfig Load(Config conf)
        {
            Application = conf.Get("/tars/application/server<app>", "UNKNOWN");
            ServerName = conf.Get("/tars/application/server<server>", null);

            string localStr = conf.Get("/tars/application/server<local>");
            Local = localStr == null || localStr.Length <= 0 ? null : Endpoint
                    .Parse(localStr);
            Node = conf.Get("/tars/application/server<node>");
            BasePath = conf.Get("/tars/application/server<basepath>");
            DataPath = conf.Get("/tars/application/server<datapath>");

            string charsetName = conf.Get("/tars/application/server<charsetname>", Constants.default_charset_name);
            Charset = Encoding.GetEncoding(charsetName);
            Config = conf.Get("/tars/application/server<config>");
            Notify = conf.Get("/tars/application/server<notify>");

            Log = conf.Get("/tars/application/server<log>");
            LogPath = conf.Get("/tars/application/server<logpath>", null);
            LogLevel = conf.Get("/tars/application/server<loglevel>");
            LogRate = conf.GetInt("/tars/application/server<lograte>", 5);

            LocalIP = conf.Get("/tars/application/server<localip>");
            UdpBufferSize = conf.GetInt("/tars/application/server<udpbuffersize>",
        4096);
            TcpNoDelay = conf.GetBool("/tars/application/server<tcpnodelay>", false);
            ServantAdapters = new Dictionary<string, ServantAdapterConfig>();
            List<String> adapterNameList = conf
                    .GetSubTags("/tars/application/server");
            //宿主业务服务
            if (adapterNameList != null)
            {
                foreach (string adapterName in adapterNameList)
                {
                    ServantAdapterConfig config = new ServantAdapterConfig();
                    config.Load(conf, adapterName);
                    ServantAdapters.Add(config.Servant, config);
                }
            }
            //主控
            ServantAdapterConfig adminServantAdapterConfig = new ServantAdapterConfig
            {
                Endpoint = Local,
                Servant = $"{Application}.{ServerName}.{Constants.AdminServant}",
                TcpNoDelay = TcpNoDelay,

            };
            ServantAdapters.Add(Constants.AdminServant, adminServantAdapterConfig);

            if (Application != null && ServerName != null && LogPath != null)
            {
                LogPath = Path.Combine(LogPath, Application, ServerName);
            }
            ClientConfig = new ClientConfig().Load(conf);
            if (LogPath != null)
            {
                ClientConfig.LogPath = LogPath;
            }
            ClientConfig.LogLevel = LogLevel;
            ClientConfig.DataPath = DataPath;
            HostConfig = new HostConfig().Load(conf);
            return this;
        }

    }
}
