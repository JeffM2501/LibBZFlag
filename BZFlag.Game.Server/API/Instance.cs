using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BZFlag.Game.Host.API
{
    public class Instance
    {
        public event EventHandler APILoadComplete;
        public event EventHandler ModuleLoadComplete;

        GameState State = null;
        
        public Instance(GameState state)
        {
            State = state;
        }

        List<Assembly> ModuleAssemblies = new List<Assembly>();

        public void SetupAPI()
        {
            Logger.Log2("Load API");

            API.Common.ServerState = State;

            AppDomain.CurrentDomain.AssemblyResolve += ModuleAssemblyResolver;

            PluginLoader.LoadFromAssembly(Assembly.GetExecutingAssembly(), false);

            DirectoryInfo ModulesDir = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Modules"));
            if (ModulesDir.Exists)
            {
                foreach (var module in ModulesDir.GetFiles("*.dll"))
                {
                    try
                    {
                        var a = Assembly.LoadFile(module.FullName);
                        if (a != null)
                        {
                            ModuleAssemblies.Add(a);
                            PluginLoader.LoadFromAssembly(a, false);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Logger.Log1("Unable to load module " + module.Name + " :" + ex.ToString());
                    }
                }

                // load the build in modules first
                Logger.Log2("Modules Startup");
                PluginLoader.Startup(State);
                ModuleLoadComplete?.Invoke(this, EventArgs.Empty);
            }

            foreach (var f in State.ConfigData.PlugIns)
            {
                try
                {
                    DirectoryInfo PluginDir = new DirectoryInfo(Path.GetDirectoryName(f));
                    var a = Assembly.LoadFile(f);
                    if (a != null)
                    {
                        ModuleAssemblies.Add(a);
                        PluginLoader.LoadFromAssembly(a, true);
                    }
                }
                catch (System.Exception ex)
                {
                    Logger.Log1("Unable to load plug-in " + f + " :" + ex.ToString());
                }
            }
            Logger.Log2("Plug-ins Startup");
            PluginLoader.Startup(State);

            APILoadComplete?.Invoke(this, EventArgs.Empty);
        }

        private Assembly ModuleAssemblyResolver(object sender, ResolveEventArgs args)
        {
            return ModuleAssemblies.Find((x) => x.FullName == args.Name);
        }
    }
}
