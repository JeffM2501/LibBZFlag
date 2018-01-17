using BZFlag.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.IO.Compression;
using BZFlag.Data.Utils;
using BZFlag.Map.Elements;
using BZFlag.Map.Elements.Shapes;

using Ionic.Zlib;

namespace BZFlag.IO.BZW.Binary
{
    public class WorldPacker
    {
        WorldMap MapData = null;

        private DynamicOutputBuffer DynaBuffer = DynamicOutputBuffer.GetTempBuffer(64);

        public WorldPacker(WorldMap map)
        {
            MapData = map;
        }


        public byte[] Pack()
        {
            
            WriteDynamicColors();
            WriteTextureMatricies();
            WriteMaterials();
            WritePhysicsDrivers();
            WriteTransforms();
            WriteObstacles();
            WriteLinks();

            DynaBuffer.WriteFloat(-1); // water level
            DynaBuffer.WriteUInt32(0); // world weapons
            DynaBuffer.WriteUInt32(0); // zones

            return CompressMap();
        }

        protected byte[] CompressMap()
        {
            int uncompressedSize = DynaBuffer.GetBytesUsed();

            MemoryStream ms = new MemoryStream();
            Ionic.Zlib.ZlibStream ws = new Ionic.Zlib.ZlibStream(ms,Ionic.Zlib.CompressionMode.Compress, Ionic.Zlib.CompressionLevel.BestCompression, true);

            ws.Write(DynaBuffer.GetRawBuffer(), 0, uncompressedSize);
            ws.Flush();

            ws.Close();

            int len = (int)ms.Length;

            byte[] compressedData = ms.GetBuffer();
           // ms.Length();
            Array.Resize(ref compressedData, (int)ms.Length);

            DynamicOutputBuffer header = DynamicOutputBuffer.GetTempBuffer(64);
            header.WriteUInt16(Constants.WorldCodeHeaderSize);
            header.WriteUInt16(Constants.WorldCodeHeader);
            header.WriteUInt16(1);

            header.WriteUInt32(uncompressedSize);
            header.WriteUInt32(compressedData.Length);

            header.WriteBytes(compressedData);
            header.WriteUInt16(Constants.WorldCodeEndSize);
            header.WriteUInt16(Constants.WorldCodeEnd);

            return header.GetFinalBuffer();
        }

        protected void WriteLinks()
        {
            List<Link> items = FindAllObjectsOfType<Link>();

            DynaBuffer.WriteUInt32(items.Count);

            foreach (var link in items)
            {
                DynaBuffer.WriteULongPascalString(link.From.TargetName);
                DynaBuffer.WriteULongPascalString(link.To.TargetName);
            }
        }

        protected void WriteObstacles()
        {
            WriteWorldObject();

            WriteWalls(FindAllObjectsOfType<WallObstacle>());
            WriteBoxes(FindAllObjectsOfType<Box>());
            WritePyramids(FindAllObjectsOfType<Pyramid>());
            WriteBases(FindAllObjectsOfType<Base>());

            DynaBuffer.WriteUInt32(0); // teleType,
            DynaBuffer.WriteUInt32(0); // meshType,
            DynaBuffer.WriteUInt32(0); // arcType,
            DynaBuffer.WriteUInt32(0); // coneType,
            DynaBuffer.WriteUInt32(0); // sphereType,
            DynaBuffer.WriteUInt32(0); // tetraType,

            DynaBuffer.WriteUInt32(0); // groups

            DynaBuffer.WriteUInt32(0); // group instances

        }

        protected void WriteBases(List<Base> items)
        {
            DynaBuffer.WriteInt32(items.Count);

            foreach (var o in items)
            {
                DynaBuffer.WriteUInt16((UInt16)o.TeamColor);
                DynaBuffer.WriteVector3F(o.Position);
                DynaBuffer.WriteFloat(o.Rotation);
                DynaBuffer.WriteVector3F(o.Size);

                byte state = 0;

                if (o.DriveThrough)
                    state |= Constants.DRIVE_THRU;
                if (o.ShootThrough)
                    state |= Constants.SHOOT_THRU;
                if (o.Ricochet)
                    state |= Constants.RICOCHET;
                DynaBuffer.WriteByte(state);
            }
        }

        protected void WritePyramids(List<Pyramid> items)
        {
            DynaBuffer.WriteInt32(items.Count);

            foreach (var o in items)
            {
                DynaBuffer.WriteVector3F(o.Position);
                DynaBuffer.WriteFloat(o.Rotation);
                DynaBuffer.WriteVector3F(o.Size);

                byte state = 0;

                if (o.DriveThrough)
                    state |= Constants.DRIVE_THRU;
                if (o.ShootThrough)
                    state |= Constants.SHOOT_THRU;
                if (o.Ricochet)
                    state |= Constants.RICOCHET;
                if (o.FlipZ)
                    state |= Constants.FLIP_Z;
                DynaBuffer.WriteByte(state);
            }
        }

        protected void WriteBoxes(List<Box> items)
        {
            DynaBuffer.WriteInt32(items.Count);

            foreach (var box in items)
            {
                DynaBuffer.WriteVector3F(box.Position);
                DynaBuffer.WriteFloat(box.Rotation);
                DynaBuffer.WriteVector3F(box.Size);

                byte state = 0;

                if (box.DriveThrough)
                    state |= Constants.DRIVE_THRU;
                if (box.ShootThrough)
                    state |= Constants.SHOOT_THRU;
                if (box.Ricochet)
                    state |= Constants.RICOCHET;
                DynaBuffer.WriteByte(state);
            }
        }

        protected void WriteWalls(List<WallObstacle> walls)
        {
            DynaBuffer.WriteInt32(walls.Count);

            foreach (var wall in walls)
            {
                DynaBuffer.WriteVector3F(wall.Position);
                DynaBuffer.WriteFloat(wall.Rotation);
                DynaBuffer.WriteFloat(wall.Size.Y);
                DynaBuffer.WriteFloat(wall.Size.Z);
                DynaBuffer.WriteByte(wall.Ricochet ? Constants.RICOCHET : 0);
            }
        }

        protected void WriteWorldObject()
        {
            DynaBuffer.WriteULongPascalString(string.Empty);// MapData.WorldInfo.Name);
        }

        protected void WriteTransforms()
        {
            List<MeshTransform> items = FindAllObjectsOfType<MeshTransform>();
            DynaBuffer.WriteUInt32(items.Count);

            foreach (var xform in items)
                WriteMeshTransform(xform);
        }

        protected void WriteMeshTransform(MeshTransform t)
        {
            DynaBuffer.WriteULongPascalString(t.Name);
            DynaBuffer.WriteUInt32(t.Transforms.Count);

            foreach (var data in t.Transforms)
            {
                DynaBuffer.WriteByte((byte)data.XFormType);

                if (data.XFormType == MeshTransform.TransformType.IndexTransform)
                    DynaBuffer.WriteInt32(data.Index);
                else
                {
                    DynaBuffer.WriteFloat(data.Data.X);
                    DynaBuffer.WriteFloat(data.Data.Y);
                    DynaBuffer.WriteFloat(data.Data.Z);

                    if (data.XFormType == MeshTransform.TransformType.SpinTransform)
                        DynaBuffer.WriteFloat(data.Data.A);
                }
            }
        }

        protected void WritePhysicsDrivers()
        {
            List<Physics> items = FindAllObjectsOfType<Physics>();
            DynaBuffer.WriteUInt32(items.Count);

            foreach (var phy in items)
            {
                DynaBuffer.WriteULongPascalString(phy.Name);

                DynaBuffer.WriteVector3F(phy.Linear);
                DynaBuffer.WriteVector3F(phy.Angular);
                DynaBuffer.WriteVector3F(phy.Radial);

                DynaBuffer.WriteFloat(phy.Slide);
                DynaBuffer.WriteULongPascalString(phy.Death);
            }
        }

        protected void WriteMaterials()
        {
            List<Material> items = FindAllObjectsOfType<Material>();
            DynaBuffer.WriteUInt32(items.Count);

            foreach (var mat in items)
            {
                DynaBuffer.WriteULongPascalString(mat.Name);

                byte modeByte = 0;
                if (mat.NoCulling)
                    modeByte |= (1 << 0);
                if (mat.NoSorting)
                    modeByte |= (1 << 1);
                if (mat.NoRadar)
                    modeByte |= (1 << 2);
                if (mat.NoShadow)
                    modeByte |= (1 << 3);
                if (mat.Occluder)
                    modeByte |= (1 << 4);
                if (mat.GroupAlpha)
                    modeByte |= (1 << 5);
                if (mat.NoLighting)
                    modeByte |= (1 << 6);
                DynaBuffer.WriteByte(modeByte);

                DynaBuffer.WriteInt32(mat.DynamicColorRefrence);
                DynaBuffer.WriteColor4F(mat.Ambient);
                DynaBuffer.WriteColor4F(mat.Diffuse);
                DynaBuffer.WriteColor4F(mat.Specular);
                DynaBuffer.WriteColor4F(mat.Emission);
                DynaBuffer.WriteFloat(mat.Shininess);
                DynaBuffer.WriteFloat(mat.AlphaThreshold);

                DynaBuffer.WriteByte((byte)mat.Textures.Count);

                foreach (var tx in mat.Textures)
                {
                    DynaBuffer.WriteULongPascalString(tx.Name);
                    DynaBuffer.WriteInt32(tx.MatrixID);

                    DynaBuffer.WriteInt32((Int32)tx.CombineMode);

                    byte stateByte = 0;

                    if (tx.UseAlpha)
                        stateByte |= (1 << 0);
                    if (tx.UseColor)
                        stateByte |= (1 << 1);
                    if (tx.UseSphereMap)
                        stateByte |= (1 << 2);

                    DynaBuffer.WriteByte(stateByte);
                }

                DynaBuffer.WriteByte((byte)mat.Shaders.Count);

                foreach (var s in mat.Shaders)
                    DynaBuffer.WriteULongPascalString(s);
            }
        }

        protected void WriteTextureMatricies()
        {
            List<TextureMatrix> items = FindAllObjectsOfType<TextureMatrix>();
            DynaBuffer.WriteUInt32(items.Count);

            foreach (var tx in items)
            {
                DynaBuffer.WriteULongPascalString(tx.Name);

                byte state = 0;
                if (tx.UseStatic && tx.UseDynamic)
                    state = 4;
                else if (tx.UseStatic)
                    state = 1;
                else if (tx.UseDynamic)
                    state = 2;

                DynaBuffer.WriteByte(state);

                if (tx.UseStatic)
                {
                    DynaBuffer.WriteFloat(tx.Rotation);
                    DynaBuffer.WriteVector2F(tx.FixedShift);
                    DynaBuffer.WriteVector2F(tx.FixedScale);
                    DynaBuffer.WriteVector2F(tx.FixedCenter);
                }

                if (tx.UseDynamic)
                {
                    DynaBuffer.WriteFloat(tx.SpinFreq);
                    DynaBuffer.WriteVector2F(tx.ShiftFreq);
                    DynaBuffer.WriteVector2F(tx.ScaleFreq);
                    DynaBuffer.WriteVector2F(tx.Scale);
                    DynaBuffer.WriteVector2F(tx.Center);
                }
            }
        }

        protected void WriteDynamicColors()
        {
            List<DynamicColor> colors = FindAllObjectsOfType<DynamicColor>();

            DynaBuffer.WriteUInt32(colors.Count);
            foreach (DynamicColor color in colors)
            {
                DynaBuffer.WriteULongPascalString(color.Name);

                for (int c = 0; c < 4; c++)
                {
                    DynamicColor.ChannelParams chan = color.Channels[c];


                    DynaBuffer.WriteFloat(chan.MinValue);
                    DynaBuffer.WriteFloat(chan.MinValue);

                    DynaBuffer.WriteUInt32(chan.Sinusoids.Count);

                    foreach (var p in chan.Sinusoids)
                    {
                        DynaBuffer.WriteFloat(p.Period);
                        DynaBuffer.WriteFloat(p.Offset);
                        DynaBuffer.WriteFloat(p.Weight);
                    }


                    DynaBuffer.WriteUInt32(chan.ClampUps.Count);
                    foreach (var p in chan.ClampUps)
                    {
                        DynaBuffer.WriteFloat(p.Period);
                        DynaBuffer.WriteFloat(p.Offset);
                        DynaBuffer.WriteFloat(p.Width);
                    }

                    DynaBuffer.WriteUInt32(chan.ClampDowns.Count);
                    foreach (var p in chan.ClampDowns)
                    {
                        DynaBuffer.WriteFloat(p.Period);
                        DynaBuffer.WriteFloat(p.Offset);
                        DynaBuffer.WriteFloat(p.Width);
                    }

                    DynaBuffer.WriteUInt32(chan.Sequence.Values.Count);
                    if (chan.Sequence.Values.Count > 0)
                    {
                        DynaBuffer.WriteFloat(chan.Sequence.Period);
                        DynaBuffer.WriteFloat(chan.Sequence.Offset);

                        foreach (var p in chan.Sequence.Values)
                            DynaBuffer.WriteByte(p);
                    }
                }
            }
        }

        private List<T> FindAllObjectsOfType<T>() where T : Map.Elements.BasicObject
        {
            List<T> objects = new List<T>();

            foreach (var i in MapData.Objects)
            {
                if (i.PackAs() == typeof(T))
                    objects.Add(i as T);
            }
            return objects;
        }
    }
}
