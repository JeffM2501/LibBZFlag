using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages.BZFS
{
	public class MsgWantWHash : NoPackedDataNetworkMessage
	{
		public bool IsRandomMap = false;

		public string WorldHash = string.Empty;

		public MsgWantWHash()
		{
			Code = CodeFromChars("wh");
		}

		public override void Unpack(byte[] data)
		{
			string t = Encoding.UTF8.GetString(data);
			if(t.Length > 0)
			{
				IsRandomMap = t[0] == 't';
				WorldHash = t.Substring(1).TrimEnd('\0');
			}
		}
	}
}
