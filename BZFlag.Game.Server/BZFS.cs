using BZFlag.Networking;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace BZFlag.Game.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.ShowDateTime = true;

            ServerConfig cfg = new ServerConfig();
            if (args.Length > 0)
            {
                cfg = ServerConfig.Read(args[0]);
                Logger.Log1("Loading config from " + args[0]);

            }
            else
            {
                Logger.Log0("No config, using default");
                if (true)
                {
                    string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config.xml");
                    ServerConfig.Write(cfg, path);
                    Logger.Log2("Writing config to " + path);
                }
            }

            new Server(cfg).Run();
        }
    }
}
