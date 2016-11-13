using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Utils;
using BZFlag.Data.Players;

namespace BZFlag.Networking.Messages.BZFS.Player
{
	public class MsgPlayerInfo : NetworkMessage
	{
		public class PlayerInfoData
		{
			public int PlayerID = 0;
			public PlayerAttributes Attributes = PlayerAttributes.Unknown;
		}

		public List<PlayerInfoData> PlayerUpdates = new List<PlayerInfoData>();


		public MsgPlayerInfo()
		{
			Code = CodeFromChars("pb");
		}

		public override byte[] Pack()
		{
			throw new NotImplementedException();
		}

		public override void Unpack(byte[] data)
		{
			Reset(data);

			int count = ReadByte();
			for(int i =0; i < count; i++)
			{
				PlayerInfoData info = new PlayerInfoData();
				info.PlayerID = ReadByte();
				info.Attributes = (PlayerAttributes)ReadByte();

				PlayerUpdates.Add(info);
			}
		}
	}
}
