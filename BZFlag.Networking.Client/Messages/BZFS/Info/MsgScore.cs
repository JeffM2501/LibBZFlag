﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages.BZFS.Info
{
    public class MsgScore : NetworkMessage
    {
        public int PlayerID = -1;
        public int Wins = 0;
        public int Losses = 0;
        public int TeamKills = 0;

        public MsgScore()
        {
            Code = CodeFromChars("sc");
        }
        public override byte[] Pack()
        {
            throw new NotImplementedException();
        }

        public override void Unpack(byte[] data)
        {
            Reset(data);
            PlayerID = ReadByte();
            Wins = ReadUInt16();
            Losses = ReadUInt16();
            TeamKills = ReadUInt16();
        }
    }
}
