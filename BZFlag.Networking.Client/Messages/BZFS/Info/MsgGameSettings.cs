using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Game;

namespace BZFlag.Networking.Messages.BZFS.Info
{
    public class MsgGameSettings : NetworkMessage
    {
        public float WorldSize = 0;
        public GameTypes GameType = GameTypes.Unknown;
        public GameOptionFlags GameOptions = GameOptionFlags.NoStyle;
        public int MaxPlayers = 0;
        public int MaxShots = 0;
        public int MaxFlags = 0;
        public float LinearAcceleration = 0;
        public float AngularAcceleration = 0;

        public int ShakeWins = 0;
        public float ShakeTimeout = 0;

        public int UsedToBeSyncTime = 0;

        public MsgGameSettings()
        {
            Code = CodeFromChars("gs");
        }

        public override byte[] Pack()
        {
            throw new NotImplementedException();
        }

        public override void Unpack(byte[] data)
        {
            ResetOffset();

            WorldSize = ReadFloat(data);
            GameType = (GameTypes)ReadUInt16(data);
            GameOptions = (GameOptionFlags)ReadUInt16(data);
            MaxPlayers = ReadUInt16(data);
            MaxShots = ReadUInt16(data);
            MaxFlags = ReadUInt16(data);

            LinearAcceleration = ReadFloat(data);
            AngularAcceleration = ReadFloat(data);

            ShakeTimeout = 0.1f * ReadUInt16(data);
            ShakeWins = ReadUInt16(data);
            UsedToBeSyncTime = (int)ReadUInt32(data);
        }
    }
}
