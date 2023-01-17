using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAudioTest.Asio
{
    public class AsioConfigOptions
    {
        public List<AsioDriver> Drivers { get; set; }
    }
    public class AsioDriver
    {
        public string Name { get; set; }
        public Dictionary<int, string> InputChannels { get; set; }
    }
}
