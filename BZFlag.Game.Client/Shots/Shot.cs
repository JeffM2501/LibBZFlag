using System;

using BZFlag.Game.Players;
using BZFlag.Data.Types;
using BZFlag.Data.Teams;

namespace BZFlag.Game.Shots
{
	public class Shot : EventArgs
	{
		public int GlobalID = int.MinValue;
		public int BZFSShotID = int.MinValue;

        public bool Active = true;

		public Player Owner = null;

		public double TimeSent = 0;

		public Vector3F InitalPosition = Vector3F.Zero;
		public Vector3F InitalVelocity = Vector3F.Zero;

		public float DeltaTime = 0;

		public TeamColors Team = TeamColors.NoTeam;
		public string Flag = string.Empty;

		public float Lifetime = float.MinValue;

		public Vector3F Position = Vector3F.Zero;
		public Vector3F Velocity = Vector3F.Zero;

		public double LastUpdate = 0;
		public Player Target = null;

        public ShotPath Path = null;

        protected ShotPath.Segment CurrentSegment = null;
        public void Update(double time, double delta)
        {
            if (CurrentSegment != null && CurrentSegment.EndT >= time)
                CurrentSegment = null;

            if (CurrentSegment == null)
            {
                while (CurrentSegment != null)
                {
                    foreach(ShotPath.Segment seg in Path.Segments)
                    {
                        if (seg.StartT <= time && seg.EndT >= time)
                        {
                            CurrentSegment = seg;
                            break;
                        }
                    }
                    break;
                }
            }

            if (CurrentSegment == null)
            {
                Active = false;
                return;
            }

            double tDelta = CurrentSegment.EndT - CurrentSegment.StartT;
            double tParam = (time - CurrentSegment.StartT) / tDelta;
            Vector3F vecDelta = CurrentSegment.EndPoint = CurrentSegment.StartPoint;

            Position = CurrentSegment.StartPoint + (vecDelta * tParam);

        }
	}
}
