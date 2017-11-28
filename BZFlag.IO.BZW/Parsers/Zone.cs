using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Teams;
using BZFlag.Map.Elements.Shapes;


namespace BZFlag.IO.BZW.Parsers
{
    public class ZoneParser : PositionableObjectParser
    {
        public ZoneParser()
        {
            Object = new Zone();
        }

        public ZoneParser(Zone obj)
        {
            Object = obj;
        }

        protected IEnumerable<TeamColors> IntListToBaseColors(IEnumerable<int> bases)
        {
            List<TeamColors> teams = new List<TeamColors>();
            foreach (var b in bases)
                teams.Add((TeamColors)b);
            return teams;
        }

        protected IEnumerable<int> BaseColorListToInts(IEnumerable<TeamColors> bases)
        {
            List<int> teams = new List<int>();
            foreach (var b in bases)
                teams.Add((int)b);
            return teams;
        }

        public override bool AddCodeLine(string command, string line)
        {
            Zone p = Object as Zone;
            if (p == null)
                return base.AddCodeLine(command, line);

            string nubs = Reader.GetRestOfWords(line);
            if (!base.AddCodeLine(command, line))
            {
                if (command == "FLAG")
                    p.Flags.Add(nubs);
                else if (command == "ZONEFLAG")
                    p.ZoneFlags.Add(nubs);
                else if (command == "SAFE")
                    p.Safe.AddRange(IntListToBaseColors(Reader.ParseIntVector(nubs)));
                else if (command == "TEAM")
                    p.Team.AddRange(IntListToBaseColors(Reader.ParseIntVector(nubs)));
                else
                    p.Attributes.Add(line);
            }

            return true;
        }

        public override void Finish()
        {

        }

        public override string BuildCode()
        {
            Zone p = Object as Zone;
            if (p == null)
                return base.BuildCode();

            string name = base.BuildCode();

            foreach (var f in p.ZoneFlags)
                AddCode(1, "zoneflag", f);

            foreach (var f in p.Flags)
                AddCode(1, "flag", f);

            if (p.Safe.Count > 0)
                AddCode(1, "safe", string.Join(" ", Utilities.GetStringList<int>(BaseColorListToInts(p.Safe))));
            if (p.Team.Count > 0)
                AddCode(1, "team", string.Join(" ", Utilities.GetStringList<int>(BaseColorListToInts(p.Team))));

            return name;
        }
    }
}
