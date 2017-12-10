using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Text;

namespace BZFlag.Game.Host
{
    public class ServerConfig
    {
        public int Port = 5154;


        // plugins
        public List<string> PlugIns = new List<string>();

        public string LogFile = string.Empty;
        public int LogLevel = 4;

        // external databases
        public string BanListFile = string.Empty;


        // public data
        public bool ListPublicly = false;
        public string PublicHost = string.Empty;
        public string PublicListKey = string.Empty;
        public List<string> PublicAdvertizeGroups = new List<string>();
        public string PublicTitle = string.Empty;

        // authentication data
        public bool AllowAnonUsers = false;

        public List<string> SecurityGroups = new List<string>();

        // world data
        public string MapFile = string.Empty;
        public string MapURL = "https://www.hyperdrive.tech/blank.bzc";


        public static ServerConfig Read(string filepath)
        {
            FileInfo f = new FileInfo(filepath);
            if (!f.Exists)
                return new ServerConfig();

            try
            {
                XmlSerializer xml = new XmlSerializer(typeof(ServerConfig));
                var fs = f.OpenRead();
                ServerConfig cfg = xml.Deserialize(fs) as ServerConfig;
                fs.Close();

                return cfg == null ? new ServerConfig() : cfg;
            }
            catch (Exception /*ex*/)
            {
                return new ServerConfig();
            }
        }

        public static bool Write(ServerConfig config, string filepath)
        {
            FileInfo f = new FileInfo(filepath);
            if (f.Exists)
                f.Delete();

            try
            {
                XmlSerializer xml = new XmlSerializer(typeof(ServerConfig));
                var fs = f.OpenWrite();
                xml.Serialize(fs, config);
                fs.Close();

                return true;
            }
            catch (Exception /*ex*/)
            {
                return false;
            }
        }
    }
}
