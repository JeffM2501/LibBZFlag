using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Map.Elements.Shapes;

namespace BZFlag.IO.BZW.Parsers
{
	public abstract class PhaseableObjectParser : PositionableObjectParser
    {
		public override bool AddCodeLine(string command, string line)
		{
            PhaseableObject p = Object as PhaseableObject;
            if (p == null)
                return base.AddCodeLine(command,line);

            if (!base.AddCodeLine(command, line))
			{
                if (command == "PASSABLE")
                    p.Passable = true;
                else if (command == "DRIVETHROUGH")
                    p.DriveThrough = true;
                else if (command == "SHOOTTHROUGH")
                    p.ShootThrough = true;
                else if (command == "RICOCHET")
                    p.Ricochet = true;
			}

			return true;
		}

        public override void Finish()
        {

        }

		public override string BuildCode()
		{
            PhaseableObject p = Object as PhaseableObject;
            if (p == null)
                return base.BuildCode();

            string name = base.BuildCode();

            if (p.Passable)
                AddCode(1, "passable");
            else
            {
                if (p.DriveThrough)
                    AddCode(1, "drivethrough");

                if (p.ShootThrough)
                    AddCode(1, "shootthrough");
            }

            if (p.Ricochet)
                AddCode(1, "ricochet");

            return name;
		}
	}
}
