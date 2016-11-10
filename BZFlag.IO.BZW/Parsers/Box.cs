using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Map.Elements.Shapes;

namespace BZFlag.IO.BZW.Parsers
{
	public class BoxParser : PhaseableObjectParser
    {
        public BoxParser()
        {
            Object = new Box();
        }

        public BoxParser(Box obj)
        {
            Object = obj;
        }
	}
}
