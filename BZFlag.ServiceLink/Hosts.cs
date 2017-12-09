using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace BZFlag.Services
{
    public static class Hosts
    {
        public static readonly string DefaultListServer = "https://my.bzflag.org/db/";
        public static string ListServerURL = DefaultListServer;

        public static readonly string DefaultBZFSVersion = "BZFS0221";
        public static string BZFSVersion = DefaultBZFSVersion;

        private static string InternalVersion = string.Empty;

        public static string ApplicationVersion
        {
            get
            {
                if (InternalVersion == string.Empty)
                {
                    InternalVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                }

#if DEBUG
                string buildType = "Debug";
#else
                string buildType = "Release";
#endif
                return InternalVersion + "-libBZFlag_" + buildType + "_Managed";
            }
        }

    }
}
