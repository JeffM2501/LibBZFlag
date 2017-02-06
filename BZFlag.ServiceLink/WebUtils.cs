using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Services
{
    public static class WebUtils
    {
        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

		public static  string ByteArrayToString(byte[] bytes)
		{
			StringBuilder sb = new StringBuilder();
			foreach(byte b in bytes)
			{
				sb.Append(b.ToString("x2"));
			}
			return sb.ToString();
		}
	}
}
