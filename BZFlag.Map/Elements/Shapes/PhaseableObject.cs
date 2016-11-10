using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Map.Elements.Shapes
{
	public abstract class PhaseableObject : PositionableObject
    {
        public bool Passable = false;
        public bool ShootThrough = false;
        public bool DriveThrough = false;
        public bool Ricochet = false;
	}
}
