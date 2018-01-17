using BZFlag.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Map.Elements;

using BZFlag.LinearMath;
using BZFlag.IO.BZW;

namespace BZFlag.IO.BZW.Parsers
{
    public class BasicObjectParser
    {
        public virtual string ObjectTerminator { get { return "end"; } }

        public List<string> Code = new List<string>(); // output code

        public BasicObject Object = null;

        public BasicObjectParser()
        {
            Object = new BasicObject();
        }

        public BasicObjectParser(BasicObject obj)
        {
            Object = obj;
        }


        public virtual void Init(string objectType, string typeParams)
        {
            Object.ObjectType = objectType;
        }

        public virtual void Finish()
        {

        }

        public virtual bool AddCodeLine(string command, string line)
        {

            if (command == "NAME")
            {
                Object.Name = Reader.GetRestOfWords(line);
                return true;
            }
            Object.Attributes.Add(line);
            return false;
        }

        public virtual string BuildCode()
        {
            Code.Clear();

            string t = Object.ObjectType;
            if (Object.TypeParams != null)
                t += " " + Object.TypeParams;

            if (Object.GetType() == typeof(BasicObject))    // serializing an unknown object, just dump it;
                Code.AddRange(Object.Attributes.ToArray());
            return t;
        }

        protected StringBuilder GetIndent(int indent)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < indent; i++)
                sb.Append("\t");

            return sb;
        }

        public void AddCode(int indent, string name, string value)
        {
            if (name == string.Empty || value == string.Empty)
                return;

            StringBuilder sb = GetIndent(indent);
            sb.Append(name);
            if (value != string.Empty)
            {
                sb.Append(" ");
                sb.Append(value);
            }

            Code.Add(sb.ToString());
        }

        public void AddCode(int indent, string name, float value)
        {
            AddCode(indent, name, value.ToString());
        }

        public void AddCode(int indent, string name, bool value)
        {
            AddCode(indent, name, value ? "1" : "0");
        }

        public void AddCode(int indent, string name)
        {
            if (name == string.Empty)
                return;

            StringBuilder sb = GetIndent(indent);
            sb.Append(name);
            Code.Add(sb.ToString());
        }

        public void AddCode(int indent, string name, float[] values)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < values.Length; i++)
            {
                sb.Append(values[i].ToString());
                if (i != values.Length - 1)
                    sb.Append(" ");
            }

            AddCode(indent, name, sb.ToString());
        }

        public void AddCode(int indent, string name, Vector4F value)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(value.A.ToString());
            sb.Append(" ");
            sb.Append(value.X.ToString());
            sb.Append(" ");
            sb.Append(value.Y.ToString());
            sb.Append(" ");
            sb.Append(value.Z.ToString());
            AddCode(indent, name, sb.ToString());
        }

        public void AddCode(int indent, string name, Vector3F value)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(value.X.ToString());
            sb.Append(" ");
            sb.Append(value.Y.ToString());
            sb.Append(" ");
            sb.Append(value.Z.ToString());
            AddCode(indent, name, sb.ToString());
        }

        public void AddCode(int indent, string name, Vector2F value)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(value.X.ToString());
            sb.Append(" ");
            sb.Append(value.Y.ToString());
            AddCode(indent, name, sb.ToString());
        }
    }
}
