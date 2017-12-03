using System;
using System.Collections.Generic;

using BZFlag.Map.Elements;
using BZFlag.Map.Elements.Shapes;

using Jitter;
using Jitter.Collision;
using Jitter.Dynamics;
using Jitter.Collision.Shapes;
using Jitter.LinearMath;

namespace BZFlag.Map
{
    public class WorldMap
    {
        public class PhysicalConstants
        {
            public float Gravity = -9.7f;
        }

        public PhysicalConstants Constants = new PhysicalConstants();


        public List<BasicObject> Objects = new List<BasicObject>();

        public World WorldInfo = new World();
        public Options WorldOptions = new Options();

        public JitterWorld PhysicsWorld = null;

        public void IntForLoad()
        {
            Teleporter.TeleporterCount = 0;
        }

        protected List<Teleporter> TeleporterCache = new List<Teleporter>();

        public void FinishLoad()
        {
            // check to see if all the teleporters have names and if not fix any links

            foreach (BasicObject obj in Objects)
            {
                Teleporter tp = obj as Teleporter;
                if (tp == null)
                    continue;

                TeleporterCache.Add(tp);

                if (tp.Name == string.Empty)
                    tp.Name = "teleporter_" + tp.Index.ToString();
            }

            PhysicsWorld = new JitterWorld(new CollisionSystemPersistentSAP());
            PhysicsWorld.Gravity = new JVector(0,0,-9.81f);

            foreach (BasicObject obj in Objects)
            {
                PositionableObject po = obj as PositionableObject;
                if (po == null)
                    continue;

                RigidBody body = new RigidBody(new BoxShape(new JVector(po.Size.X + 2,po.Size.Y + 2,po.Size.Z)));
                body.IsStatic = true;
                body.Position = new JVector(po.Position.X, po.Position.Y, po.Position.Z + (po.Size.Z * 0.5f));
                body.Orientation = JMatrix.CreateRotationZ(po.Rotation * ((float)Math.PI/180.0f));
                body.Tag = po;

                PhysicsWorld.AddBody(body);
            }
        }

        public void CacheRuntimeObjects()
        {
            foreach (BasicObject obj in Objects)
            {
                Teleporter tp = obj as Teleporter;
                if (tp == null)
                    continue;

                TeleporterCache.Add(tp);
            }
        }

        public Teleporter GetTeleporterByID(int id)
        {
            if (id < 0 || id >= TeleporterCache.Count)
                return null;

            return TeleporterCache[id];
        }

        private BasicObject FindObjectByName(string name)
        {
            return Objects.Find((x) => x.Name.ToUpperInvariant() == name.ToUpperInvariant());
        }

        private Teleporter FindTeleporterByName(string name)
        {
            return Objects.Find((x) => x as Teleporter != null && x.Name.ToUpperInvariant() == name.ToUpperInvariant()) as Teleporter;
        }

        private Teleporter FindTeleporterFaceIndex(int index)
        {
            int porterID = (index / 2) + 1;

            return Objects.Find((x) => x as Teleporter != null && (x as Teleporter).Index == porterID) as Teleporter;
        }

        public void AddObject(BasicObject obj)
        {
            if (obj as World != null)
                WorldInfo = obj as World;
            else if (obj as Options != null)
                WorldOptions = obj as Options;
            else
                Objects.Add(obj);
        }

        public void AddObjects(IEnumerable<BasicObject> lst)
        {
            foreach (var o in lst)
                AddObject(o);
        }
    }
}
