using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Map.Elements;

namespace BZFlag.IO.BZW.Parsers
{
	public class WorldParser : BasicObjectParser
	{
        public WorldParser()
        {
            Object = new World();
        }

        public WorldParser(World obj)
        {
            Object = obj;
        }

        public override bool AddCodeLine(string command, string line)
		{
            World w = Object as World;
            if (w == null)
                return base.AddCodeLine(command, line);

            if (command == "NAME")
                w.Name = Reader.GetRestOfWords(line);
            else if (command == "SIZE")
                float.TryParse(Reader.GetRestOfWords(line), out w.Size);
            else if (command == "FLAGHEIGHT")
                float.TryParse(Reader.GetRestOfWords(line), out w.FlagHeight);
            else if (command == "NOWALLS")
                w.NoWalls = true;
            else if (command == "FREECTFSPAWNS")
                w.FreeCTFSpawns = true;
            else
                w.Attributes.Add(line);

			return true;
		}

		public override string BuildCode()
		{
			Code.Clear();

            World w = Object as World;
            if (w == null)
                return base.BuildCode();

            if (w.Name != string.Empty)
				AddCode(1, "name", w.Name);

			AddCode(1,"size", w.Size);
			AddCode(1, "flagHeight", w.FlagHeight);
			if (w.NoWalls)
				AddCode(1, "noWalls");

			if(w.FreeCTFSpawns)
				AddCode(1, "freeCTFSpawns");

			foreach(var s in  w.Attributes)
				AddCode(2, s);

            return w.ObjectType;
		}
	}
}
