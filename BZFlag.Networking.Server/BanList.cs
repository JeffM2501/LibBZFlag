using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking
{
	public class BanList
	{
		public class BanRecord
		{
			public string BanMask = string.Empty;
			public string Reason = string.Empty;
			public string Source = string.Empty;
		}

		protected List<BanRecord> Hostbans = new List<BanRecord>();
		protected List<BanRecord> IPBans = new List<BanRecord>();

		public BanRecord FindHostBan(string host)
		{
			string lHost = host.ToLowerInvariant();
			// just do a simple lookup now
			return Hostbans.Find(x => lHost.Contains(x.BanMask));
		}

		public BanRecord FindIPBan(string ip)
		{
			string lip = ip.ToLowerInvariant();
			// just do a simple lookup now
			return IPBans.Find(x => lip.Contains(x.BanMask));
		}
	}
}
