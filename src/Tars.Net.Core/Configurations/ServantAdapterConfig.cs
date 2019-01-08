using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Tars.Net.Codecs;

namespace Tars.Net.Configurations
{
    public class ServantAdapterConfig
    {
        public Endpoint Endpoint { set; get; }
        public int MaxConnections { set; get; } = 128;
        public int QueueCap { set; get; } = 1024;
        public int QueueTimeout { set; get; } = 10000;
        public string Servant { set; get; }
        public string Protocol { set; get; } = "tars";
        public int Threads { set; get; } = 1;
        public string HandleGroup { set; get; }
        
        //可自定义模板配置
        public TimeSpan QuietPeriodTimeSpan { get; set; } = TimeSpan.FromSeconds(1);
        public TimeSpan ShutdownTimeoutTimeSpan { get; set; } = TimeSpan.FromSeconds(3);
        public bool TcpNoDelay { get; set; }
        public int MaxFrameLength { get; set; } = 5 * 1024 * 1024;
        public int LengthFieldLength { get; set; } = 4;

        public ServantAdapterConfig Load(Config conf, string adapterName)
        {
            string path = "/tars/application/server/" + adapterName;
            Endpoint = Endpoint.Parse(conf.Get(path + "<endpoint>"));
            HandleGroup = conf.Get(path + "<handlegroup>", null);
            Protocol = conf.Get(path + "<protocol>", "tars");
            MaxConnections = conf.GetInt(path + "<maxconns>", 128);
            QueueCap = conf.GetInt(path + "<queuecap>", 1024);
            QueueTimeout = conf.GetInt(path + "<queuetimeout>", 10000);
            Servant = conf.Get(path + "<servant>");
            Threads = conf.GetInt(path + "<threads>", 1);
            int quietPeriodSeconds = conf.GetInt(path + "<quietperiodseconds>", 1);
            QuietPeriodTimeSpan = TimeSpan.FromSeconds(quietPeriodSeconds);
            int shutdownTimeoutSeconds = conf.GetInt(path + "<shutdowntimeoutseconds>", 3);
            ShutdownTimeoutTimeSpan = TimeSpan.FromSeconds(shutdownTimeoutSeconds);
            TcpNoDelay = conf.GetBool("<tcpnodelay>", false);
            return this;
        }
    }
}
