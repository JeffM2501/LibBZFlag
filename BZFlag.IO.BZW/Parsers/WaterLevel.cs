using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Map.Elements;

namespace BZFlag.IO.BZW.Parsers
{
    public class WaterLevelParser : BasicObjectParser
    {

        public WaterLevelParser()
        {
            Object = new WaterLevel();
        }

        public WaterLevelParser(WaterLevel obj)
        {
            Object = obj;
        }

        public override bool AddCodeLine(string command, string line)
        {
            WaterLevel p = Object as WaterLevel;
            if (p == null)
                return base.AddCodeLine(command, line);

            if (command == "NAME")
                p.Name = Reader.GetRestOfWords(line);
            else if (command == "HEIGHT")
                float.TryParse(Reader.GetRestOfWords(line), out p.Height);
            else
                p.Attributes.Add(line);

            return true;
        }

        public override string BuildCode()
        {
            WaterLevel p = Object as WaterLevel;
            if (p == null)
                return base.BuildCode();

            Code.Clear();

            AddCode(1, "height", p.Height);
            foreach (var s in p.Attributes)
                AddCode(2, s);

            return p.ObjectType;
        }
    }
}
