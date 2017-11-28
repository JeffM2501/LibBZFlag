using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Types;

namespace BZFlag.Map.Elements.Shapes
{
    public class Tetra : PhaseableObject
    {
        public MeshTransform Transform = new MeshTransform();

        public List<Vector3F> Verts = new List<Vector3F>();

        public class Face
        {
            public bool UseNormals = false;
            public List<Vector3F> Norms = new List<Vector3F>();

            public bool UseUVs = false;
            public List<Vector2F> UVs = new List<Vector2F>();

            public int MaterialID = 0;
        }

        public List<Face> Faces = new List<Face>();

        public Tetra()
        {
            ObjectType = "Sphere";
        }
    }
}
