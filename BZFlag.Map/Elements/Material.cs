using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Types;

namespace BZFlag.Map.Elements
{
	public class Material : BasicObject
	{
		public List<string> Aliases = new List<string>();

		public int DynamicColorRefrence = int.MinValue;

		public Color4F Ambient = Color4F.Empty;
		public Color4F Diffuse = Color4F.White;
		public Color4F Specular = Color4F.White;
		public Color4F Emission = Color4F.Empty;
		public float Shininess = 0;

		public bool Occluder = false;
		public bool GroupAlpha = false;
		public bool NoRadar = false;
		public bool NoShadow = false;
		public bool NoCulling = false;
		public bool NoSorting = false;
		public bool NoLighting = false;
		public float AlphaThreshold = 0;

		public enum CombineModes
		{
			Replace = 0,
			Modulate,
			Decal,
			Blend,
			Add,
			Combine
		}

		public class TextureInfo
		{
			public string Name = string.Empty;
			public string LocalName = string.Empty;
			public int MatrixID = -1;
			public CombineModes CombineMode = CombineModes.Replace;
			public bool UseAlpha = false;
			public bool UseColor = false;
			public bool UseSphereMap = false;
		}

		public List<TextureInfo> Textures = new List<TextureInfo>();

		public List<String> Shaders = new List<String>();

		public Material() : base()
		{
			ObjectType = "Material";
		}
	}
}
