using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Utils;

namespace BZFlag.Networking.Messages.BZFS.Info
{
    public class MsgScore : NetworkMessage
    {
        public class ScoreData
        {
            public int PlayerID = BZFlag.Data.Players.PlayerConstants.InvalidPlayerID;
            public int Wins = 0;
            public int Losses = 0;
            public int TeamKills = 0;

            public void Pack (DynamicOutputBuffer buffer)
            {
                buffer.WriteByte(PlayerID);
                buffer.WriteUInt16(Wins);
                buffer.WriteUInt16(Losses);
                buffer.WriteUInt16(TeamKills);
            }

            public static ScoreData Unpack(DynamicBufferReader buffer)
            {
                ScoreData data = new ScoreData();

                data.PlayerID = buffer.ReadByte();
                data.Wins = buffer.ReadUInt16();
                data.Losses = buffer.ReadUInt16();
                data.TeamKills = buffer.ReadUInt16();

                return data; 
            }
        }

        public List<ScoreData> Scores = new List<ScoreData>();


        public MsgScore()
        {
            Code = CodeFromChars("sc");
        }

        public override byte[] Pack()
        {
            if (Scores.Count == 0)
                return null;

            var buffer = DynamicOutputBuffer.Get(Code);

            buffer.WriteByte(Scores.Count);
            foreach (var score in Scores)
                score.Pack(buffer);

            return buffer.GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            Reset(data);
            int count = ReadByte();

            for (int i = 0; i < count; i++)
                Scores.Add(ScoreData.Unpack(this));
            
        }
    }
}
