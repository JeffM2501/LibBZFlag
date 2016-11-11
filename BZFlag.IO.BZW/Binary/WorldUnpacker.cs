using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

using BZFlag.Data.Utils;
using BZFlag.Map;

namespace BZFlag.IO.BZW.Binary
{
    public class WorldUnpacker : DynamicBufferReader
    {
        public static readonly UInt16 MapVersion = 1;

        public void AddData(byte[] b)
        {
            int oldSize = Buffer.Length;
            Array.Resize(ref Buffer, oldSize + b.Length);
            Array.Copy(b, 0, Buffer, oldSize, b.Length);
        }

        public WorldMap Unpack()
        {
            BufferOffset = 0;
            WorldMap map = new WorldMap();

            int len = ReadUInt16();
            ushort code = ReadUInt16();

            if (code != Constants.WorldCodeHeader)
                return null;

            int Version = ReadUInt16();
            if (Version != MapVersion)
                return null;

            UInt32 uncompressedSize = ReadUInt32();
            UInt32 compressedSize = ReadUInt32();

            MemoryStream ms = new MemoryStream(Buffer, BufferOffset, (int)compressedSize);
            DeflateStream dfs = new DeflateStream(ms, CompressionLevel.Optimal);

            byte[] uncompressedData = new byte[uncompressedSize];
            dfs.Read(uncompressedData, 0, (int)uncompressedSize);
            dfs.Close();
            ms.Close();

            Buffer = uncompressedData;
            BufferOffset = 0;

            // start parsin that sweet sweet world data

            return map;
        }
    }
}
