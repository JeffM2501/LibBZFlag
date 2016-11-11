using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Map.Elements
{
	public class World : BasicObject
	{
		public float Size = 400;
		public float FlagHeight = 0;

		public bool NoWalls = false;
		public bool FreeCTFSpawns = false;

		public World()
		{
			ObjectType = "World";
			Name = "Untitled BZW";
		}
	}
}
