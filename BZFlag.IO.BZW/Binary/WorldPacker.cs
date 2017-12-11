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
    public class WorldPacker : DynamicOutputBuffer
    {
        WorldMap MapData = null;

        public WorldPacker(WorldMap map) : base(false, 64)
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

            WriteFloat(0); // water level
            WriteUInt32(0); // world weapons
            WriteUInt32(0); // zones

            return CompressMap();
        }


        protected byte[] CompressMap()
        {
            int uncompressedSize = BytesUsed;

            MemoryStream ms = new MemoryStream();
            Ionic.Zlib.ZlibStream ws = new Ionic.Zlib.ZlibStream(ms,Ionic.Zlib.CompressionMode.Compress, Ionic.Zlib.CompressionLevel.Default, true);

            ws.Write(Buffer, 0, uncompressedSize);
            ws.Flush();

            ws.Close();

            int len = (int)ms.Length;

            byte[] compressedData = ms.GetBuffer();
           // ms.Length();
            Array.Resize(ref compressedData, (int)ms.Length);

            DynamicOutputBuffer header = new DynamicOutputBuffer(false, 64);
            header.WriteUInt16(Constants.WorldCodeHeaderSize);
            header.WriteUInt16(Constants.WorldCodeHeader);
            header.WriteUInt16(1);

            header.WriteUInt32(uncompressedSize);
            header.WriteUInt32(compressedData.Length);

            header.WriteUInt16(0);

            header.WriteBytes(compressedData);
            header.WriteUInt16(Constants.WorldCodeEndSize);
            header.WriteUInt16(Constants.WorldCodeEnd);

            return header.GetFinalBuffer();
        }

        protected void WriteLinks()
        {
            List<Link> items = FindAllObjectsOfType<Link>();

            WriteUInt32(items.Count);

            foreach (var link in items)
            {
                WriteULongPascalString(link.From.TargetName);
                WriteULongPascalString(link.To.TargetName);
            }
        }

        protected void WriteObstacles()
        {
            WriteWorldObject();

            WriteWalls(FindAllObjectsOfType<WallObstacle>());
            WriteBoxes(FindAllObjectsOfType<Box>());
            WritePyramids(FindAllObjectsOfType<Pyramid>());
            WriteBases(FindAllObjectsOfType<Base>());

            WriteUInt32(0); // teleType,
            WriteUInt32(0); // meshType,
            WriteUInt32(0); // arcType,
            WriteUInt32(0); // coneType,
            WriteUInt32(0); // sphereType,
            WriteUInt32(0); // tetraType,

            WriteUInt32(0); // groups

            WriteUInt32(0); // group instances

        }

        protected void WriteBases(List<Base> items)
        {
            WriteInt32(items.Count);

            foreach (var o in items)
            {
                WriteUInt16((UInt16)o.TeamColor);
                WriteVector3F(o.Position);
                WriteFloat(o.Rotation);
                WriteVector3F(o.Size);

                byte state = 0;

                if (o.DriveThrough)
                    state |= Constants.DRIVE_THRU;
                if (o.ShootThrough)
                    state |= Constants.SHOOT_THRU;
                if (o.Ricochet)
                    state |= Constants.RICOCHET;
                WriteByte(state);
            }
        }

        protected void WritePyramids(List<Pyramid> items)
        {
            WriteInt32(items.Count);

            foreach (var o in items)
            {
                WriteVector3F(o.Position);
                WriteFloat(o.Rotation);
                WriteVector3F(o.Size);

                byte state = 0;

                if (o.DriveThrough)
                    state |= Constants.DRIVE_THRU;
                if (o.ShootThrough)
                    state |= Constants.SHOOT_THRU;
                if (o.Ricochet)
                    state |= Constants.RICOCHET;
                if (o.FlipZ)
                    state |= Constants.FLIP_Z;
                WriteByte(state);
            }
        }

        protected void WriteBoxes(List<Box> items)
        {
            WriteInt32(items.Count);

            foreach (var box in items)
            {
                WriteVector3F(box.Position);
                WriteFloat(box.Rotation);
                WriteVector3F(box.Size);

                byte state = 0;

                if (box.DriveThrough)
                    state |= Constants.DRIVE_THRU;
                if (box.ShootThrough)
                    state |= Constants.SHOOT_THRU;
                if (box.Ricochet)
                    state |= Constants.RICOCHET;
                WriteByte(state);
            }
        }

        protected void WriteWalls(List<WallObstacle> walls)
        {
            WriteInt32(walls.Count);

            foreach (var wall in walls)
            {
                WriteVector3F(wall.Position);
                WriteFloat(wall.Rotation);
                WriteFloat(wall.Size.Y);
                WriteFloat(wall.Size.Z);
                WriteByte(wall.Ricochet ? Constants.RICOCHET : 0);
            }
        }

        protected void WriteWorldObject()
        {
            WriteULongPascalString(string.Empty);// MapData.WorldInfo.Name);
        }

        protected void WriteTransforms()
        {
            List<MeshTransform> items = FindAllObjectsOfType<MeshTransform>();
            WriteUInt32(items.Count);

            foreach (var xform in items)
                WriteMeshTransform(xform);
        }

        protected void WriteMeshTransform(MeshTransform t)
        {
            WriteULongPascalString(t.Name);
            WriteUInt32(t.Transforms.Count);

            foreach (var data in t.Transforms)
            {
                WriteByte((byte)data.XFormType);

                if (data.XFormType == MeshTransform.TransformType.IndexTransform)
                    WriteInt32(data.Index);
                else
                {
                    WriteFloat(data.Data.X);
                    WriteFloat(data.Data.Y);
                    WriteFloat(data.Data.Z);

                    if (data.XFormType == MeshTransform.TransformType.SpinTransform)
                        WriteFloat(data.Data.A);
                }
            }
        }

        protected void WritePhysicsDrivers()
        {
            List<Physics> items = FindAllObjectsOfType<Physics>();
            WriteUInt32(items.Count);

            foreach (var phy in items)
            {
                WriteULongPascalString(phy.Name);

                WriteVector3F(phy.Linear);
                WriteVector3F(phy.Angular);
                WriteVector3F(phy.Radial);

                WriteFloat(phy.Slide);
                WriteULongPascalString(phy.Death);
            }
        }

        protected void WriteMaterials()
        {
            List<Material> items = FindAllObjectsOfType<Material>();
            WriteUInt32(items.Count);

            foreach (var mat in items)
            {
                WriteULongPascalString(mat.Name);

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
                WriteByte(modeByte);

                WriteInt32(mat.DynamicColorRefrence);
                WriteColor4F(mat.Ambient);
                WriteColor4F(mat.Diffuse);
                WriteColor4F(mat.Specular);
                WriteColor4F(mat.Emission);
                WriteFloat(mat.Shininess);
                WriteFloat(mat.AlphaThreshold);

                WriteByte((byte)mat.Textures.Count);

                foreach (var tx in mat.Textures)
                {
                    WriteULongPascalString(tx.Name);
                    WriteInt32(tx.MatrixID);

                    WriteInt32((Int32)tx.CombineMode);

                    byte stateByte = 0;

                    if (tx.UseAlpha)
                        stateByte |= (1 << 0);
                    if (tx.UseColor)
                        stateByte |= (1 << 1);
                    if (tx.UseSphereMap)
                        stateByte |= (1 << 2);

                    WriteByte(stateByte);
                }

                WriteByte((byte)mat.Shaders.Count);

                foreach (var s in mat.Shaders)
                    WriteULongPascalString(s);
            }
        }

        protected void WriteTextureMatricies()
        {
            List<TextureMatrix> items = FindAllObjectsOfType<TextureMatrix>();
            WriteUInt32(items.Count);

            foreach (var tx in items)
            {
                WriteULongPascalString(tx.Name);

                byte state = 0;
                if (tx.UseStatic && tx.UseDynamic)
                    state = 4;
                else if (tx.UseStatic)
                    state = 1;
                else if (tx.UseDynamic)
                    state = 2;

                WriteByte(state);

                if (tx.UseStatic)
                {
                    WriteFloat(tx.Rotation);
                    WriteVector2F(tx.FixedShift);
                    WriteVector2F(tx.FixedScale);
                    WriteVector2F(tx.FixedCenter);
                }

                if (tx.UseDynamic)
                {
                    WriteFloat(tx.SpinFreq);
                    WriteVector2F(tx.ShiftFreq);
                    WriteVector2F(tx.ScaleFreq);
                    WriteVector2F(tx.Scale);
                    WriteVector2F(tx.Center);
                }
            }
        }

        protected void WriteDynamicColors()
        {
            List<DynamicColor> colors = FindAllObjectsOfType<DynamicColor>();

            WriteUInt32(colors.Count);
            foreach (DynamicColor color in colors)
            {
                WriteULongPascalString(color.Name);

                for (int c = 0; c < 4; c++)
                {
                    DynamicColor.ChannelParams chan = color.Channels[c];


                    WriteFloat(chan.MinValue);
                    WriteFloat(chan.MinValue);

                    WriteUInt32(chan.Sinusoids.Count);

                    foreach (var p in chan.Sinusoids)
                    {
                        WriteFloat(p.Period);
                        WriteFloat(p.Offset);
                        WriteFloat(p.Weight);
                    }


                    WriteUInt32(chan.ClampUps.Count);
                    foreach (var p in chan.ClampUps)
                    {
                        WriteFloat(p.Period);
                        WriteFloat(p.Offset);
                        WriteFloat(p.Width);
                    }

                    WriteUInt32(chan.ClampDowns.Count);
                    foreach (var p in chan.ClampDowns)
                    {
                        WriteFloat(p.Period);
                        WriteFloat(p.Offset);
                        WriteFloat(p.Width);
                    }

                    WriteUInt32(chan.Sequence.Values.Count);
                    if (chan.Sequence.Values.Count > 0)
                    {
                        WriteFloat(chan.Sequence.Period);
                        WriteFloat(chan.Sequence.Offset);

                        foreach (var p in chan.Sequence.Values)
                            WriteByte(p);
                    }
                }
            }
        }

        private List<T> FindAllObjectsOfType<T>() where T : Map.Elements.BasicObject
        {
            List<T> objects = new List<T>();

            foreach (var i in MapData.Objects)
            {
                if (i.GetType() == typeof(T))
                    objects.Add(i as T);
            }
            return objects;
        }
    }
}
