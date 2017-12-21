using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Game;
using BZFlag.Data.Utils;

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
            DynamicOutputBuffer buffer = DynamicOutputBuffer.Get(Code);

            buffer.WriteFloat(WorldSize);
            buffer.WriteUInt16((int)GameType);
            buffer.WriteUInt16((int)GameOptions);
            buffer.WriteUInt16(MaxPlayers);
            buffer.WriteUInt16(MaxShots);
            buffer.WriteUInt16(MaxFlags);

            buffer.WriteFloat(LinearAcceleration);
            buffer.WriteFloat(AngularAcceleration);

            buffer.WriteUInt16((int)(ShakeTimeout / 0.1f));
            buffer.WriteUInt16(ShakeWins);

            buffer.WriteUInt32(UsedToBeSyncTime);

            return buffer.GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            Reset(data);

            WorldSize = ReadFloat();
            GameType = (GameTypes)ReadUInt16();
            GameOptions = (GameOptionFlags)ReadUInt16();
            MaxPlayers = ReadUInt16();
            MaxShots = ReadUInt16();
            MaxFlags = ReadUInt16();

            LinearAcceleration = ReadFloat();
            AngularAcceleration = ReadFloat();

            ShakeTimeout = 0.1f * ReadUInt16();
            ShakeWins = ReadUInt16();
            UsedToBeSyncTime = (int)ReadUInt32();
        }
    }
}
