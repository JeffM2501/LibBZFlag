using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Map.Elements
{
	public class Link : BasicObject
	{
        public class PorterLink
        {
            public bool Wildcard = false;

            public string TargetGroup = string.Empty;
            public string TargetName = string.Empty;
            public bool Front = false;
        }

        public PorterLink From = new PorterLink();
        public PorterLink To = new PorterLink();

        public Link()
		{
			ObjectType = "Link";
		}
	}
}
