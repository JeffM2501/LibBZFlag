using BZFlag.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ServicesTest
{
    class Program
    {
        static bool Wait = false;
        static bool Error = false;

        static void Main(string[] args)
        {

            ClientToken tok = new ClientToken();
            tok.RequestCompleted += RequestCompleted;
            tok.RequestErrored += RequestErrored;

            string callsign = "CALLSIGN";
            string password = "password";
            string globalIP = "IP";

            Wait = true;
            tok.GetToken(callsign, password);

            while (Wait)
            {
                Thread.Sleep(10);
            }

            if (Error)
            {
                Console.WriteLine("Error getting token " + tok.LastError);
                return;
            }

            Console.WriteLine("Got token " + tok.LastToken);

            Wait = true;

            ClientTokenCheck chk = new ClientTokenCheck();
            chk.RequestCompleted += RequestCompleted;
            chk.RequestErrored += RequestErrored;

            List<string> groups = new List<string>();
            groups.Add("DEVELOPERS");
            groups.Add("Planning.Developers");
            groups.Add("A.GROUP.THAT.NEVER.EXISTS");

            chk.CheckToken(callsign, tok.LastToken, globalIP, groups);

            while (Wait)
            {
                Thread.Sleep(10);

            }

            if (Error)
            {
                Console.WriteLine("Error getting token " + chk.LastError);
                return;
            }

            Console.WriteLine("Got response OK" + chk.BZID);

            Console.ReadLine();
        }

        private static void RequestErrored(object sender, EventArgs e)
        {
            Wait = false;
            Error = true;
        }

        private static void RequestCompleted(object sender, EventArgs e)
        {
            Wait = false;
            Error = false;
        }
    }
}
