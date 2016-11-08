using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;

namespace BZFlag.Authentication
{
    public class ServiceLink
    {
		public WebClient Client = null;

		public string LastToken = string.Empty;
		public string LastError = string.Empty;

		public static readonly string DefaultListServer = "https://my.bzflag.org/db/";
		public static string ListServerURL = DefaultListServer;

		public static readonly string DefaultBZFSVersion = "BZFS0221";
		public static string BZFSVersion = DefaultBZFSVersion;

		public class ListServerData
		{
			public string Name = string.Empty;
			public string Address = string.Empty;
			public string Version = string.Empty;
			public string Description = string.Empty;
			public string DataBits = string.Empty;

            public int  GameOptions;
            public int  GameType;
            public int  MaxShots;
            public int  ShakeWins;
            public int  ShakeTimeout;      // 1/10ths of second
            public int  MaxPlayerScore;
            public int  MaxTeamScore;
            public int  MaxTime;       // seconds
            public int  MaxPlayers;
            public int  RogueCount;
            public int  RogueMax;
            public int  RedCount;
            public int  RedMax;
            public int  GreenCount;
            public int  GreenMax;
            public int  BlueCount;
            public int  BlueMax;
            public int  PurpleCount;
            public int  PurpleMax;
            public int  ObserverCount;
            public int  ObserverMax;

            public int TotalPlayers = 0;

            public string Host = string.Empty;
            public int Port = -1;

            private static byte[] shortBuffer = new byte[] { 0, 0 };
            private static byte[] longBuffer = new byte[] { 0, 0, 0, 0 };

            private static int readOffset = 0;

            public ListServerData() { }
            public ListServerData( string host, int port)
            {
                Name = host + ":" + port.ToString();
                Address = Host;
                Host = host;
                Port = port;
            }

            private static int ReadUInt16(byte[] fromBuffer)
            {
                if (fromBuffer.Length < readOffset + 2)
                    return 0;

                readOffset += 2;
                Array.Copy(fromBuffer, readOffset+2, shortBuffer, 0, 2);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(shortBuffer);

                return BitConverter.ToUInt16(shortBuffer, 0);
            }

            private static int ReadByte(byte[] fromBuffer)
            {
                if (fromBuffer.Length < readOffset + 1)
                    return 0;

                readOffset += 1;
                return fromBuffer[readOffset - 1];
            }

            public void ProcessDataBits()
            {
                readOffset = 0;

                if (DataBits == string.Empty)
                    return;

                if (Address != string.Empty)
                {
                    string[] tmp = Name.Split(":".ToCharArray(), 2);
                    Host = tmp[0];
                    int.TryParse(tmp[1], out Port);
                }

                byte[] buffer = WebUtils.StringToByteArray(DataBits);

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

                TotalPlayers = RogueCount + RedCount + GreenCount + BlueCount + PurpleCount;
            }
        }

		public List<ListServerData> ServerList = new List<ListServerData>();

		public event EventHandler RequestCompleted = null;
		public event EventHandler RequestErrored = null;

		public void GetList(string callsign, string password)
		{
			string p = string.Format("action=LIST&vesion={0}&callsign={1}&pasword={2}", BZFSVersion, HttpUtility.UrlEncode(callsign), HttpUtility.UrlEncode(password));
			Client = new WebClient();
			Client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
			Client.UploadStringCompleted += Client_UploadStringCompleted;
			Client.UploadStringAsync(new Uri(ListServerURL),"POST", p);
		}

		public void GetToken(string callsign, string password)
		{
			string p = string.Format("action=GETTOKEN&vesion={0}&callsign={1}&pasword={2}", BZFSVersion, HttpUtility.UrlEncode(callsign), HttpUtility.UrlEncode(password));
			Client = new WebClient();
			Client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
			Client.UploadStringCompleted += Client_UploadStringCompleted;
			Client.UploadStringAsync(new Uri(ListServerURL), p);
		}

        public ListServerData FindServerWithMostPlayers()
        {
            ListServerData s = null;
            foreach(var p in ServerList)
            {
                if (s == null || p.TotalPlayers > s.TotalPlayers)
                    s = p;
            }
            return s;
        }

        private void Client_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
		{
			LastError = string.Empty;
			LastToken = string.Empty;

			if (e.Error != null)
			{
				LastError = e.Error.ToString();
				if(RequestErrored != null)
					RequestErrored.Invoke(this, EventArgs.Empty);

				return;
			}

			ServerList.Clear();

			try
			{
				foreach(string line in e.Result.Split("\r\n".ToCharArray()))
				{
					if(line == string.Empty)
						continue;

					string[] cmdParts = line.Split(":".ToCharArray(), 2);
					string command = cmdParts[0];
					if(command == "TOKEN")
					{
						LastToken = cmdParts[1].Trim();
					}
					else if(command == "NOTOK")
					{
						LastError = "No Token " + cmdParts[1].Trim();
					}
					else if(command == "ERROR")
					{
						LastError = "ERROR " + cmdParts[1].Trim();
					}
					else if(command == "NOTICE")
					{
						LastError = "NOTICE " + cmdParts[1].Trim();
					}
					else
					{
						ListServerData data = new ListServerData();

						string[] dataParts = line.Split(" ".ToCharArray(), 5);
						data.Name = dataParts[0];
						data.Version = dataParts[1];
						data.DataBits = dataParts[2];
						data.Address = dataParts[3];
						if(dataParts.Length > 4)
							data.Description = dataParts[4];

                        data.ProcessDataBits();

                        ServerList.Add(data);
					}
				}
			}
			catch (System.Exception ex)
			{
				//LastToken = string.Empty;
				LastError = ex.ToString();
			}
			

			if(LastError != string.Empty)
			{
				if(RequestErrored != null)
					RequestErrored.Invoke(this, EventArgs.Empty);
			}
			else
			{
				if(RequestCompleted != null)
					RequestCompleted.Invoke(this, EventArgs.Empty);
			}
		}
	}
}
