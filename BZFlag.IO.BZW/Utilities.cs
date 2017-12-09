using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Types;
using BZFlag.LinearMath;
using BZFlag.IO.BZW;

namespace BZFlag.IO.BZW
{
    public static class Utilities
    {
        public static string[] GetStringList<T>(IEnumerable<T> inList)
        {
            List<string> outList = new List<string>();
            foreach (var o in inList)
                outList.Add(o.ToString());

            return outList.ToArray();
        }
        public static Vector2F ReadVector2F(string line)
        {
            Vector2F v = new Vector2F();

            var vec = Reader.ParseFloatVector(line);
            for (int i = 0; i < 2 && i < vec.Count; i++)
                v[i] = vec[i];

            return v;
        }

        public static Vector3F ReadVector3F(string line)
        {
            Vector3F v = new Vector3F();

            var vec = Reader.ParseFloatVector(line);
            for (int i = 0; i < 3 && i < vec.Count; i++)
                v[i] = vec[i];

            return v;
        }

        public static Vector4F ReadVector4F(string line)
        {
            Vector4F v = new Vector4F();

            var vec = Reader.ParseFloatVector(line);
            for (int i = 0; i < 4 && i < vec.Count; i++)
                v[i] = vec[i];

            return v;
        }
    }
}
