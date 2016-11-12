using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Map.Elements
{
	public class WaterLevel : BasicObject
	{
		public float Height = -1;
		public int MaterialID = -1;

		public WaterLevel()
		{
			ObjectType = "WaterLevel";
		}
	}
}
