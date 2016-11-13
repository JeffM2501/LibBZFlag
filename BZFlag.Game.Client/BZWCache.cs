using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using BZFlag.Map;
using BZFlag.IO.BZW.Binary;


namespace BZFlag.Game
{
	public class BZWCache
	{
		public DirectoryInfo Folder = null;

		public BZWCache(string f)
		{
			if (f != string.Empty)
			{
				Folder = new DirectoryInfo(f);
				if(!Folder.Exists)
					Folder = null;
			}
		}

		public bool CheckCacheForHash(string hash)
		{
			if(Folder == null)
				return false;

			return File.Exists(Path.Combine(Folder.FullName, hash + ".bzwc"));
		}

		public WorldMap ReadMapFromCache (string hash)
		{
			if(Folder == null)
				return null;

			FileInfo file = new FileInfo(Path.Combine(Folder.FullName, hash + ".bzwc"));
			if(!file.Exists)
				return null;

			WorldUnpacker unpacker = new WorldUnpacker();
			FileStream fs = file.OpenRead();
			byte[] bits = new byte[file.Length];
			if (fs.Read(bits, 0, bits.Length) != bits.Length)
			{
				fs.Close();
				return null;
			}
			fs.Close();
			unpacker.AddData(bits);
			return unpacker.Unpack();
		}

		public void SaveMapToCache(string hash, byte[] data)
		{
			if(Folder == null)
				return;

			FileInfo file = new FileInfo(Path.Combine(Folder.FullName, hash + ".bzwc"));
			if(!file.Exists)
				return;

			WorldUnpacker unpacker = new WorldUnpacker();
			FileStream fs = file.OpenWrite();

			fs.Write(data, 0, data.Length);
			
			fs.Close();
		}
	}
}
