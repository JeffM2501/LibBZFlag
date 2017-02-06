using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;

namespace BZFlag.Services
{
    public class GameList
    {
		protected WebClient Client = null;

        public string LastCallsign = string.Empty;
		public string LastToken = string.Empty;
		public string LastError = string.Empty;

		public class ListServerData
		{
			public string Name = string.Empty;
			public string Address = string.Empty;
			public string Version = string.Empty;
			public string Description = string.Empty;

			public GameInfo Info = new GameInfo();

            public string Host = string.Empty;
            public int Port = -1;

            public ListServerData() { }
            public ListServerData( string host, int port)
            {
                Name = host + ":" + port.ToString();
                Address = Host;
                Host = host;
                Port = port;
            }

            public override string ToString()
            {
                return Name + "(" + Info.TotalPlayers.ToString() + ")";
            }

        }

		public List<ListServerData> ServerList = new List<ListServerData>();

		public event EventHandler RequestCompleted = null;
		public event EventHandler RequestErrored = null;

		public void GetList(string callsign, string password)
		{
            LastCallsign = callsign;

            string p = string.Format("action=LIST&version={0}&callsign={1}&password={2}", Hosts.BZFSVersion, HttpUtility.UrlEncode(callsign), HttpUtility.UrlEncode(password));
			Client = new WebClient();
			Client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
			Client.UploadStringCompleted += Client_UploadStringCompleted;
			Client.UploadStringAsync(new Uri(Hosts.ListServerURL),"POST", p);
		}

        public ListServerData FindServerWithMostPlayers()
        {
            ListServerData s = null;
            foreach(var p in ServerList)
            {
                if (s == null || p.Info.TotalPlayers > s.Info.TotalPlayers)
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
						data.Info.ReadFromString(dataParts[2]);
						data.Address = dataParts[3];
						if(dataParts.Length > 4)
							data.Description = dataParts[4];

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
