using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Map.Elements.Shapes
{
	public class Pyramid : PhaseableObject
    {
		public bool FlipZ = false;

		public Pyramid()
		{
			ObjectType = "Pyramid";
		}
	}
}
