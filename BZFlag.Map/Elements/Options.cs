using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Map.Elements
{
	public class Options : BasicObject
	{
		public List<string> Attributes = new List<string>();
		public Options()
		{
			ObjectType = "Options";
		}
	}
}
