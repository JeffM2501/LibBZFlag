using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using BZFlag.Map;
using BZFlag.Map.Elements;
using BZFlag.Map.Elements.Shapes;

using BZFlag.IO.BZW.Parsers;

namespace BZFlag.IO.BZW
{
    public static class Writer
    {
        private static BasicObjectParser ParserFromObject(BasicObject obj)
        {
            BasicObjectParser parser = ParserFactory.Create(obj.ObjectType.ToUpperInvariant());
            parser.Object = obj;
            return parser;
        }

        public static bool WriteMap(StreamWriter outStream, WorldMap map)
        {
            WriteObject(outStream, map.WorldInfo);
            WriteObject(outStream, map.WorldOptions);

            foreach (var o in map.Objects)
                WriteObject(outStream, o);

            return true;
        }

        private static void WriteObject(StreamWriter outStream, BasicObject obj)
        {
            BasicObjectParser parser = ParserFromObject(obj);
            if (parser == null)
                return;

            string keyword = parser.BuildCode();

            if (parser.Code.Count == 0)
                return;

            outStream.WriteLine(keyword);
            foreach (var s in parser.Code)
                outStream.WriteLine(s);

            outStream.WriteLine(parser.ObjectTerminator);
            outStream.WriteLine();
        }
    }
}
