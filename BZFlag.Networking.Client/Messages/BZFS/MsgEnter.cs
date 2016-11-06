using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Networking.Messages.BZFS
{
	public class MsgEnter : NetworkMessage
	{
		public UInt16 PlayerType = 0;
		public Int16 PlayerTeam = -2;

		public string Callsign = string.Empty;
		public string Email = string.Empty;
		public string Token = string.Empty;
		public string Version = "2.4.9";

		public MsgEnter()
		{
			Code = CodeFromChars("en");

			Version += DateTime.Now.Year.ToString();
			Version += DateTime.Now.Month.ToString("D2");
			Version += DateTime.Now.Day.ToString("D2");
			Version += "-DEVEL";
			Version += "-" + System.Environment.OSVersion.Platform.ToString();
			Version += "VC14mgd";
			Version += "-CLI";
		}

		public override byte[] Pack()
		{
			DynamicOutputBuffer buffer = new DynamicOutputBuffer(Code);

			buffer.WriteUInt16(PlayerType);
			buffer.WriteInt16(PlayerTeam);

			buffer.WriteFixedSizeString(Callsign, 32);
			buffer.WriteFixedSizeString(Email, 128);
			buffer.WriteFixedSizeString(Token, 22);
			buffer.WriteFixedSizeString(Version, 60);

			return buffer.GetMessageBuffer();
		}

		public override void Unpack(byte[] data)
		{
			
		}
	}
}
