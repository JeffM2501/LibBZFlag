using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Types;
using BZFlag.Data.Flags;

namespace BZFlag.Networking.Messages.BZFS
{
    public class MsgFlagUpdate : NetworkMessage
    {
        public class FlagUpdateData
        {
            public int FlagID = -1;
            public string Abreviation = string.Empty;
            public FlagStatuses Status = FlagStatuses.FlagNoExist;
            public FlagEndurances Endurance = FlagEndurances.FlagNormal;
            public int Owner = -1;
            public Vector3F Postion = Vector3F.Zero;
            public Vector3F LaunchPosition = Vector3F.Zero;
            public Vector3F LandingPostion = Vector3F.Zero;
            public float FlightTime = 0;
            public float FlightEnd = 0;
            public float InitalVelocity = 0;
        }

        public List<FlagUpdateData> FlagUpdates = new List<FlagUpdateData>();

        public MsgFlagUpdate()
        {
            Code = CodeFromChars("fu");
        }

        public override byte[] Pack()
        {
            throw new NotImplementedException();
        }

        public override void Unpack(byte[] data)
        {
            ResetOffset();

            int count = ReadUInt16(data);
            for (int i = 0; i < count; i++)
            {
                FlagUpdateData flag = new FlagUpdateData();
                flag.FlagID = ReadUInt16(data);
                flag.Abreviation = ReadFixedSizeString(data, 2);
                flag.Status = (FlagStatuses)ReadUInt16(data);
                flag.Endurance = (FlagEndurances)ReadUInt16(data);
                flag.Owner = ReadByte(data);
                flag.Postion = ReadVector3F(data);
                flag.LaunchPosition = ReadVector3F(data);
                flag.LandingPostion = ReadVector3F(data);
                flag.FlightTime = ReadFloat(data);
                flag.FlightEnd = ReadFloat(data);
                flag.InitalVelocity = ReadFloat(data);

				FlagUpdates.Add(flag);

			}
        }
    }
}
