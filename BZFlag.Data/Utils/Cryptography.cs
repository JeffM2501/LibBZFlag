using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace BZFlag.Data.Utils
{
    public class Cryptography
    {
        public static string MD5Hash(byte[] buffer)
        {
            MD5 hasher = MD5.Create();
            byte[] data = hasher.ComputeHash(buffer);

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
                sBuilder.Append(data[i].ToString("x2"));

            return sBuilder.ToString();
        }
    }
}
