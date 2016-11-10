using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Map.Elements
{
	public class WaterLevel : BasicObject
	{
		public float Height = 0;
		public List<string> Attributes = new List<string>();

		public WaterLevel()
		{
			ObjectType = "WaterLevel";
		}
	}
}
