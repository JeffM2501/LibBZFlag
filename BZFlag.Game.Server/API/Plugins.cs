using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace BZFlag.Game.Host.API
{
    public interface IPlugIn
    {
        string Name { get; }
        string Description { get; }

        void Startup(GameState state);

        void Shutdown(GameState state);
    }

    public class PlugIn : IPlugIn
    {
        public GameState State { get; set; } = null;

        public virtual string Name => string.Empty;

        public virtual string Description => string.Empty;

        public virtual void Shutdown(GameState state)
        {
        }

        public virtual void Startup(GameState state)
        {
        }
    }

    internal static class PluginLoader
    {
        private static string PluginTypeName = typeof(IPlugIn).Name;

        private class PluginInfo
        {
            public IPlugIn Module = null;
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
                    info.Module = Activator.CreateInstance(t) as IPlugIn;
                    Plugins.Add(info);
                    Logger.Log3("Loaded Class " + t.Name + " from " + Path.GetFileName(ass.Location));
                }
            }
        }

        public static void Startup(GameState state)
        {
            foreach (var p in Plugins)
            {
                if (!p.Loaded)
                {
                    PlugIn pl = p.Module as PlugIn;

                    if (pl != null)
                        pl.State = state;

                    p.Module.Startup(state);
                    p.Loaded = true;
                }
            }
               
        }

        public static void Shutdown(GameState state)
        {
            Logger.Log2("API Shutdown");
            foreach (var p in Plugins)
            {
                if (p.Loaded)
                {
                    p.Module.Shutdown(state);
                    p.Loaded = false;
                }
            }
        }
    }
}
