using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking
{
	public class OutboundMessageBuffer
	{
		private List<byte[]> Outbound = new List<byte[]>();

		public byte[] Pop()
		{
			lock(Outbound)
			{
				if(Outbound.Count == 0)
					return null;

				byte[] b = Outbound[0];
				Outbound.RemoveAt(0);
				return b;
			}
		}

		public void Push(byte[] buffer)
		{
			lock(Outbound)
				Outbound.Add(buffer);
		}

		public void Clear()
		{
			lock(Outbound)
				Outbound.Clear();
		}
	}
}
