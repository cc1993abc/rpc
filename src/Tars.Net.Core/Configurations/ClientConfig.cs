using System;
using System.Collections.Generic;
using System.Text;

namespace Tars.Net.Configurations
{
    public class ClientConfig
    {
        public string Locator { set; get; }
        public string Stat { set; get; }
        public string Property { set; get; }
        public string ModuleName { set; get; }
        public int RefreshEndpointInterval { set; get; }
        public int ReportInterval { set; get; }
        public int SyncInvokeTimeout { set; get; }
        public int AsyncInvokeTimeout { set; get; }

        public int Connections { get; set; }
        public int ConnectTimeout { get; set; }
        public int CorePoolSize { get; set; }
        public int MaxPoolSize { get; set; }
        public int KeepAliveTime { get; set; }
        public int QueueSize { get; set; }

        public Encoding Charset { set; get; }

        public string LogPath { set; get; }
        public string LogLevel { set; get; }
        public string DataPath { set; get; }
         
        public ClientConfig Load(Config conf)
        {
            Locator = conf.Get("/tars/application/client<locator>");
            LogPath = conf.Get("/tars/application/client<logpath>", null);
            LogLevel = conf.Get("/tars/application/client<loglevel>", "INFO");
            DataPath = conf.Get("/tars/application/client<cdatapath>", null);
            SyncInvokeTimeout = conf.GetInt("/tars/application/client<sync-invoke-timeout>", 3000);
            AsyncInvokeTimeout = conf.GetInt("/tars/application/client<async-invoke-timeout>", 3000);
            RefreshEndpointInterval = conf.GetInt("/tars/application/client<refresh-endpoint-interval>", 60000);
            Stat = conf.Get("/tars/application/client<stat>");
            Property = conf.Get("/tars/application/client<property>");
            ReportInterval = conf.GetInt("/tars/application/client<report-interval>", 60000);
            ModuleName = conf.Get("/tars/application/client<modulename>", Constants.default_modulename);
            Connections = conf.GetInt("/tars/application/client<connections>", Constants.default_connections);
            ConnectTimeout = conf.GetInt("/tars/application/client<connect-timeout>", Constants.default_connect_timeout);
            CorePoolSize = conf.GetInt("/tars/application/client<corepoolsize>", Constants.default_core_pool_size);
            MaxPoolSize = conf.GetInt("/tars/application/client<maxpoolsize>", Constants.default_max_pool_size);
            KeepAliveTime = conf.GetInt("/tars/application/client<keepalivetime>", Constants.default_keep_alive_time);
            QueueSize = conf.GetInt("/tars/application/client<queuesize>", Constants.default_queue_size);
            string charsetName = conf.Get("/tars/application/client<charsetname>", Constants.default_charset_name);
            Charset = Encoding.GetEncoding(charsetName);
            return this;
        }
        public override string ToString()
        {
            return $"CommunicatorConfig [locator={Locator}, syncInvokeTimeout={SyncInvokeTimeout}, asyncInvokeTimeout={AsyncInvokeTimeout}, refreshEndpointInterval={RefreshEndpointInterval}, reportInterval={ReportInterval}, stat={Stat}, property={Property}, moduleName={ModuleName}, connections={Connections}, connectTimeout={ConnectTimeout}, corePoolSize={CorePoolSize}, maxPoolSize={MaxPoolSize}, keepAliveTime={KeepAliveTime}, queueSize={QueueSize}, charsetName={Charset.BodyName}, logPath={LogPath}, logLevel={LogLevel}, dataPath={DataPath}]";
        }
    }
}
