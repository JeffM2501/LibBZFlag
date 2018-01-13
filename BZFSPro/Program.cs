using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

using BZFlag.Game.Host;

namespace BZFSPro
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.ShowDateTime = true;

            ServerConfig cfg = new ServerConfig();
            if (args.Length > 0)
            {
                if (args.Length > 1)
                {
                    if (args[0].ToLowerInvariant() == "-save_conf")
                    {
                        Logger.Log0("Saving config template to " + args[1]);

                        cfg = BZFS.BuildSaveableConfig();
                        string ext = Path.GetExtension(args[1]).ToUpper();
                        if (ext == ".XML")
                            ServerConfig.WriteXML(cfg, args[1]);
                        else if (ext == ".JSON")
                            ServerConfig.WriteJSON(cfg, args[1]);
                        else if (ext == ".YAML")
                            ServerConfig.WriteYAML(cfg, args[1]);
                    }
                    else
                        BZFS.Useage();

                    return;
                }
                else
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
            }
            else
            {
                Logger.Log0("No config, using default");
            }

            new Server.Instance().Run(cfg);
        }
    }
}
