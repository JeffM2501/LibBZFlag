using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Teams;
using BZFlag.Data.Types;
using BZFlag.Data.Utils;

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
			Reset(data);
			PlayerID = ReadByte();
			PlayerType = ReadUInt16();
			Team = (TeamColors)ReadUInt16();
			Wins = ReadUInt16();
			Losses = ReadUInt16();
			TeamKills = ReadUInt16();
			Callsign = ReadFixedSizeString(Constants.CallsignLen);
			Motto = ReadFixedSizeString(Constants.MottoLen);
		}
	}
}
