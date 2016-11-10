using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Types;
using BZFlag.Map.Elements.Shapes;
using BZFlag.IO.BZW;

namespace BZFlag.IO.BZW.Parsers
{
	public abstract class PositionableObjectParser : BasicObjectParser
	{
        public override bool AddCodeLine(string command, string line)
		{
            PositionableObject p = Object as PositionableObject;
            if (p == null)
                return base.AddCodeLine(command, line);

            if (!base.AddCodeLine(command, line))
			{
				if(command == "POSITION")
					p.Position = Utilities.ReadVector3F(Reader.GetRestOfWords(line));
				else if(command == "SIZE")
                    p.Size = Utilities.ReadVector3F(Reader.GetRestOfWords(line));
				else if(command == "ROTATION")
					float.TryParse(Reader.GetRestOfWords(line), out p.Rotation);
				else
                    p.Attributes.Add(line);
			}

			return true;
		}

		public override void Finish()
		{

		}

		public override string BuildCode()
		{
            PositionableObject p = Object as PositionableObject;
            if (p == null)
                return base.BuildCode();

            Code.Clear();
			if(p.Name != string.Empty)
				AddCode(1, "name", p.Name);

			AddCode(1, "position", p.Position);
			AddCode(1, "rotation", p.Rotation);
			AddCode(1, "size", p.Size);

            return p.ObjectType;
		}
	}
}
