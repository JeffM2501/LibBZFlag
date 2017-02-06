using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Services
{
	public static class Hosts
	{
		public static readonly string DefaultListServer = "https://my.bzflag.org/db/";
		public static string ListServerURL = DefaultListServer;

		public static readonly string DefaultBZFSVersion = "BZFS0221";
		public static string BZFSVersion = DefaultBZFSVersion;

		public static string ApplicationVersion = "2.4.99." + DateTime.Now.ToShortDateString() + "-Managed_Debug-" + "AnyCPU";

	}
}
