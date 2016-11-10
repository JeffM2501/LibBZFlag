using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Map.Elements;

namespace BZFlag.IO.BZW.Parsers
{
	public class LinkParser : BasicObjectParser
	{
        public LinkParser()
        {
            Object = new Link();
        }

        public LinkParser(Link obj)
        {
            Object = obj;
        }

        
        protected bool IsWildcardCharacter(char c)
        {
            return c == '*' || c == '?';
        }

		protected bool IsWildcardCharacter(string str)
		{
			return str != string.Empty && IsWildcardCharacter(str[0]);
		}

		public Link.PorterLink ImportPorterLink(string code)
        {
            Link.PorterLink l = new Link.PorterLink();
       
            if (code == string.Empty)
            {
                l.Wildcard = true;
                return l;
            }

            l.Wildcard = false;

            if (code.Contains(':'))
            {
                string[] parts = code.Split(":".ToCharArray());
                if (parts.Length == 2)
                {
                    l.TargetName = parts[0];
                    if (IsWildcardCharacter(parts[1]))
                    {
                        l.TargetGroup = l.TargetName;
                        l.TargetName = string.Empty;
                        l.Wildcard = true;
                    }
                    else
                        l.Front = parts[1].ToLowerInvariant() != "b";
                }
                else if (parts.Length > 2)
                {
                    // groups and shit!
                    l.TargetGroup = parts[0];
                    l.TargetName = parts[1];
                    if (IsWildcardCharacter(parts[2]))
                        l.Wildcard = true;
                    else
                        l.Front = parts[1].ToLowerInvariant() != "b";
                }
            }
            else if (IsWildcardCharacter(code.Trim()[0]))
                l.Wildcard = true;
            else
            {
                int faceID = 0;
                int.TryParse(code, out faceID);

                l.TargetName = "teleporter_" + (faceID / 2).ToString();
                l.Front = faceID % 2 == 1;
            }
            return l;
        }

        public string GetPorterLinkCode(Link.PorterLink l)
        {
            string Code = string.Empty;

            if (l.TargetGroup != string.Empty)
                Code += l.TargetGroup + ":";
            if (l.TargetName != string.Empty)
                Code += l.TargetName + ":";

            if (l.Wildcard)
                Code += "*";
            else
                Code += l.Front ? "f" : "b";

            return Code;
        }

		public override bool AddCodeLine(string command, string line)
		{
            Link l = Object as Link;
            if (l == null)
                return base.AddCodeLine(command, line);

            if (!base.AddCodeLine(command, line))
			{
                if (command == "FROM")
                    l.From = ImportPorterLink(Reader.GetRestOfWords(line));
                else if (command == "TO")
                    l.To = ImportPorterLink(Reader.GetRestOfWords(line));
                else
                    l.Attributes.Add(line);
			}

			return true;
		}

		public override string BuildCode()
		{
            Link l = Object as Link;
            if (l == null)
                return base.BuildCode();

            Code.Clear();

            if (l.Name != string.Empty)
			    AddCode(1, "name", l.Name);

            AddCode(1, "from", GetPorterLinkCode(l.From));
            AddCode(1, "to", GetPorterLinkCode(l.To));
            foreach (var s in l.Attributes)
				AddCode(2, s);

			return l.ObjectType;
		}
	}
}
