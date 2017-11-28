using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Data.Types
{
    public class Vector2F : IEnumerable<float>
    {
        protected float[] Pos = new float[] { 0, 0 };

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

        public Vector2F() { }

        public Vector2F(float x, float y)
        {
            Pos[0] = x;
            Pos[1] = y;
        }

        public Vector2F(float[] v, int start = 0)
        {
            Pos[0] = v[start];
            Pos[1] = v[start + 1];
        }

        public Vector2F(Vector3F v)
        {
            Pos[0] = v.X;
            Pos[1] = v.Y;
        }

        public Vector2F(Vector4F v)
        {
            Pos[0] = v.X;
            Pos[1] = v.Y;
        }


        public static Vector2F operator +(Vector2F lhs, Vector2F rhs)
        {
            return new Vector2F(lhs.X + rhs.X, lhs.Y + rhs.Y);
        }

        public static Vector2F operator -(Vector2F lhs, Vector2F rhs)
        {
            return new Vector2F(lhs.X - rhs.X, lhs.Y - rhs.Y);
        }

        public static Vector2F operator *(Vector2F lhs, float rhs)
        {
            return new Vector2F(lhs.X * rhs, lhs.Y * rhs);
        }

        public static readonly Vector2F Zero = new Vector2F(0, 0);
    }
}