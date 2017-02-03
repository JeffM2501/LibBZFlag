﻿using BZFlag.Data.Flags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Utils;

namespace BZFlag.Networking.Messages.BZFS.Flags
{
	public class MsgGrabFlag : NetworkMessage
	{
		public int PlayerID = -1;
		public FlagUpdateData FlagData = new FlagUpdateData();


		public MsgGrabFlag()
		{
			Code = CodeFromChars("gf");
		}

		public override byte[] Pack()
		{
			DynamicOutputBuffer buffer = new DynamicOutputBuffer(Code);

			buffer.WriteByte(PlayerID);
			buffer.WriteFlagUpdateData(FlagData);

			return buffer.GetMessageBuffer();
		}

		public override void Unpack(byte[] data)
		{
			Reset(data);

			PlayerID = ReadByte();
			FlagData = ReadFlagUpdateData();
		}
	}
}