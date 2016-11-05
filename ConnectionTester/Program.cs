using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using BZFlag.Networking;
using BZFlag.Networking.Messages;

namespace ConnectionTester
{
	class Program
	{
		static void Main(string[] args)
		{
			Client client = new Client();

			client.HostMessageReceived += Client_HostMessageReceived;
			client.Startup("trinity.fairserve.net", 5153);

			while(true)
			{
				client.Update();
				Thread.Sleep(100);
			}
		}

		private static void Client_HostMessageReceived(object sender, Client.HostMessageReceivedEventArgs e)
		{
			if (e.Message as UnknownMessage != null)
			{
				UnknownMessage u = e.Message as UnknownMessage;

				Console.WriteLine("Message " + e.Message.Code.ToString() + " size " + u.DataBuffer.Length.ToString());
				if (u.DataBuffer.Length > 0)
				{
					Console.Write("Payload ");

					foreach(byte b in u.DataBuffer)
					{
						Console.Write(b.ToString() + " ");
					}
					Console.WriteLine();
				}
				Console.WriteLine();
			}
		}
	}
}
