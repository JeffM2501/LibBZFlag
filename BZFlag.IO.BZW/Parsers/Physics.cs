using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Types;
using BZFlag.Map.Elements;

namespace BZFlag.IO.BZW.Parsers
{
    public class PhysicsParser : BasicObjectParser
    {
        public PhysicsParser()
        {
            Object = new Physics();
        }

        public PhysicsParser(Physics obj)
        {
            Object = obj;
        }

        public override bool AddCodeLine(string command, string line)
        {
            Physics p = Object as Physics;
            if (p == null)
                return base.AddCodeLine(command, line);

            if (!base.AddCodeLine(command, line))
            {
                if (command == "LINEAR")
                    p.Linear = Utilities.ReadVector3F(Reader.GetRestOfWords(line));
                else if (command == "ANGULAR")
                    p.Angular = Utilities.ReadVector3F(Reader.GetRestOfWords(line));
                else if (command == "SLIDE")
                    float.TryParse(Reader.GetRestOfWords(line), out p.Slide);
                else if (command == "DEATH")
                    p.Death = Reader.GetRestOfWords(line);
                else
                    p.Attributes.Add(line);
            }

            return true;
        }

        public override string BuildCode()
        {
            Physics p = Object as Physics;
            if (p == null)
                return base.BuildCode();

            Code.Clear();

            if (!p.Linear.IsZero())
                AddCode(1, "linear", p.Linear);

            if (!p.Angular.IsZero())
                AddCode(1, "angular", p.Angular);

            if (p.Slide > 0)
                AddCode(1, "slide", p.Slide);

            if (p.Death != string.Empty)
                AddCode(1, "death", p.Death);

            foreach (var s in p.Attributes)
                AddCode(2, s);

            return p.ObjectType;
        }
    }
}
