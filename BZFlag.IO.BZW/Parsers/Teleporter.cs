using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Map.Elements.Shapes;

namespace BZFlag.IO.BZW.Parsers
{
	public class TeleporterParser : PositionableObjectParser
	{
        public TeleporterParser()
        {
            Object = new Teleporter();
        }

        public TeleporterParser(Teleporter obj)
        {
            Object = obj;
        }

        public override void Init(string objectType, string typeParams)
        {
            Teleporter p = Object as Teleporter;
            if (p == null)
                base.Init(objectType, typeParams);
            else
            {
                p.ObjectType = objectType;

                if (typeParams != string.Empty)
                    p.Name = typeParams;
            }
        }

        public override bool AddCodeLine(string command, string line)
		{
            Teleporter p = Object as Teleporter;
            if (p == null)
                return base.AddCodeLine(command, line);

            if (command == "BORDER")
                float.TryParse(Reader.GetRestOfWords(line), out p.Border);
            else if (command == "NAME" && p.Name != string.Empty)
                p.Name = Reader.GetRestOfWords(line);
            else if (command == "HORIZONTAL")
                p.Horizontal = true;
            else if (command == "RICOCHET")
                p.Ricochet = true;
            else if (!base.AddCodeLine(command, line))
                return false;

			return true;
		}

		public override string BuildCode()
		{
            Teleporter p = Object as Teleporter;
            if (p == null)
                base.BuildCode();

            Code.Clear();
    
            AddCode(1, "position", p.Position);
            AddCode(1, "rotation", p.Rotation);
            AddCode(1, "size", p.Size);

            AddCode(1, "border", p.Border);
            if (p.Horizontal)
                AddCode(1, "horizontal");
            if (p.Ricochet)
                AddCode(1, "ricochet");

            string t = "teleporter";
            if (p.Name != string.Empty)
                t += " " + p.Name;

			return t;
		}
	}
}
