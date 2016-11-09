using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages.BZFS.Info
{
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
            ResetOffset();

            int count = ReadUInt16(data);

            for(int i =0; i < count; i++)
            {
                Resource res = new Resource();
                res.ResType = (ResourceTypes)ReadUInt16(data);
                res.URL = ReadUShortPascalString(data);

                Resources.Add(res);
            }
        }
    }
}
