using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages.BZFS.Control
{
	public class MsgReplayReset : NetworkMessage
	{
		public int LastPlayer = -1;

		public MsgReplayReset()
		{
			Code = CodeFromChars("rr");
		}

		public override byte[] Pack()
		{
			throw new NotImplementedException();
		}

		public override void Unpack(byte[] data)
		{
			Reset(data);
			LastPlayer = ReadByte();
		}
	}
}
