using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Types;
using BZFlag.Map.Elements.Shapes;

namespace BZFlag.IO.BZW.Parsers
{
    public class MeshParser : PositionableObjectParser
    {
       public Mesh.Transformation ParseTransformation( string type, string data)
        {
            Mesh.Transformation t = new Mesh.Transformation();

            if (type == "SCALE")
            {
                t.TransformType = Mesh.Transformation.TransformTypes.Scale;
                t.Value = new Vector4F(Utilities.ReadVector3F(data));
            }
            else if (type == "SHIFT")
            {
                t.TransformType = Mesh.Transformation.TransformTypes.Shift;
                t.Value = new Vector4F(Utilities.ReadVector3F(data));
            }
            else if (type == "SHEAR")
            {
                t.TransformType = Mesh.Transformation.TransformTypes.Shear;
                t.Value = new Vector4F(Utilities.ReadVector3F(data));
            }
            else if (type == "SPIN")
            {
                t.TransformType = Mesh.Transformation.TransformTypes.Spin;
                t.Value = Utilities.ReadVector4F(data);
            }
            return t;
        }


        private Mesh.Face TempFace = null;

        public MeshParser()
        {
            Object = new Mesh();
        }

        public MeshParser(Mesh obj)
        {
            Object = obj;
        }

        public override bool AddCodeLine(string command, string line)
        {
            Mesh p = Object as Mesh;
            if (p == null)
                return base.AddCodeLine(command, line);

            Code.Add(line);// save the raw data

            string nub = Reader.GetRestOfWords(line);

            if (command == "VERTEX")
                p.Vertecies.Add(Utilities.ReadVector3F(nub));
            else if (command == "NORMAL")
                p.Normals.Add(Utilities.ReadVector3F(nub));
            else if (command == "TEXTCOORD")
                p.UVs.Add(Utilities.ReadVector2F(nub));
            else if (command == "INSIDE")
                p.InsidePoints.Add(Utilities.ReadVector3F(nub));
            else if (command == "OUTSIDE")
                p.OutsidePoints.Add(Utilities.ReadVector3F(nub));
            else if (command == "SHIFT" || command == "SPIN" || command == "SCALE" || command == "SHEAR")
                p.Transforms.Add(ParseTransformation(command,nub));
            else if (command == "FACE")
            {
                if (TempFace != null)
                    p.Faces.Add(TempFace);

                TempFace = new Mesh.Face();
            }
            else if (command == "ENDFACE")
            {
                if (TempFace != null)
                    p.Faces.Add(TempFace);

                TempFace = null;
            }
            else if (command == "PHYDRV")
            {
                if (TempFace != null)
                    TempFace.PhysicsDriver = nub;
                else
                    p.PhysicsDriver = nub;
            }
            else if (command == "NOCLOSTERS")
            {
                if (TempFace != null)
                    TempFace.NoClusters = true;
                else
                    p.NoClusters = true;
            }
            else if (command == "SMOOTHBOUNCE")
            {
                if (TempFace != null)
                    TempFace.SmoothBounce = true;
                else
                    p.SmoothBounce = true;
            }
            else if (command == "PASSABLE")
            {
                if (TempFace != null)
                    TempFace.Passable = true;
            }
            else if (command == "DRIVETHROUGH")
            {
                if (TempFace != null)
                    TempFace.DriveThrough = true;
            }
            else if (command == "SHOOTTHROUGH")
            {
                if (TempFace != null)
                    TempFace.ShootThrough = true;
            }
            else if (!base.AddCodeLine(command, line))
                return false;

            return true;
        }

        public override void Finish()
        {
            Mesh p = Object as Mesh;
            if (p == null)
                base.Finish();
            else
            {
                if (TempFace != null)
                    p.Faces.Add(TempFace);

                base.Finish();
            }
        }

        public override string BuildCode()
        {
            Mesh m = Object as Mesh;
            if (m == null)
                return base.BuildCode();
                string name = base.BuildCode();

            foreach (var p in m.InsidePoints)
                AddCode(1, "inside", p);

            foreach (var p in m.OutsidePoints)
                AddCode(1, "outside", p);

            foreach (var p in m.Vertecies)
                AddCode(1, "vertex", p);
            foreach (var p in m.Normals)
                AddCode(1, "normal", p);
            foreach (var p in m.UVs)
                AddCode(1, "texcoord", p);

            foreach (var xform in m.Transforms)
            {
                switch (xform.TransformType)
                {
                    case Mesh.Transformation.TransformTypes.Scale:
                        AddCode(1, "scale", new Vector3F(xform.Value));
                        break;
                    case Mesh.Transformation.TransformTypes.Shear:
                        AddCode(1, "shear", new Vector3F(xform.Value));
                        break;
                    case Mesh.Transformation.TransformTypes.Shift:
                        AddCode(1, "shift", new Vector3F(xform.Value));
                        break;
                    case Mesh.Transformation.TransformTypes.Spin:
                        AddCode(1, "spin", xform.Value);
                        break;
                }
            }

            if (m.PhysicsDriver != string.Empty)
                AddCode(1, "phydrv", m.PhysicsDriver);

            if (m.NoClusters)
                AddCode(1, "noclusters");

            if (m.SmoothBounce)
                AddCode(1, "smoothbounce");

            foreach(var face in m.Faces)
            {
                AddCode(1, "face");

                AddCode(2, "vertices", string.Join(" ",Utilities.GetStringList<int>(face.Vertecies)));
                AddCode(2, "normals", string.Join(" ", Utilities.GetStringList<int>(face.Normals)));
                AddCode(2, "texcoords", string.Join(" ", Utilities.GetStringList<int>(face.UVs)));

                if (face.PhysicsDriver != string.Empty)
                    AddCode(2, "phydrv", m.PhysicsDriver);

                if (face.NoClusters)
                    AddCode(2, "noclusters");

                if (face.SmoothBounce)
                    AddCode(2, "smoothbounce");

                if (face.Passable)
                    AddCode(2, "passable");
                else
                {
                    if (face.ShootThrough)
                        AddCode(2, "shootthrough");
                    if (face.DriveThrough)
                        AddCode(2, "drivethrough");
                }

                AddCode(1, "endface");
            }

           return name;
        }
    }
}
