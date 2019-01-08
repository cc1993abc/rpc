using System;
using System.Collections.Generic;
using System.Text;

namespace Tars.Net.Configurations
{
    public class Constants
    { 
        public const int default_max_sample_count = 200;
        public const int default_sample_rate = 1000;

        public const string default_modulename = "tarsproxy";
        public const string default_stat = "tars.tarsstat.StatObj";

        public const int default_connections = 4;
        public const int default_connect_timeout = 3000;
        public const int default_sync_timeout = 3000;
        public const int default_async_timeout = 3000;

        public const int default_refresh_interval = 60 * 1000;
        public const int default_report_interval = 60 * 1000;

        public const int default_background_queuesize = 20000;

        public const string default_charset_name = "UTF-8";

        public const int default_queue_size = 20000;
        public static readonly int default_core_pool_size = Environment.ProcessorCount;
        public static readonly int default_max_pool_size = Environment.ProcessorCount * 2;
        public const int default_keep_alive_time = 120;

        public const int default_check_interval = 60 * 1000;
        public const int default_try_time_interval = 30;
        public const int default_min_timeout_invoke = 20;
        public const int default_frequence_fail_invoke = 50;
        public const float default_frequence_fail_radio = 0.5f;
         

        public const string AdminServant = "AdminObj";
    }
}
