using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking
{
    public static class Protocol
    {
        public static readonly string BZFSHailString = "BZFLAG\r\n\r\n";
        public static readonly byte[] BZFSHail = System.Text.Encoding.ASCII.GetBytes(BZFSHailString);

        public static readonly string DefaultBZFSVersionString = "BZFS0221";
        public static readonly byte[] DefaultBZFSVersion = System.Text.Encoding.ASCII.GetBytes(DefaultBZFSVersionString);
    }
}