using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BZFlag.Map.Elements;
using BZFlag.Map.Elements.Shapes;

using BZFlag.IO.BZW.Parsers;

namespace BZFlag.IO.BZW
{
    public static class ParserFactory
    {
        private static Dictionary<string, Type> ObjectFactories = new Dictionary<string, Type>();

        public static void AddFactory(string name, Type type)
        {
            name = name.ToUpperInvariant();
            if (ObjectFactories.ContainsKey(name))
                ObjectFactories[name] = type;
            else
                ObjectFactories.Add(name, type);
        }

        static ParserFactory()
        {
            AddFactory("world", typeof(WorldParser));
            AddFactory("options", typeof(OptionsParser));
            AddFactory("box", typeof(BoxParser));
            AddFactory("pyramid", typeof(PyramidParser));
            AddFactory("base", typeof(BaseParser));
            AddFactory("teleporter", typeof(TeleporterParser));
            AddFactory("link", typeof(LinkParser));
            AddFactory("zone", typeof(ZoneParser));
            AddFactory("mesh", typeof(MeshParser));
            AddFactory("physics", typeof(PhysicsParser));
        }

        public static BasicObjectParser Create(string name)
        {
            string key = name.ToUpperInvariant();
            if (ObjectFactories.ContainsKey(key))
                return Activator.CreateInstance(ObjectFactories[key]) as BasicObjectParser;

            return new BasicObjectParser();
        }
    }
}
