using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Map.Elements.Shapes
{
    public class Sphere : PhaseableObject
    {
        public enum MaterialLocations
        {
            Edge = 0,
            Bottom,
            MaterialCount
        };

        public MeshTransform Transform = new MeshTransform();
        public int Divisions = 0;
        public bool Hemisphere = false;
        public int PhysicsID = -1;
        public bool SmoothBounce = false;
        public bool UseNormals = false;

        public float[] TextureSize = new float[2];
        public Dictionary<MaterialLocations, int> MaterialIDs = new Dictionary<MaterialLocations, int>();

        public Sphere()
        {
            ObjectType = "Sphere";
        }
    }
}
