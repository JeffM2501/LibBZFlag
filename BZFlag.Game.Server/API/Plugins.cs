using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace BZFlag.Game.Host.API
{
    public interface PlugIn
    {
        string Name { get; }
        string Description { get; }

        void Startup(Server serverInstance);

        void Shutdown(Server serverInstance);
    }

    internal static class PluginLoader
    {
        private static string PluginTypeName = typeof(PlugIn).Name;

        private class PluginInfo
        {
            public PlugIn Module = null;
            public bool Loaded = false;
            public bool IsPlugin = false;
        }

        private static List<PluginInfo> Plugins = new List<PluginInfo>();

        public static void LoadFromAssembly(Assembly ass, bool isPlugin)
        {
            foreach (var t in ass.GetTypes())
            {
                var i = t.GetInterface(PluginTypeName);
                if (i != null)
                {
                    PluginInfo info = new PluginInfo();
                    info.IsPlugin = isPlugin;
                    info.Module = Activator.CreateInstance(t) as PlugIn;
                    Plugins.Add(info);
                    Logger.Log3("Loaded plug-in " + t.Name + " from " + Path.GetFileName(ass.Location));
                }
            }
        }

        public static void Startup(Server servInstance)
        {
            Logger.Log2("Plug-ins Startup");
            foreach (var p in Plugins)
            {
                if (!p.Loaded)
                {
                    p.Module.Startup(servInstance);
                    p.Loaded = true;
                }
            }
               
        }

        public static void Shutdown(Server servInstance)
        {
            Logger.Log2("Plug-ins Shutdown");
            foreach (var p in Plugins)
            {
                if (p.Loaded)
                {
                    p.Module.Shutdown(servInstance);
                    p.Loaded = false;
                }
            }
        }
    }
}
