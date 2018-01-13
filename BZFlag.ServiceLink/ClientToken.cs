using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;

namespace BZFlag.Services
{
    public class ClientToken
    {
        protected WebClient Client = null;

        public string LastCallsign = string.Empty;
        public string LastToken = string.Empty;
        public string LastError = string.Empty;

        public event EventHandler RequestCompleted = null;
        public event EventHandler RequestErrored = null;


        public void GetToken(string callsign, string password)
        {
            string p = string.Format("action=GETTOKEN&version={0}&callsign={1}&password={2}", Hosts.BZFSVersion, HttpUtility.UrlEncode(callsign), HttpUtility.UrlEncode(password));
            Client = new WebClient();
            Client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            Client.UploadStringCompleted += Client_UploadStringCompleted;
            Client.UploadStringAsync(new Uri(Hosts.ListServerURL), p);
        }

        private void Client_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            LastError = string.Empty;
            LastToken = string.Empty;

            if (e.Error != null)
            {
                LastError = e.Error.ToString();
                if (RequestErrored != null)
                    RequestErrored.Invoke(this, EventArgs.Empty);

                return;
            }

            try
            {
                foreach (string line in e.Result.Split("\r\n".ToCharArray()))
                {
                    if (line == string.Empty)
                        continue;

                    string[] cmdParts = line.Split(":".ToCharArray(), 2);
                    string command = cmdParts[0];
                    if (command == "TOKEN")
                    {
                        LastToken = cmdParts[1].Trim();
                    }
                    else if (command == "NOTOK")
                    {
                        LastError = "No Token " + cmdParts[1].Trim();
                    }
                    else if (command == "ERROR")
                    {
                        LastError = "ERROR " + cmdParts[1].Trim();
                    }
                    else if (command == "NOTICE")
                    {
                        LastError = "NOTICE " + cmdParts[1].Trim();
                    }
                    else
                    {

                    }
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
                if (RequestCompleted != null)
                    RequestCompleted.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
