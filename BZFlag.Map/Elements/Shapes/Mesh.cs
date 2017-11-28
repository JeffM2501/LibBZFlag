using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Types;

namespace BZFlag.Map.Elements.Shapes
{
    public class Mesh : PhaseableObject
    {
        public List<Vector3F> InsidePoints = new List<Vector3F>();
        public List<Vector3F> OutsidePoints = new List<Vector3F>();

        public List<Vector3F> Vertecies = new List<Vector3F>();
        public List<Vector3F> Normals = new List<Vector3F>();
        public List<Vector2F> UVs = new List<Vector2F>();

        public class Face
        {
            public List<int> Vertecies = new List<int>();
            public List<int> Normals = new List<int>();
            public List<int> UVs = new List<int>();

            public string PhysicsDriver = string.Empty;
            public bool SmoothBounce = false;
            public bool NoClusters = false;

            public bool Passable = false;
            public bool ShootThrough = false;
            public bool DriveThrough = false;
            public bool Ricochet = false;

            public int MaterialID = -1;
            public int PhysicsDriverID = -1;
        }

        public List<Face> Faces = new List<Face>();

        public class Transformation
        {
            public enum TransformTypes
            {
                Scale,
                Shift,
                Shear,
                Spin,
            }
            public TransformTypes TransformType = TransformTypes.Shift;
            public Vector4F Value = new Vector4F();

            public Transformation() { }
        }

        public List<Transformation> Transforms = new List<Transformation>();

        public string PhysicsDriver = string.Empty;
        public bool SmoothBounce = false;
        public bool NoClusters = false;

        public Mesh()
        {
            ObjectType = "Mesh";
        }

    }
}
