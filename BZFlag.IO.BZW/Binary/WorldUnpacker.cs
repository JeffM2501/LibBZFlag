using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

using BZFlag.Data.Types;
using BZFlag.Data.Utils;
using BZFlag.Data.Teams;
using BZFlag.Map;
using BZFlag.Map.Elements;
using BZFlag.Map.Elements.Shapes;

namespace BZFlag.IO.BZW.Binary
{
    public class WorldUnpacker : DynamicBufferReader
    {
        public static readonly UInt16 MapVersion = 1;

		public List<string> Errors = new List<string>();
		public List<string> Warnings = new List<string>();

		protected void Error(string err)
		{
			Errors.Add(err);
		}

		protected void Warning(string warn)
		{
			Warnings.Add(warn);
		}

		public void AddData(byte[] b)
        {
            int oldSize = Buffer.Length;
            Array.Resize(ref Buffer, oldSize + b.Length);
            Array.Copy(b, 0, Buffer, oldSize, b.Length);
        }

        public WorldMap Unpack()
        {
            BufferOffset = 0;
            WorldMap map = new WorldMap();

            int len = ReadUInt16();
            ushort code = ReadUInt16();

            if (code != Constants.WorldCodeHeader)
                return null;

            int Version = ReadUInt16();
            if (Version != MapVersion)
                return null;

            UInt32 uncompressedSize = ReadUInt32();
            UInt32 compressedSize = ReadUInt32();

            UInt16 zlibFlags = ReadUInt16(); // deflate doesnt need these

            try
            {
                MemoryStream ms = new MemoryStream(Buffer, BufferOffset, (int)compressedSize);
                DeflateStream dfs = new DeflateStream(ms, CompressionMode.Decompress);

                byte[] uncompressedData = new byte[uncompressedSize];
                dfs.Read(uncompressedData, 0, (int)uncompressedSize);
                dfs.Close();
                ms.Close();

                Buffer = uncompressedData;
                BufferOffset = 0;

			    // start parsin that sweet sweet world data
			    map.AddObjects(UnpackDynamicColors());
			    map.AddObjects(UnpackTextureMatricies());
			    map.AddObjects(UnpackMaterials());
			    map.AddObjects(UnpackPhysicsDrivers());
			    map.AddObjects(UnpackTransforms());
			    map.AddObjects(UnpackObstacles());
			    map.AddObjects(UnpackLinks());
			    map.AddObject(UnpackWaterLevel());
			    map.AddObjects(UnpackWorldWeapons());
			    map.AddObjects(UnpackZones());
            }
            catch (Exception /*ex*/)
            {

            }

            return map;
        }

		protected List<BasicObject> UnpackZones()
		{
			List<BasicObject> items = new List<BasicObject>();

			UInt32 count = ReadUInt32();

			for(int i = 0; i < count; i++)
			{
				Zone z = new Zone();
				items.Add(z);

				z.Position = ReadVector3F();
				z.Size = ReadVector3F();
				z.Rotation = ReadFloat();

				int c = ReadUInt16();
				for(int j = 0; j < c; j++)
					z.Flags.Add(ReadFixedSizeString(2));

				c = ReadUInt16();
				for(int j = 0; j < c; j++)
					z.Team.Add((TeamColors)ReadUInt16());

				c = ReadUInt16();
				for(int j = 0; j < c; j++)
					z.Safe.Add((TeamColors)ReadUInt16());

			}
			return items;
		}

		protected List<BasicObject> UnpackWorldWeapons()
		{
			List<BasicObject> items = new List<BasicObject>();

			UInt32 count = ReadUInt32();

			for(int i = 0; i < count; i++)
			{
				WorldWeapon ww = new WorldWeapon();
				items.Add(ww);

				ww.Flag = ReadFixedSizeString(2);
				ww.Position = ReadVector3F();
				ww.Rotation = ReadFloat();
				ww.InitalDelay = ReadFloat();
				int c = ReadUInt16();
				for(int j =0; j < c; j++)
				{
					ww.DelayList.Add(ReadFloat());
				}
			}
			return items;
		}

		protected WaterLevel UnpackWaterLevel()
		{
			WaterLevel level = new WaterLevel();
			level.Height = ReadFloat();
			if(level.Height >= 0)
				level.MaterialID = ReadInt32();

			return level;
		}

		protected List<BasicObject> UnpackLinks()
		{
			List<BasicObject> items = new List<BasicObject>();

			UInt32 count = ReadUInt32();

			for(int i = 0; i < count; i++)
			{
				Link link = new Link();
				link.From.TargetName = ReadULongPascalString();
				link.To.TargetName = ReadULongPascalString();

				items.Add(link);
			}

			return items;
		}

		protected List<BasicObject> UnpackObstacles()
		{
			List<BasicObject> items = new List<BasicObject>();

			items.Add(ParseWorldObject());

			// parse the fixed size world
			for(GroupDefinition.ObstacleTypes objType = GroupDefinition.ObstacleTypes.wallType; objType < GroupDefinition.ObstacleTypes.ObstacleTypeCount; objType++)
				items.AddRange(UnpackObstacleList(objType));

			UInt32 count = ReadUInt32();

			for(int i = 0; i < count; i++)
			{
				GroupDefinition group = new GroupDefinition();
				items.Add(group);
				group.Name = ReadULongPascalString();

				for(GroupDefinition.ObstacleTypes objType = GroupDefinition.ObstacleTypes.wallType; objType < GroupDefinition.ObstacleTypes.ObstacleTypeCount; objType++)
					group.Obstacles.AddRange(UnpackObstacleList(objType));
			}
			count = ReadUInt32();

			for(int j = 0; j < count; j++)
			{
				GroupInstance instance = new GroupInstance();
				items.Add(instance);
				instance.GroupDef = ReadULongPascalString();
				instance.Name = ReadULongPascalString(); // this has some material mapping shit int in, TODO, extract it

				instance.Transform = UnpackMeshTransform();

				byte bits = ReadByte();

				instance.ModifyTeam = ((bits & (1 << 0)) == 0) ? false : true;
				instance.ModifyColor = ((bits & (1 << 1)) == 0) ? false : true;
				instance.ModifyPhysicsDriver = ((bits & (1 << 2)) == 0) ? false : true;
				instance.ModifyMaterial = ((bits & (1 << 3)) == 0) ? false : true;
				instance.DriveThrough = ((bits & (1 << 4)) == 0) ? false : true;
				instance.ShootThrough = ((bits & (1 << 5)) == 0) ? false : true;
				instance.Ricochet = ((bits & (1 << 6)) == 0) ? false : true;

				if(instance.ModifyTeam)
					instance.Team = (TeamColors)ReadUInt16();
				if(instance.ModifyColor)
					instance.Tint = ReadColor4F();
				if(instance.ModifyPhysicsDriver)
					instance.Phydrv = ReadInt32();
				if(instance.ModifyMaterial)
					instance.MaterialID = ReadInt32();
			}


			return items;
		}

        protected World ParseWorldObject()
        {
			World world = new World();
			world.Name = ReadULongPascalString();
			world.NoWalls = true;

			return world;
        }

		protected BasicObject[] UnpackObstacleList(GroupDefinition.ObstacleTypes objType)
		{
			List<BasicObject> items = new List<BasicObject>();

			UInt32 objects = ReadUInt32();
			for(int i = 0; i < objects; i++)
			{
				BasicObject obj = UnpackObjectByType(objType);
				if(obj != null)
					items.Add(obj);
			}

			return items.ToArray();
		}


		protected BasicObject UnpackObjectByType(GroupDefinition.ObstacleTypes objType)
		{
			switch(objType)
			{
				case GroupDefinition.ObstacleTypes.wallType:
					return UnpackWall();

				case GroupDefinition.ObstacleTypes.boxType:
					return UnpackBox();

				case GroupDefinition.ObstacleTypes.pyrType:
					return UnpackPyramid();

				case GroupDefinition.ObstacleTypes.baseType:
					return UnpackBase();

				case GroupDefinition.ObstacleTypes.teleType:
					return UnpackTeleporter();

				case GroupDefinition.ObstacleTypes.meshType:
					return UnpackMesh();

				case GroupDefinition.ObstacleTypes.arcType:
					return UnpackArc();

				case GroupDefinition.ObstacleTypes.coneType:
					return UnpackCone();

				case GroupDefinition.ObstacleTypes.sphereType:
					return UnpackSphere();

				case GroupDefinition.ObstacleTypes.tetraType:
					return UnpackTetra();

				default:
					return null;
			}
		}

		protected Tetra UnpackTetra()
		{
			Tetra o = new Tetra();

			byte stateByte = ReadByte();
			o.DriveThrough = (stateByte & (1 << 0)) != 0;
			o.ShootThrough = (stateByte & (1 << 1)) != 0;
			o.Ricochet = (stateByte & (1 << 2)) != 0;

			o.Transform = UnpackMeshTransform();


			for(int i = 0; i < 4; i++)
				o.Verts.Add(ReadVector3F());

			byte normalFlags = ReadByte();

			for(int i = 0; i < 4; i++)
			{
				Tetra.Face face = new Tetra.Face();
				o.Faces.Add(face);
				face.UseNormals = (normalFlags & (1 << i)) != 0;
				if (face.UseNormals)
				{
					for(int j = 0; j < 3; j++)
						face.Norms.Add(ReadVector3F());
				}
			}

			byte uvFlags = ReadByte();

			for(int i = 0; i < 4; i++)
			{
				Tetra.Face face = o.Faces[i];
				face.UseUVs = (uvFlags & (1 << i)) != 0;
				if(face.UseNormals)
				{
					for(int j = 0; j < 3; j++)
						face.UVs.Add(ReadVector2F());
				}
			}

			for(int i = 0; i < 4; i++)
			{
				Tetra.Face face = o.Faces[i];
				face.MaterialID = ReadInt32();
			}

			return o;
		}

		protected Sphere UnpackSphere()
		{
			Sphere o = new Sphere();
			o.Transform = UnpackMeshTransform();
			o.Position = ReadVector3F();
			o.Size = ReadVector3F();
			o.Rotation = ReadFloat();
			o.Divisions = ReadInt32();
			o.PhysicsID = ReadInt32();

			for(int i = 0; i < 2; i++)
				o.TextureSize[i] = ReadFloat();

			for(Sphere.MaterialLocations loc = Sphere.MaterialLocations.Edge; loc < Sphere.MaterialLocations.MaterialCount; loc++)
				o.MaterialIDs.Add(loc, ReadInt32());

			byte stateByte = ReadByte();
			o.DriveThrough = (stateByte & (1 << 0)) != 0;
			o.ShootThrough = (stateByte & (1 << 1)) != 0;
			o.SmoothBounce = (stateByte & (1 << 2)) != 0;
			o.UseNormals = (stateByte & (1 << 3)) != 0;
			o.Hemisphere = (stateByte & (1 << 4)) != 0;
			o.Ricochet = (stateByte & (1 << 5)) != 0;

			return o;
		}

		protected Cone UnpackCone()
		{
			Cone o = new Cone();
			o.Transform = UnpackMeshTransform();
			o.Position = ReadVector3F();
			o.Size = ReadVector3F();
			o.Rotation = ReadFloat();
			o.SweepAngle = ReadFloat();
			o.Divisions = ReadInt32();
			o.PhysicsID = ReadInt32();

			for(int i = 0; i < 2; i++)
				o.TextureSize[i] = ReadFloat();

			for(Cone.MaterialLocations loc = Cone.MaterialLocations.Edge; loc < Cone.MaterialLocations.MaterialCount; loc++)
				o.MaterialIDs.Add(loc, ReadInt32());

			byte stateByte = ReadByte();
			o.DriveThrough = (stateByte & (1 << 0)) != 0;
			o.ShootThrough = (stateByte & (1 << 1)) != 0;
			o.SmoothBounce = (stateByte & (1 << 2)) != 0;
			o.UseNormals = (stateByte & (1 << 3)) != 0;
			o.Ricochet = (stateByte & (1 << 4)) != 0;

			return o;
		}


		protected Arc UnpackArc()
		{
			Arc o = new Arc();
			o.Transform = UnpackMeshTransform();
			o.Position = ReadVector3F();
			o.Size = ReadVector3F();
			o.Rotation = ReadFloat();
			o.SweepAngle = ReadFloat();
			o.Ratio = ReadFloat();
			o.Divisions = ReadInt32();
			o.PhysicsID = ReadInt32();

			for(int i = 0; i < 4; i++)
				o.TextureSize[i] = ReadFloat();

			for(Arc.MaterialLocations loc = Arc.MaterialLocations.Top; loc < Arc.MaterialLocations.MaterialCount; loc++)
				o.MaterialIDs.Add(loc, ReadInt32());

			byte stateByte = ReadByte();
			o.DriveThrough = (stateByte & (1 << 0)) != 0;
			o.ShootThrough = (stateByte & (1 << 1)) != 0;
			o.SmoothBounce = (stateByte & (1 << 2)) != 0;
			o.UseNormals = (stateByte & (1 << 3)) != 0;
			o.Ricochet = (stateByte & (1 << 4)) != 0;

			return o;
		}

		protected Mesh UnpackMesh()
		{
			Mesh o = new Mesh();

			int count = ReadInt32();
			for(int i = 0; i < count; i++)
			{
				if(ReadByte() == 0)
					o.InsidePoints.Add(ReadVector3F());
				else
					o.OutsidePoints.Add(ReadVector3F());
			}

			count = ReadInt32();
			for(int i = 0; i < count; i++)
				o.Vertecies.Add(ReadVector3F());

			count = ReadInt32();
			for(int i = 0; i < count; i++)
				o.Normals.Add(ReadVector3F());

			count = ReadInt32();
			for(int i = 0; i < count; i++)
				o.UVs.Add(ReadVector2F());

			byte stateByte = 0;

			count = ReadInt32();
			for(int i = 0; i < count; i++)
			{
				Mesh.Face face = new Mesh.Face();

				stateByte = ReadByte();

				bool tmpNormals = (stateByte & (1 << 0)) != 0;
				bool tmpTexcoords = (stateByte & (1 << 1)) != 0;
				face.DriveThrough = (stateByte & (1 << 2)) != 0;
				face.ShootThrough = (stateByte & (1 << 3)) != 0;
				face.SmoothBounce = (stateByte & (1 << 4)) != 0;
				face.NoClusters = (stateByte & (1 << 5)) != 0;
				face.Ricochet = (stateByte & (1 << 6)) != 0;

				int t = ReadInt32();
				for(int j = 0; j < t; j++)
					face.Vertecies.Add(ReadInt32());

				if (tmpNormals)
				{
					t = ReadInt32();
					for(int j = 0; j < t; j++)
						face.Normals.Add(ReadInt32());
				}

				if(tmpTexcoords)
				{
					t = ReadInt32();
					for(int j = 0; j < t; j++)
						face.UVs.Add(ReadInt32());
				}

				face.MaterialID = ReadInt32();
				face.PhysicsDriverID = ReadInt32();
			}

			stateByte = ReadByte();


			o.DriveThrough = (stateByte & (1 << 0)) != 0;
			o.ShootThrough = (stateByte & (1 << 1)) != 0;
			o.SmoothBounce = (stateByte & (1 << 2)) != 0;
			o.NoClusters = (stateByte & (1 << 3)) != 0;
			bool drawInfoOwner = (stateByte & (1 << 4)) != 0;
			o.Ricochet = (stateByte & (1 << 5)) != 0;

			if(drawInfoOwner && (o.UVs.Count >= 2))
			{
				//  rewind the buffer, and re-parse, because the last set of UVs were bullshit and actually draw info data
			}

			return o;
		}

		protected Teleporter UnpackTeleporter()
		{
			Teleporter o = new Teleporter();
			o.Name = ReadULongPascalString();

			o.Position = ReadVector3F();
			o.Rotation = ReadFloat();
			o.Size = ReadVector3F();
			o.Border = ReadFloat();

			o.Horizontal = ReadByte() != 0;

			byte state = ReadByte();

			o.DriveThrough = (state & Constants.DRIVE_THRU) != 0;
			o.ShootThrough = (state & Constants.SHOOT_THRU) != 0;
			o.Ricochet = (state & Constants.RICOCHET) != 0;

			return o;
		}

		protected Base UnpackBase()
		{
			Base o = new Base();

			o.TeamColor = (TeamColors)ReadUInt16();

			o.Position = ReadVector3F();
			o.Rotation = ReadFloat();
			o.Size = ReadVector3F();
			byte state = ReadByte();

			o.DriveThrough = (state & Constants.DRIVE_THRU) != 0;
			o.ShootThrough = (state & Constants.SHOOT_THRU) != 0;
			o.Ricochet = (state & Constants.RICOCHET) != 0;
			
			return o;
		}

		protected Pyramid UnpackPyramid()
		{
			Pyramid o = new Pyramid();

			o.Position = ReadVector3F();
			o.Rotation = ReadFloat();
			o.Size = ReadVector3F();
			byte state = ReadByte();

			o.DriveThrough = (state & Constants.DRIVE_THRU) != 0;
			o.ShootThrough = (state & Constants.SHOOT_THRU) != 0;
			o.Ricochet = (state & Constants.RICOCHET) != 0;
			o.FlipZ = (state & Constants.FLIP_Z) != 0;
			return o;
		}

		protected Box UnpackBox()
		{
			Box box = new Box();

			box.Position = ReadVector3F();
			box.Rotation = ReadFloat();
			box.Size = ReadVector3F();
			byte state = ReadByte();

			box.DriveThrough = (state & Constants.DRIVE_THRU) != 0;
			box.ShootThrough = (state & Constants.SHOOT_THRU) != 0;
			box.Ricochet = (state & Constants.RICOCHET) != 0;
			return box;
		}

		protected WallObstacle UnpackWall()
		{
			WallObstacle wall = new WallObstacle();

			wall.Position = ReadVector3F();
			wall.Rotation = ReadFloat();
			wall.Size = new Vector3F(0, ReadFloat(), ReadFloat());
			wall.Ricochet = (ReadByte() & Constants.RICOCHET) != 0;
			return wall;
		}

		protected MeshTransform UnpackMeshTransform()
		{
			MeshTransform t = new MeshTransform();

			t.Name = ReadULongPascalString();

			var xformCount = ReadUInt32();
			for(int x = 0; x < xformCount; x++)
			{
				MeshTransform.TransformData data = new MeshTransform.TransformData();

				data.XFormType = (MeshTransform.TransformType)ReadByte();
				if(data.XFormType == MeshTransform.TransformType.IndexTransform)
					data.Index = ReadInt32();
				else
				{
					data.Data = new Vector4F(ReadVector3F());
					if(data.XFormType == MeshTransform.TransformType.SpinTransform)
						data.Data.A = ReadFloat();
				}

				t.Transforms.Add(data);
			}
			return t;
		}

		protected List<BasicObject> UnpackTransforms()
		{
			List<BasicObject> items = new List<BasicObject>();

			UInt32 count = ReadUInt32();

			for(int i = 0; i < count; i++)
				items.Add(UnpackMeshTransform());

			return items;
		}

		protected List<BasicObject> UnpackPhysicsDrivers()
		{
			List<BasicObject> items = new List<BasicObject>();

			UInt32 count = ReadUInt32();

			for(int i = 0; i < count; i++)
			{
				Physics phy = new Physics();
				items.Add(phy);
				phy.Name = ReadULongPascalString();

				phy.Linear = ReadVector3F();
				phy.Angular = ReadVector3F();
				phy.Radial = ReadVector3F();

				phy.Slide = ReadFloat();
				phy.Death = ReadULongPascalString();

			}

			return items;
		}

		protected List<BasicObject> UnpackMaterials()
		{
			List<BasicObject> items = new List<BasicObject>();

			UInt32 count = ReadUInt32();

			for(int i = 0; i < count; i++)
			{
				Material mat = new Material();
				items.Add(mat);
				mat.Name = ReadULongPascalString();

				byte modeByte = ReadByte();
				mat.NoCulling = (modeByte & (1 << 0)) != 0;
				mat.NoSorting = (modeByte & (1 << 1)) != 0;
				mat.NoRadar = (modeByte & (1 << 2)) != 0;
				mat.NoShadow = (modeByte & (1 << 3)) != 0;
				mat.Occluder = (modeByte & (1 << 4)) != 0;
				mat.GroupAlpha = (modeByte & (1 << 5)) != 0;
				mat.NoLighting = (modeByte & (1 << 6)) != 0;

				mat.DynamicColorRefrence = ReadInt32();
				mat.Ambient = ReadColor4F();
				mat.Diffuse = ReadColor4F();
				mat.Specular = ReadColor4F();
				mat.Emission = ReadColor4F();
				mat.Shininess = ReadFloat();
				mat.AlphaThreshold = ReadFloat();

				int tCount = ReadByte();
				for(int t = 0; t < tCount; t++)
				{
					Material.TextureInfo tx = new Material.TextureInfo();
					tx.Name = ReadULongPascalString();
					tx.LocalName = tx.Name;
					tx.MatrixID = ReadInt32();
					tx.CombineMode = (Material.CombineModes)ReadInt32();
					byte stateByte = ReadByte();

					if((stateByte & (1 << 0)) != 0)
						tx.UseAlpha = true;
				
					if((stateByte & (1 << 1)) != 0)
						tx.UseColor = true;

					if((stateByte & (1 << 2)) != 0)
						tx.UseSphereMap = true;

					mat.Textures.Add(tx);
				}

				int sCount = ReadByte();
				for(int s = 0; s < sCount; s++)
					mat.Shaders.Add(ReadULongPascalString());
			}

			return items;
		}

		protected List<BasicObject> UnpackDynamicColors()
		{
			List<BasicObject> items = new List<BasicObject>();

			UInt32 count = ReadUInt32();

			for(int i = 0; i < count; i++)
			{
				DynamicColor color = new DynamicColor();
				items.Add(color);
				color.Name = ReadULongPascalString();
				for(int c =0; c < 4; c++)
				{
					DynamicColor.ChannelParams chan = color.Channels[c];

					chan.MinValue = ReadFloat();
					chan.MinValue = ReadFloat();

					UInt32 len = ReadUInt32();
					for(int s = 0; s < len; s++)
					{
						SinusoidParams p = new SinusoidParams();
						p.Period = ReadFloat();
						p.Offset = ReadFloat();
						p.Weight = ReadFloat();
						chan.Sinusoids.Add(p);
					}

					len = ReadUInt32();
					for(int s = 0; s < len; s++)
					{
						ClampParams p = new ClampParams();
						p.Period = ReadFloat();
						p.Offset = ReadFloat();
						p.Width = ReadFloat();
						chan.ClampUps.Add(p);
					}

					len = ReadUInt32();
					for(int s = 0; s < len; s++)
					{
						ClampParams p = new ClampParams();
						p.Period = ReadFloat();
						p.Offset = ReadFloat();
						p.Width = ReadFloat();
						chan.ClampDowns.Add(p);
					}

					len = ReadUInt32();
					if (len > 0)
					{
						chan.Sequence.Period = ReadFloat();
						chan.Sequence.Offset = ReadFloat();

						for(int s = 0; s < len; s++)
							chan.Sequence.Values.Add(ReadByte());
					}
				}
			}

			return items;
		}

		protected List<BasicObject> UnpackTextureMatricies()
		{
			List<BasicObject> items = new List<BasicObject>();

			UInt32 count = ReadUInt32();

			for(int i = 0; i < count; i++)
			{
				TextureMatrix tx = new TextureMatrix();
				items.Add(tx);
				tx.Name = ReadULongPascalString();

				byte state = ReadByte();
				tx.UseStatic = (state & (1 << 0)) != 0;
				tx.UseDynamic = (state & (1 << 1)) != 0;

				if (tx.UseStatic)
				{
					tx.Rotation = ReadFloat();
					tx.FixedShift = ReadVector2F();
					tx.FixedScale = ReadVector2F();
					tx.FixedCenter = ReadVector2F();
				}

				if (tx.UseDynamic)
				{
					tx.SpinFreq = ReadFloat();
					tx.ShiftFreq = ReadVector2F();
					tx.ScaleFreq = ReadVector2F();
					tx.Scale = ReadVector2F();
					tx.Center = ReadVector2F();
				}
			}

			return items;
		}
	}
}
