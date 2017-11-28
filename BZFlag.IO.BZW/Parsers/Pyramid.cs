using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Map.Elements.Shapes;

namespace BZFlag.IO.BZW.Parsers
{
    public class PyramidParser : PhaseableObjectParser
    {
        public PyramidParser()
        {
            Object = new Pyramid();
        }

        public PyramidParser(Pyramid obj)
        {
            Object = obj;
        }

        public override bool AddCodeLine(string command, string line)
        {
            Pyramid p = Object as Pyramid;
            if (p == null)
                return base.AddCodeLine(command, line);

            if (command == "FLIPZ")
                p.FlipZ = true;
            else if (!base.AddCodeLine(command, line))
                return false;

            return true;
        }

        public override void Finish()
        {
            base.Finish();

            Pyramid p = Object as Pyramid;

            if ((p != null) && (p.Size[0] < 0 | p.Size[1] < 0 || p.Size[2] < 0) && !p.FlipZ)
            {
                p.FlipZ = true;
                p.Size[0] = (float)Math.Abs(p.Size[0]);
                p.Size[1] = (float)Math.Abs(p.Size[1]);
                p.Size[2] = (float)Math.Abs(p.Size[2]);
            }
        }

        public override string BuildCode()
        {
            Pyramid p = Object as Pyramid;
            if (p == null)
                return base.BuildCode();

            string name = base.BuildCode();

            if (p.FlipZ)
                AddCode(1, "flipz");

            return name;
        }
    }
}
