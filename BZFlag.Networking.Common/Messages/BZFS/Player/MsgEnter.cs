using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Types;
using BZFlag.Data.Players;
using BZFlag.Data.Teams;
using BZFlag.Data.Utils;

namespace BZFlag.Networking.Messages.BZFS.Player
{
    public class MsgEnter : NetworkMessage
    {
        public PlayerTypes PlayerType = PlayerTypes.TankPlayer;
        public TeamColors PlayerTeam = TeamColors.ObserverTeam;

        public string Callsign = string.Empty;
        public string Motto = string.Empty;
        public string Token = string.Empty;
        public string Version = "2.4.9";

        public MsgEnter()
        {
            Code = CodeFromChars("en");

            Version += DateTime.Now.Year.ToString();
            Version += DateTime.Now.Month.ToString("D2");
            Version += DateTime.Now.Day.ToString("D2");
            Version += "-DEVEL";
            Version += "-" + System.Environment.OSVersion.Platform.ToString();
            Version += "VC14mgd";
            Version += "-CLI";
        }

        public override byte[] Pack()
        {
            DynamicOutputBuffer buffer = DynamicOutputBuffer.Get(Code);

            buffer.WriteUInt16((UInt16)PlayerType);
            buffer.WriteInt16((Int16)PlayerTeam);

            buffer.WriteFixedSizeString(Callsign, Constants.CallsignLen);
            buffer.WriteFixedSizeString(Motto, Constants.MottoLen);
            buffer.WriteFixedSizeString(Token, Constants.TokenLen);
            buffer.WriteFixedSizeString(Version, Constants.VersionLen);

            return buffer.GetMessageBuffer();
        }

        public override void Unpack(byte[] data)
        {
            Reset(data);

            PlayerType = (PlayerTypes)ReadUInt16();
            PlayerTeam = (TeamColors)ReadInt16();

            Callsign = ReadFixedSizeString(Constants.CallsignLen);
            Motto = ReadFixedSizeString(Constants.MottoLen);
            Token = ReadFixedSizeString(Constants.TokenLen);
            Version = ReadFixedSizeString(Constants.VersionLen);
        }
    }
}
