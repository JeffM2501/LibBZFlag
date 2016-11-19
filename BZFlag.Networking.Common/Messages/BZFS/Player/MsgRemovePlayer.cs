using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BZFlag.Data.Utils;

namespace BZFlag.Networking.Messages.BZFS.Player
{
	public class MsgRemovePlayer : NetworkMessage
	{
		public int PlayerID = -1;

		public MsgRemovePlayer()
		{
			Code = CodeFromChars("rp");
		}

		public override byte[] Pack()
		{
			throw new NotImplementedException();
		}

		public override void Unpack(byte[] data)
		{
			Reset(data);
			PlayerID = ReadByte();
		}
	}
}
