using BZFlag.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Game.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerConfig cfg = new ServerConfig();
            if (args.Length > 0)
                cfg = ServerConfig.Read(args[0]);

            new Server(cfg).Run();
        }
    }
}
