using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages.BZFS.Info
{
    // Not used by anything really
    public class MsgFetchResources : NetworkMessage
    {
        public enum ResourceTypes
        {
            Image = 0,
            Sound,
            Font,
            File,
            Unknown
        };

        public class Resource
        {
            public ResourceTypes ResType = ResourceTypes.Unknown;
            public string URL = string.Empty;

            // non-transmitted values
            public string LocalFile = string.Empty;
            public object Tag = null;
        }

        public List<Resource> Resources = new List<Resource>();


        public MsgFetchResources()
        {
            Code = CodeFromChars("fr");
        }

        public override byte[] Pack()
        {
            throw new NotImplementedException();
        }

        public override void Unpack(byte[] data)
        {
            Reset(data);

            int count = ReadUInt16();

            for (int i = 0; i < count; i++)
            {
                Resource res = new Resource();
                res.ResType = (ResourceTypes)ReadUInt16();
                res.URL = ReadUShortPascalString();

                Resources.Add(res);
            }
        }
    }
}
