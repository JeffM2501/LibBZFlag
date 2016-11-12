using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Map.Elements.Shapes
{
	public class Arc : PhaseableObject
	{
		public enum MaterialLocations
		{
			Top = 0,
			Bottom,
			Inside,
			Outside,
			StartFace,
			EndFace,
			MaterialCount
		};

		public MeshTransform Transform = new MeshTransform();
		public int Divisions = 0;
		public float SweepAngle = 0;
		public float Ratio = 0;
		public int PhysicsID = -1;
		public bool SmoothBounce = false;
		public bool UseNormals = false;

		public float[] TextureSize = new float[4];
		public Dictionary<MaterialLocations,int> MaterialIDs = new Dictionary<MaterialLocations, int>();

		public Arc()
		{
			ObjectType = "Arc";
		}
	}
}
