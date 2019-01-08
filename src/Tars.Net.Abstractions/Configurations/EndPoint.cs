using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Tars.Net.Configurations
{
    public class Endpoint
    {
        public string Host { set; get; }
        public int Port { set; get; }
        public int Timeout { set; get; }
        //网略协议类型
        public string Proto { set; get; }
        public string Bind { set; get; }
        public string Container { set; get; }
        public string SetDivision { set; get; }

        public Endpoint(string proto, string host, int port, int timeout, string bind, string container, string setDivision)
        {
            Proto = proto;
            Host = host;
            Port = port;
            Timeout = timeout;
            Bind = bind;
            Container = container;
            SetDivision = setDivision;
        }

        public static Endpoint Parse(string local)
        {
            string proto = null, host = null, bind = null, container = null, setDivision = null;
            int port = 0, timeout = 3000;
            string[] keys = local.Split(' ');
            for (int i = 0; i < keys.Length; i++)
            {
                if (i == 0)
                    proto = keys[i];
                if (keys[i] == "-h")
                    host = keys[++i];
                if (keys[i] == "-p")
                    port = Convert.ToInt32(keys[++i]);
                if (keys[i] == "-t")
                    timeout = Convert.ToInt32(keys[++i]);
                if (keys[i] == "-b")
                    bind = keys[++i];
                if (keys[i] == "-c")
                    container = keys[++i];
                if (keys[i] == "-s")
                    setDivision = keys[++i];
            }
            return new Endpoint(proto, host, port, timeout, bind, container, setDivision);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Proto.ToLower());
            sb.Append(" -h ").Append(Host);
            sb.Append(" -p ").Append(Port);
            if (Timeout > 0)
            {
                sb.Append(" -t ").Append(Timeout);
            }
            if (string.IsNullOrWhiteSpace(Bind))
            {
                sb.Append(" -b ").Append(Bind);
            }
            if (string.IsNullOrWhiteSpace(Container))
            {
                sb.Append(" -c ").Append(Container);
            }
            if (string.IsNullOrWhiteSpace(SetDivision))
            {
                sb.Append(" -s ").Append(SetDivision);
            }
            return sb.ToString();
        }
    }
}
