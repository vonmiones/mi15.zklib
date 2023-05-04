using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mi15libraries
{
    public class SystemConfig
    {
        public ClockConfig clock { get; set; }
        public DeviceConfig device { get; set; }
        public LogConfig log { get; set; }
        public ServerConfig server { get; set; }
    }
    public class ClockConfig
    {
        public string sync { get; set; }
        public int interval { get; set; }
        public string timezone { get; set; }
    }

    public class DeviceConfig
    {
        public int count { get; set; }
        public bool enablelog { get; set; }
    }

    public class LogConfig
    {
        public string extension { get; set; }
        public string format { get; set; }
        public string path { get; set; }
        public bool separtate { get; set; }
        public string type { get; set; }
    }

    public class ServerConfig
    {
        public int port { get; set; }
    }

}
