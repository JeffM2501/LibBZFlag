using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BZFlag.Data.Teams;
using BZFlag.Data.Types;

namespace BZFlag.Networking.Messages.BZFS.Player
{
	public class MsgAddPlayer : NetworkMessage
	{
		public int PlayerID = -1;

		public int PlayerType = -1;
		public TeamColors Team = TeamColors.AutomaticTeam;
		public int Wins = 0;
		public int Losses = 0;
		public int TeamKills = 0;
		public string Callsign = string.Empty;
		public string Motto = string.Empty;

		public MsgAddPlayer()
		{
			Code = CodeFromChars("ap");
		}

		public override byte[] Pack()
		{
			throw new NotImplementedException();
		}

		public override void Unpack(byte[] data)
		{
			ResetOffset();
			PlayerID = ReadByte(data);
			PlayerType = ReadUInt16(data);
			Team = (TeamColors)ReadUInt16(data);
			Wins = ReadUInt16(data);
			Losses = ReadUInt16(data);
			TeamKills = ReadUInt16(data);
			Callsign = ReadFixedSizeString(data, Constants.CallsignLen);
			Motto = ReadFixedSizeString(data, Constants.MottoLen);
		}
	}
}
