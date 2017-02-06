using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Services
{
	public class GameInfo
	{
		public int GameOptions;
		public int GameType;
		public int MaxShots;
		public int ShakeWins;
		public int ShakeTimeout;      // 1/10ths of second
		public int MaxPlayerScore;
		public int MaxTeamScore;
		public int MaxTime;       // seconds
		public int MaxPlayers;
		public int RogueCount;
		public int RogueMax;
		public int RedCount;
		public int RedMax;
		public int GreenCount;
		public int GreenMax;
		public int BlueCount;
		public int BlueMax;
		public int PurpleCount;
		public int PurpleMax;
		public int ObserverCount;
		public int ObserverMax;

		public int TotalPlayers {  get { return RogueCount + RedCount + GreenCount + BlueCount + PurpleCount; } }


		private static byte[] shortBuffer = new byte[] { 0, 0 };
		private static byte[] longBuffer = new byte[] { 0, 0, 0, 0 };

		private static int offset = 0;


		private static int ReadUInt16(byte[] fromBuffer)
		{
			if(fromBuffer.Length < offset + 2)
				return 0;

			offset += 2;
			Array.Copy(fromBuffer, offset + 2, shortBuffer, 0, 2);
			if(BitConverter.IsLittleEndian)
				Array.Reverse(shortBuffer);

			return BitConverter.ToUInt16(shortBuffer, 0);
		}

		private static int ReadByte(byte[] fromBuffer)
		{
			if(fromBuffer.Length < offset + 1)
				return 0;

			offset += 1;
			return fromBuffer[offset - 1];
		}

		private static int WriteUInt16(int val, byte[] toBuffer)
		{
			return WriteUInt16((UInt16)val,toBuffer);
		}

		private static int WriteUInt16(UInt16 val, byte[] toBuffer)
		{
			if(toBuffer.Length < offset + 2)
				return -1;

			var bytes = BitConverter.GetBytes(val);
			if(BitConverter.IsLittleEndian)
				Array.Reverse(shortBuffer);

			Array.Copy(toBuffer, offset, bytes, 0, 2);
			offset += 2;

			return offset;
		}

		private static int WriteByte(int val, byte[] toBuffer)
		{
			return WriteByte((byte)val,toBuffer);
		}

		private static int WriteByte(byte val, byte[] toBuffer)
		{
			if(toBuffer.Length < offset + 1)
				return -1;
			toBuffer[offset] = val;
			offset++;
			return offset;
		}

		public bool ReadFromString(string data)
		{
			offset = 0;


			byte[] buffer = WebUtils.StringToByteArray(data);

			GameOptions = ReadUInt16(buffer);
			GameType = ReadUInt16(buffer);
			MaxShots = ReadUInt16(buffer);
			ShakeWins = ReadUInt16(buffer);
			ShakeTimeout = ReadUInt16(buffer);
			MaxPlayerScore = ReadUInt16(buffer);
			MaxTeamScore = ReadUInt16(buffer);
			MaxTime = ReadUInt16(buffer);

			MaxPlayers = ReadByte(buffer);
			RogueCount = ReadByte(buffer);
			RogueMax = ReadByte(buffer);
			RedCount = ReadByte(buffer);
			RedMax = ReadByte(buffer);
			GreenCount = ReadByte(buffer);
			GreenMax = ReadByte(buffer);
			BlueCount = ReadByte(buffer);
			BlueMax = ReadByte(buffer);
			PurpleCount = ReadByte(buffer);
			PurpleMax = ReadByte(buffer);
			ObserverCount = ReadByte(buffer);
			ObserverMax = ReadByte(buffer);

			return true;
		}

		public string  WriteToString()
		{
			offset = 0;

			byte[] buffer = new byte[29];

			WriteUInt16(GameOptions,buffer);
			WriteUInt16(GameType, buffer);
			WriteUInt16(MaxShots, buffer);
			WriteUInt16(ShakeWins, buffer);
			WriteUInt16(ShakeTimeout, buffer);
			WriteUInt16(MaxPlayerScore, buffer);
			WriteUInt16(MaxTeamScore,buffer);
			WriteUInt16(MaxTime, buffer);

			WriteByte(MaxPlayers, buffer);
			WriteByte(RogueCount, buffer);
			WriteByte(RogueMax, buffer);
			WriteByte(RedCount, buffer);
			WriteByte(RedMax, buffer);
			WriteByte(GreenCount, buffer);
			WriteByte(GreenMax, buffer);
			WriteByte(BlueCount, buffer);
			WriteByte(BlueMax, buffer);
			WriteByte(PurpleCount, buffer);
			WriteByte(PurpleMax, buffer);
			WriteByte(ObserverCount, buffer);
			WriteByte(ObserverMax, buffer);

			return WebUtils.ByteArrayToString(buffer);
		}
	}
}
