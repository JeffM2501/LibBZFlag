using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;

using BZFlag.Game.Host;
using BZFlag.Services;
using System.Net.Sockets;
using System.Net;

namespace ConnectionTester
{
    public static class SimpleHoster
    {
        public static bool Done = false;
        public static object locker = new object();

        public static TcpListener tcp;
        public static PublicServer ps;

        public static void Run(string[] args)
        {
            UDPConnectionManager udp = new UDPConnectionManager();

            tcp = new TcpListener(IPAddress.Any, 5154);
            tcp.Start();

            tcp.BeginAcceptTcpClient(TCPClientAccepted, null);

            udp.AllowAll = true;
            udp.OutOfBandUDPMessage += Udp_OutOfBandUDPMessage;
            udp.Listen(5154);

            UdpClient udpc = new UdpClient("127.0.0.1", 5154);
            udpc.Send(Encoding.ASCII.GetBytes("hello"), 5);


            ps = new PublicServer();
            ps.Address = "trials.hyperdrive.tech";
            ps.Port = 5154;
            ps.Description = "An unusable test server";
            ps.Version = BZFlag.Services.Hosts.ApplicationVersion;
            ps.Name = "trials.hyperdrive.tech";

            ps.Key = args[0];

            ps.RequestCompleted += Ps_RequestCompleted;
            ps.RequestErrored += Ps_RequestErrored;

            ps.UpdateMasterServer();

            bool b = false;
            while (!b)
            {
                lock (locker)
                    b = Done;
                Thread.Sleep(10);
            }

        }

        private static void TCPClientAccepted(IAsyncResult ar)
        {
            var data = tcp.EndAcceptTcpClient(ar);

            Console.WriteLine("TCP packet received from  " + data.ToString());
        }

        private static void Ps_RequestErrored(object sender, EventArgs e)
        {
            Console.WriteLine("List request failed " + ps.LastError);
            lock (locker)
                Done = true;
        }

        private static void Ps_RequestCompleted(object sender, EventArgs e)
        {
            Console.WriteLine("List request Complete " + ps.LastError);
            lock (locker)
                Done = true;
        }

        private static void Udp_OutOfBandUDPMessage(object sender, UDPConnectionManager.OutOfBandUDPEventArgs e)
        {
            Console.WriteLine("out of band packet from " + e.Source.ToString());
            Console.WriteLine(Encoding.ASCII.GetString(e.DataBuffer));
        }
    }
}
