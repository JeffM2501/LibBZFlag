using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Types;

namespace BZFlag.Map.Elements.Shapes
{
	public class WallObstacle : PhaseableObject
	{
		public WallObstacle()
		{
			ObjectType = "wall";
		}
	}
}
