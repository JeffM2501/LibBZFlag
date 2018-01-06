
using System;
using System.IO;
using System.Reflection;

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
                string ext = Path.GetExtension(args[0]).ToUpper();
                if (ext == ".XML")
                    cfg = ServerConfig.ReadXML(args[0]);
                else if (ext == ".JSON")
                    cfg = ServerConfig.ReadJSON(args[0]);
                else if (ext == ".YAML")
                    cfg = ServerConfig.ReadYAML(args[0]);

                Logger.Log1("Loading config from " + args[0]);

            }
            else
            {
                Logger.Log0("No config, using default");
            }

            new Server(cfg).Run();
        }
    }
}
