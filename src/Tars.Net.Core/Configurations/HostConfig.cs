using System;
using System.Collections.Generic;
using System.Text;

namespace Tars.Net.Configurations
{
    public class HostConfig
    {
        public string Container { set; get; }
        public bool IsDocker { set; get; }
        public bool EnableSet { set; get; }
        public string SetDivision { set; get; }

        public HostConfig Load(Config conf)
        {
            string enableSetStr = conf.Get("/tars/application<enableset>");
            if ("Y".Equals(enableSetStr, StringComparison.OrdinalIgnoreCase))
                EnableSet = true;
            SetDivision = conf.Get("/tars/application<setdivision>");
            string isDockerStr = conf.Get("/tars/application<isdocker>");
            if ("Y".Equals(isDockerStr, StringComparison.OrdinalIgnoreCase))
                IsDocker = true;
            Container = conf.Get("/tars/application<container>");
            return this;
        }
    }
}
