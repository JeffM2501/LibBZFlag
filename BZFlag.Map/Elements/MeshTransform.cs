using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Types;

namespace BZFlag.Map.Elements
{
	public class MeshTransform :  BasicObject
	{
		public enum TransformType
		{
			ShiftTransform = 0,
			ScaleTransform = 1,
			ShearTransform = 2,
			SpinTransform = 3,
			IndexTransform = 4,
			LastTransform
		}

		public class TransformData
		{
			public TransformType XFormType = TransformType.LastTransform;
			public int Index = -1;
			public Vector4F Data = Vector4F.Zero;
		}

		public List<TransformData> Transforms = new List<TransformData>();

		public MeshTransform()
		{
			ObjectType = "Transform";
		}
	}
}
