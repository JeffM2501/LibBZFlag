using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages.BZFS
{
	public class MsgEnter : NetworkMessage
	{
		public MsgEnter()
		{
			Code = CodeFromChars("en");
		}

		public override byte[] Pack()
		{
			return new byte[] { 0, 0, 0x65, 0x6e };
		}

		public override void Unpack(byte[] data)
		{
			
		}
	}
}
