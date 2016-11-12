using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Data.Types
{
	public class Color4F : IEnumerable<float>
	{
		protected float[] Values = new float[] { 0, 0, 0, 0 };

		public float R
		{
			get { return Values[0]; }
			set { Values[0] = value; }
		}

		public float G
		{
			get { return Values[1]; }
			set { Values[1] = value; }
		}

		public float B
		{
			get { return Values[2]; }
			set { Values[2] = value; }
		}

		public float A
		{
			get { return Values[3]; }
			set { Values[3] = value; }
		}

		public float this[int key]
		{
			get
			{
				return Values[key];
			}
			set
			{
				Values[key] = value;
			}
		}

		public IEnumerator<float> GetEnumerator()
		{
			return ((IEnumerable<float>)Values).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<float>)Values).GetEnumerator();
		}


		public Color4F() { }

		public Color4F(float r, float g, float b)
		{
			Values[0] = r;
			Values[1] = g;
			Values[2] = b;
			Values[3] = 1;
		}

		public Color4F(float r, float g, float b, float a)
		{
			Values[0] = r;
			Values[1] = g;
			Values[2] = b;
			Values[3] = a;
		}

		public Color4F(float[] v, int start = 0)
		{
			Values[0] = v[start];
			Values[1] = v[start + 1];
			Values[2] = v[start + 2];
			Values[3] = v[start + 3];
		}

		public static readonly Color4F Empty = new Color4F(0, 0, 0, 0);
		public static readonly Color4F White = new Color4F(1, 1, 1, 1);
	}
}
