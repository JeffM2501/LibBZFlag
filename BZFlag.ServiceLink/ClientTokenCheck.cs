using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BZFlag.Services
{
    public class ClientTokenCheck
    {
        protected WebClient Client = null;

        public object Tag = null;
        public string LastError = string.Empty;

        public bool OK = false;
        public bool NameRegistered = false;
        public string BZID = string.Empty;

        public List<string> Groups = new List<string>();

        public event EventHandler RequestCompleted = null;
        public event EventHandler RequestErrored = null;

        public void CheckToken(string callsign, string token, string address, List<string> groups)
        {
            string tCheck = string.Format("{0}@{1}={2}", HttpUtility.UrlEncode(callsign), address, HttpUtility.UrlEncode(token));

            StringBuilder groupArgs = new StringBuilder();
            foreach(var g in groups)
            {
                groupArgs.Append(HttpUtility.UrlEncode(g.ToUpperInvariant()));
                groupArgs.Append("%0D%0A");
            }

            string p = string.Format("action=CHECKTOKENS&checktokens={0}&groups={1}", tCheck, groupArgs.ToString());
            Client = new WebClient();
            Client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            Client.UploadStringCompleted += Client_UploadStringCompleted;
            Client.UploadStringAsync(new Uri(Hosts.ListServerURL), p);
        }

        private void Client_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            BZID = string.Empty;
            LastError = string.Empty;
            OK = false;
            NameRegistered = false;

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
                    if (line == string.Empty || LastError != string.Empty)
                        continue;

                    string[] cmdParts = line.Split(":".ToCharArray(), 2);
                    string command = cmdParts[0];
                    if (command.ToUpperInvariant() == "MSG:")
                        continue;

                    if (command == "TOKGOOD")
                    {
                        LastError = string.Empty;
                        OK = true;
                        NameRegistered = true;

                        string[] parts = cmdParts[1].Trim().Split(":".ToCharArray());

                        string[] desiredGroups = Groups.ToArray();

                        Groups.Clear();
                        foreach (string g in parts) // scan the groups to match the names up to the requested format, if not just blast it in.
                        {
                            string n = g;
                            foreach (var dg in desiredGroups)
                            {
                                if (dg.ToUpperInvariant() == g.ToUpperInvariant())
                                    n = dg;
                            }

                            Groups.Add(n);
                        }
                    }
                    else if (command == "TOKBAD")
                    {
                        NameRegistered = true;
                        LastError = "Bad Token";
                        OK = false;
                    }
                    else if (command == "UNK")
                    {
                        OK = false;
                        NameRegistered = true;
                    }
                    else if (command == "BZID")
                    {
                        if (cmdParts.Length == 1)
                            LastError = "Malformed BZID";
                        else
                        {
                            string[] parts = cmdParts[1].Trim().Split(" ".ToCharArray());
                            if (parts.Length == 0)
                                LastError = "Malformed BZID";
                            else
                                BZID = parts[0];
                        }  
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

            if (OK && BZID == string.Empty)
            {
                OK = false;
                LastError = "Missing BZID";
            }

            if (LastError != string.Empty)
            {
                OK = false;

                if (RequestErrored != null)
                    RequestErrored.Invoke(this, EventArgs.Empty);
            }
            else
            {
                OK = true;
                if (RequestCompleted != null)
                    RequestCompleted.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
