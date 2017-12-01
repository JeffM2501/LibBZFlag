using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages.BZFS.BZDB
{
    public class MsgSetVars : NetworkMessage
    {
        public Dictionary<string, string> BZDBVariables = new Dictionary<string, string>();

        public readonly static int CodeValue = 0x7376;

        public MsgSetVars()
        {
            Code = CodeFromChars("sv");
        }

        public override byte[] Pack()
        {
            throw new NotImplementedException();
        }

        public override void Unpack(byte[] data)
        {
            BZDBVariables.Clear();
            Reset(data);

            int varCount = ReadUInt16();
            for (int i = 0; i < varCount; i++)
            {
                string k = ReadPascalString();
                string v = ReadPascalString();
                if (BZDBVariables.ContainsKey(k))
                    BZDBVariables[k] = v;
                else
                    BZDBVariables.Add(k, v);
            }
        }
    }
}