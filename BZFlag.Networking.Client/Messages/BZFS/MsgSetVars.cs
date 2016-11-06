using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages.BZFS
{
	public class MsgSetVars : NetworkMessage
	{
		public Dictionary<string, string> BZDBVariables = new Dictionary<string, string>();

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
			ResetOffset();

			int varCount = ReadUInt16(data);
			for (int i = 0; i < varCount; i++)
				BZDBVariables.Add(ReadPascalString(data), ReadPascalString(data));
		}
	}
}
