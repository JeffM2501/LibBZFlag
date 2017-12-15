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

        void Shutdown();
    }

    internal static class PluginLoader
    {
        private static string PluginTypeName = typeof(PlugIn).Name;

        private static List<PlugIn> Plugins = new List<PlugIn>();

        public static void LoadFromAssembly(Assembly ass)
        {

            foreach (var t in ass.GetTypes())
            {
                var i = t.GetInterface(PluginTypeName);
                if (i != null)
                {
                    Plugins.Add(Activator.CreateInstance(t) as PlugIn);
                    Logger.Log3("Loaded plug-in " + t.Name + " from " + Path.GetFileName(ass.Location));
                }
            }
        }

        public static void Startup(Server servInstance)
        {
            Logger.Log2("Plug-ins Startup");
            foreach (var p in Plugins)
                p.Startup(servInstance);
        }

        public static void Shutdown()
        {
            Logger.Log2("Plug-ins Shutdown");
            foreach (var p in Plugins)
                p.Shutdown();
        }
    }
}
