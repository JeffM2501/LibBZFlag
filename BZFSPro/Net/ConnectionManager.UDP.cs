using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BZFSPro.Net
{
    internal partial class ConnectionManager
    {
        protected UdpClient UDPSocketV4 = null;
        protected UdpClient UDPSocketV6 = null;
    }
}
