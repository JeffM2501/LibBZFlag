using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Map.Elements;

namespace BZFlag.IO.BZW.Parsers
{
	public class OptionsParser : BasicObjectParser
	{
        public OptionsParser()
        {
            Object = new Options();
        }

        public OptionsParser(Options obj)
        {
            Object = obj;
        }

        public override bool AddCodeLine(string command, string line)
		{
            base.AddCodeLine(command, line);
			return true;
		}

		public override string BuildCode()
		{
            Options p = Object as Options;
            if (p == null)
                return base.BuildCode();

            Code.Clear();

			foreach(var s in p.Attributes)
				AddCode(1, s);

			return p.ObjectType;
		}
	}
}
