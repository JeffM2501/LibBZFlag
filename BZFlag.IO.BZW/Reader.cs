using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using BZFlag.Map;

using BZFlag.IO.BZW.Parsers;

namespace BZFlag.IO.BZW
{
    public static class Reader
    {
		internal static string TrimTrainingComments(string text)
		{
			if (text.Contains("#"))
				text = text.Substring(0, text.IndexOf('#')).Trim();
			return text;
		}

		internal static string GetFirstWord(string text)
		{
			string tmp = TrimTrainingComments(text);
			if(tmp.Contains(" "))
				tmp = text.Substring(0, text.IndexOf(' ')).Trim();
			return tmp;
		}

		internal static string GetRestOfWords(string text)
		{
			string tmp = TrimTrainingComments(text);
            if (tmp.Contains(" "))
                tmp = text.Substring(text.IndexOf(' ')).Trim();
            else
                tmp = string.Empty;
			return tmp;
		}

		internal static List<float> ParseFloatVector(string line)
		{
			List<float> vec = new List<float>();
			foreach(string s in line.Split(" ".ToCharArray()))
			{
				if(s == string.Empty || Char.IsWhiteSpace(s[0]))
					continue;

				float d = 0;
				float.TryParse(s, out d);
				vec.Add(d);
			}
			return vec;
		}

        internal static List<int> ParseIntVector(string line)
        {
            List<int> vec = new List<int>();
            foreach (string s in line.Split(" ".ToCharArray()))
            {
                if (s == string.Empty || Char.IsWhiteSpace(s[0]))
                    continue;

                int i = 0;
                int.TryParse(s, out i);
                vec.Add(i);
            }
            return vec;
        }

        public static WorldMap ReadMap(StreamReader inStream)
		{
            WorldMap map = new WorldMap();
			map.IntForLoad();

			BasicObjectParser parser = null;
			while (!inStream.EndOfStream)
			{
				string line = inStream.ReadLine().Trim();

				if (line == string.Empty || line[0] == '#')
					continue;

                string cmd_norm = GetFirstWord(line);

                string cmd = cmd_norm.ToUpperInvariant();

				if (parser == null)
				{
                    parser = ParserFactory.Create(cmd);
                    parser.Init(cmd_norm, GetRestOfWords(TrimTrainingComments(line)));
				}
				else
				{
					if (cmd == "END")
					{
                        parser.Finish();
						map.AddObject(parser.Object);
                        parser = null;
					}
					else
                        parser.AddCodeLine(cmd,TrimTrainingComments(line));
				}

			}

			if (parser != null)	// should not happen, but don't loose data
			{
                parser.Finish();
				map.AddObject(parser.Object);
                parser = null;
			}

            map.FinishLoad();
			return map;
		}
    }
}
