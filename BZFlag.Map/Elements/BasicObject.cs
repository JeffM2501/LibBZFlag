using BZFlag.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Map.Elements
{
    public class BasicObject
    {
        public string ObjectType = string.Empty;
        public string TypeParams = string.Empty;

        public string Name = string.Empty;

        public string GUID = string.Empty;

        public List<string> Attributes = new List<string>();

        private static Random RNG = new Random();

        public Type PackAs() { return this.GetType(); }

        public BasicObject()
        {
            ObjectType = "Unknown";

            GUID = RNG.Next().ToString() + RNG.Next().ToString() + RNG.Next().ToString();
        }
    }
}
