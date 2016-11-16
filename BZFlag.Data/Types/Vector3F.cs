using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Data.Types
{
    public class Vector3F : IEnumerable<float>
    {
        protected float[] Pos = new float[] { 0, 0, 0 };

        public float X
        {
            get { return Pos[0]; }
            set { Pos[0] = value; }
        }

        public float Y
        {
            get { return Pos[1]; }
            set { Pos[1] = value; }
        }

        public float Z
        {
            get { return Pos[2]; }
            set { Pos[2] = value; }
        }

        public float this[int key]
        {
            get
            {
                return Pos[key];
            }
            set
            {
                Pos[key] = value;
            }
        }

        public IEnumerator<float> GetEnumerator()
        {
            return ((IEnumerable<float>)Pos).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<float>)Pos).GetEnumerator();
        }

        public Vector3F() { }

        public Vector3F(float x, float y, float z)
        {
            Pos[0] = x;
            Pos[1] = y;
            Pos[2] = z;
        }

        public Vector3F(float[] v, int start = 0)
        {
            Pos[0] = v[start];
            Pos[1] = v[start+1];
            Pos[2] = v[start+2];
        }

        public Vector3F(Vector2F v)
        {
            Pos[0] = v.X;
            Pos[1] = v.Y;
        }

        public Vector3F(Vector4F v)
        {
            Pos[0] = v.X;
            Pos[1] = v.Y;
            Pos[2] = v.Z;
        }

        public static Vector3F operator + (Vector3F lhs, Vector3F rhs)
        {
            return new Vector3F(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);
        }

        public static Vector3F operator -(Vector3F lhs, Vector3F rhs)
        {
            return new Vector3F(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z);
        }

        public static Vector3F operator *(Vector3F lhs, float rhs)
        {
            return new Vector3F(lhs.X * rhs, lhs.Y * rhs, lhs.Z * rhs);
        }

        public static Vector3F operator *(Vector3F lhs, double rhs)
        {
            return new Vector3F((float)(lhs.X * rhs), (float)(lhs.Y * rhs), (float)(lhs.Z * rhs));
        }

        public bool IsZero ()
        {
            return Pos[0] == 0.0f && Pos[1] == 0.0f && Pos[2] == 0.0f;
        }

        public static readonly Vector3F Zero = new Vector3F(0, 0, 0);
    }
}
