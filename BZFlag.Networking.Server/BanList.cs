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
			return TrivialLookupBan(Hostbans, host);
		}

		public BanRecord FindIPBan(string ip)
		{
			return TrivialLookupBan(IPBans, ip);
		}

		public void AddHostBans(IEnumerable<BanRecord> bans)
		{
			lock(Hostbans)
				Hostbans.AddRange(bans);
		}

		public void AddIPBans(IEnumerable<BanRecord> bans)
		{
			lock(IPBans)
				IPBans.AddRange(bans);
		}

		private BanRecord TrivialLookupBan(List<BanRecord> list, string text)
		{
			string lHost = text.ToLowerInvariant();
			// just do a simple lookup now
			lock(list)
				return list.Find(x => lHost.Substring(x.BanMask.Length) == x.BanMask);
		}
	}
}
