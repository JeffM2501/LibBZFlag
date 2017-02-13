using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Game.Host.API
{
	public static class Functions
	{
		internal static Server ServerInstnace = null;

		public static BZFlag.Data.BZDB.Database GetServerBZDB()
		{
			return ServerInstnace.BZDatabase;
		}
	}
}
