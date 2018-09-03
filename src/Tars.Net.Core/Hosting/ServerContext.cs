using System;
using System.Collections.Generic;
using System.Threading;

namespace Tars.Net.Hosting
{
    public class ServerContext
    {
        public static ServerContext Current
        {
            get
            {
                return current.Value;
            }
            set
            {
                current.Value = value;
            }
        }

        private static readonly AsyncLocal<ServerContext> current = new AsyncLocal<ServerContext>();

        public IDictionary<string, string> Context { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }
}