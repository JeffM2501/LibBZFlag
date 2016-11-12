using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BZFlag.Data.Types;
using BZFlag.Data.Teams;

namespace BZFlag.Map.Elements
{
	public class GroupDefinition : BasicObject
	{
		public enum ObstacleTypes
		{
			wallType = 0,
			boxType,
			pyrType,
			baseType,
			teleType,
			meshType,
			arcType,
			coneType,
			sphereType,
			tetraType,
			ObstacleTypeCount,
		};

		public class GroupInstance
		{
			public string GroupDef = string.Empty;

			public string Name = string.Empty;
			public MeshTransform Transform = new MeshTransform();
			public bool ModifyTeam = false;
			public TeamColors Team;
			public bool ModifyColor = false;
			public Color4F Tint = Color4F.Empty;
			public bool ModifyPhysicsDriver = false;
			public int Phydrv;
			public bool ModifyMaterial = false;
			public int MaterialID = -1;
			public bool DriveThrough;
			public bool ShootThrough;
			public bool Ricochet;
			public Dictionary<int, int> MatMap = new Dictionary<int, int>();
		}
		public List<BasicObject> Obstacles = new List<BasicObject>();
		public List<GroupInstance> Instances = new List<GroupInstance>();
	}
}
