using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages.BZFS
{
	public abstract class NoPackedDataNetworkMessage : NetworkMessage
	{
		public override byte[] Pack()
		{
			return new DynamicOutputBuffer(Code).GetMessageBuffer();
		}

		public override void Unpack(byte[] data)
		{
		}
	}
}
