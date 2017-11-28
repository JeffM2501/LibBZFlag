using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;


namespace BZFlag.Services
{
    public class PublicServer
    {
        public string LastError = string.Empty;

        protected WebClient Client = null;

        public string Name = string.Empty;
        public string Address = string.Empty;
        public int Port = 5154;
        public string Version = string.Empty;
        public string Description = string.Empty;

        public string Key = string.Empty;
        public string AdvertGroups = string.Empty;

        public GameInfo Info = new GameInfo();

        public event EventHandler RequestCompleted = null;
        public event EventHandler RequestErrored = null;

        public DateTime LastUpdate { get; protected set; }

        protected bool UpdateInProgress = false;

        public PublicServer()
        {
            LastUpdate = DateTime.MinValue;
        }

        public void UpdateMasterServer()
        {
            if (UpdateInProgress)
                return;

            UpdateInProgress = true;
            StringBuilder args = new StringBuilder();
            args.Append("action=ADD");
            args.Append(string.Format("&nameport={0}:{1}", Address, Port));
            args.Append(string.Format("&version={0}", Hosts.BZFSVersion));
            args.Append(string.Format("&gameinfo={0}", Info.WriteToString()));
            args.Append(string.Format("&build={0}", HttpUtility.UrlEncode(Version)));
            args.Append(string.Format("&title={0}", HttpUtility.UrlEncode(Description)));

            if (Key != string.Empty)
                args.Append(string.Format("&key={0}", Key));

            if (AdvertGroups != string.Empty)
                args.Append(string.Format("&advertgroups={0}", AdvertGroups));

            Client = new WebClient();
            Client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            Client.UploadStringCompleted += Client_UploadStringCompleted;
            Client.UploadStringAsync(new Uri(Hosts.ListServerURL), "POST", args.ToString());
        }

        private void Client_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            LastError = string.Empty;

            if (e.Error != null)
            {
                LastError = e.Error.ToString();
                if (RequestErrored != null)
                    RequestErrored.Invoke(this, EventArgs.Empty);

                return;
            }

            try
            {
                int count = 0;
                foreach (string line in e.Result.Split("\r\n".ToCharArray()))
                {
                    if (line == string.Empty)
                        continue;


                    string[] cmdParts = line.Split(":".ToCharArray(), 2);
                    string command = cmdParts[0];

                    if (command == "ERROR")
                        LastError = line;
                    else if (command == "MSG")
                    {

                    }
                    count++;

                }
            }
            catch (System.Exception ex)
            {
                //LastToken = string.Empty;
                LastError = ex.ToString();
            }


            if (LastError != string.Empty)
            {
                if (RequestErrored != null)
                    RequestErrored.Invoke(this, EventArgs.Empty);
            }
            else
            {
                LastUpdate = DateTime.Now;
                if (RequestCompleted != null)
                    RequestCompleted.Invoke(this, EventArgs.Empty);
            }

            UpdateInProgress = false;
        }

    }
}
