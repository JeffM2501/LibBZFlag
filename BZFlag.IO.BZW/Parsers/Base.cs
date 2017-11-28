using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Teams;

using BZFlag.Map.Elements.Shapes;

namespace BZFlag.IO.BZW.Parsers
{
    public class BaseParser : BoxParser
    {


        public BaseParser()
        {
            Object = new Base();
        }

        public BaseParser(Base obj)
        {
            Object = obj;
        }

        public override bool AddCodeLine(string command, string line)
        {
            Base p = Object as Base;
            if (p == null)
                return base.AddCodeLine(command, line);

            if (command == "COLOR")
            {
                int c = 0;
                int.TryParse(Reader.GetRestOfWords(line), out c);

                p.TeamColor = (TeamColors)c;
            }
            else if (!base.AddCodeLine(command, line))
                return false;

            return true;
        }

        public override string BuildCode()
        {
            Base p = Object as Base;
            if (p == null)
                return base.BuildCode();

            string name = base.BuildCode();

            AddCode(1, "color", (int)p.TeamColor);

            return name;
        }
    }
}
