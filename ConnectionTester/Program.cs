using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

using BZFlag.Networking;
using BZFlag.Networking.Messages;
using BZFlag.Networking.Flags;

using BZFlag.Networking.Messages.BZFS;
using BZFlag.Networking.Messages.BZFS.UDP;

namespace ConnectionTester
{
	class Program
	{
		public static Client client = null;

		public static int PlayerID = -1;

		public delegate void MessageHandler(NetworkMessage msg);
		public static Dictionary<int, MessageHandler> Handlers = new Dictionary<int, MessageHandler>();

		public static StreamWriter sw = new StreamWriter("log.txt");

		static void Main(string[] args)
		{
			RegisterHandlers();

			client = new Client();
			client.HostMessageReceived += Client_HostMessageReceived;
			client.TCPConnected += Client_TCPConnected;
			client.Startup("trinity.fairserve.net", 5153);

			while(true)
			{
				client.Update();
				Thread.Sleep(100);
			}
		}

		public static void WriteLine(string text)
		{
			Console.WriteLine(text);
			sw.WriteLine(text);
			sw.Flush();
		}

		public static void WriteLine()
		{
			WriteLine(string.Empty);
		}

		public static void Write(string text)
		{
			Console.Write(text);
			sw.Write(text);
		}

		private static void Client_TCPConnected(object sender, EventArgs e)
		{
		//	client.SendMessage(new MsgNegotiateFlags(BZFlag.Networking.Flags.FlagCache.FlagList.Keys));

			client.SendMessage(new MsgQueryGame());
			client.SendMessage(new MsgWantWHash());
		}

		static void RegisterHandlers()
		{
			Handlers.Add(new MsgQueryGame().Code, HandleMsgQueryGame);
			Handlers.Add(new MsgSuperKill().Code, HandleKilled);
			Handlers.Add(new MsgWantWHash().Code, HandleWorldHash);
			Handlers.Add(new MsgNegotiateFlags().Code, HandleNegotiateFlags);
			Handlers.Add(new MsgGameTime().Code, HandleGameTime);
			Handlers.Add(new MsgMessage().Code, HandleChatMessage);
			Handlers.Add(new MsgAccept().Code, HandleAcceptMessage);
			Handlers.Add(new MsgReject().Code, HandleRejectMessage);
			Handlers.Add(new MsgSetVars().Code, HandleSetVarsMessage);
			Handlers.Add(new MsgTeamUpdate().Code, HandleTeamUpdate);
			Handlers.Add(new MsgUDPLinkRequest().Code, HandleUDPLinkRequest);
		}

		private static void Client_HostMessageReceived(object sender, Client.HostMessageReceivedEventArgs e)
		{
			if(Handlers.ContainsKey(e.Message.Code))
				Handlers[e.Message.Code](e.Message);
			else if(e.Message as UnknownMessage != null)
			{
				UnknownMessage u = e.Message as UnknownMessage;

				byte[] codeBytes = BitConverter.GetBytes((UInt16)e.Message.Code);
				if(BitConverter.IsLittleEndian)
					Array.Reverse(codeBytes);
				string codeLetter = Encoding.ASCII.GetString(codeBytes);

				WriteLine("Message " + e.Message.Code.ToString() + "(" + codeLetter + ") size " + u.DataBuffer.Length.ToString());
				if(u.DataBuffer.Length > 0)
				{
					Write("Payload ");

					foreach(byte b in u.DataBuffer)
					{
						Write(b.ToString() + " ");
					}
					WriteLine();
				}
			}
			else
				WriteLine("Unknown Message " + e.Message.Code.ToString());

			WriteLine();
		}

		protected static void SendEnter()
		{
			var enter = new MsgEnter();
			enter.Callsign = "Billy D. Bugger";
			enter.Email = "Testing 1...2..3.";
			enter.Token = string.Empty;
			client.SendMessage(enter);
		}

		private static void HandleTeamUpdate(NetworkMessage msg)
		{
			MsgTeamUpdate upd = msg as MsgTeamUpdate;
			WriteLine("Teams Were Updated ");
			foreach(var t in upd.TeamUpdates)
				WriteLine("\t" + ((TeamColors)t.TeamID).ToString() + String.Format(" = {0} {1}/{2}",t.Size,t.Wins,t.Losses));
		}

		private static void HandleWorldHash(NetworkMessage msg)
		{
			MsgWantWHash hash = msg as MsgWantWHash;
			WriteLine("Received" + (hash.IsRandomMap ? " Random "  : " Normal ") + "WorldHash:" + hash.WorldHash);

			SendEnter();
		}

		private static void HandleGameTime(NetworkMessage msg)
		{
			MsgGameTime gt = msg as MsgGameTime;
			WriteLine("Received GameTime:" + gt.NetTime.ToString());
		}

		private static void HandleChatMessage(NetworkMessage msg)
		{
			MsgMessage t = msg as MsgMessage;
			WriteLine("Received Chat Message From:" + t.From.ToString() + " To " + t.To.ToString());
			WriteLine("\t" + t.MessageText);
		}

		private static void HandleKilled(NetworkMessage msg)
		{
			WriteLine("Received SuperKill, shutting down");
			client.Shutdown();
		}
		private static void HandleRejectMessage(NetworkMessage msg)
		{
			MsgReject reject = msg as MsgReject;

			WriteLine("Received Reject Message " + reject.ReasonCode.ToString());
			WriteLine("\tMessage " + reject.ReasonMessage);
			client.Shutdown();
		}

		private static void HandleAcceptMessage(NetworkMessage msg)
		{
			MsgAccept accept = msg as MsgAccept;
			WriteLine("Received Accept Message PlayerID is " + accept.PlayerID.ToString());

			PlayerID = accept.PlayerID;

			// start UDP Link
			client.ConnectToUDP();
			client.SendMessage(false, new MsgUDPLinkRequest(PlayerID));
		}

		private static void HandleUDPLinkRequest(NetworkMessage msg)
		{
			MsgUDPLinkRequest udp = msg as MsgUDPLinkRequest;

			if (udp.FromUDP)
			{

			}
			else
			{
			}
		}

		private static void HandleNegotiateFlags(NetworkMessage msg)
		{
			MsgNegotiateFlags flags = msg as MsgNegotiateFlags;
			if(flags.FlagAbrevs.Count > 0)
			{
				WriteLine("There were " + flags.FlagAbrevs.Count.ToString() + " flags that the client doesn't support");
				foreach(var s in flags.FlagAbrevs)
					WriteLine("\t" + s);
			}
			else
				WriteLine("Flag Negotiation Successful");
		}

		private static void HandleSetVarsMessage(NetworkMessage msg)
		{
			MsgSetVars vars = msg as MsgSetVars;
			WriteLine("BZDB Variables Were Set ");
			foreach(var v in vars.BZDBVariables)
				WriteLine("\t" + v.Key + " = " + v.Value);
		}

		private static void HandleMsgQueryGame(NetworkMessage msg)
		{
			MsgQueryGame g = msg as MsgQueryGame;
			WriteLine("MsgQueryGame " + g.Code.ToString());
			WriteLine("\tGame Style " + g.GameStyle.ToString() + " Elapsed Time " + g.ElapsedTime.ToString() );
			WriteLine("\tOptions " + g.GameOptions.ToString());
			WriteLine("\tMax Shots " + g.MaxShots.ToString() + " Max Players " + g.MaxPlayers.ToString());
			foreach(var t in g.TeamData)
			{
				WriteLine("\t" + t.Key.ToString() + " Players " + t.Value.Size.ToString() + "/" + t.Value.Max.ToString());
			}
			WriteLine("\tShake Wins " + g.ShakeWins.ToString() + " Shake Timeout " + g.ShakeTimeout.ToString());
			WriteLine("\tMax Player Score " + g.MaxTeamScore.ToString() + " Max Team Score " + g.MaxTeamScore.ToString());
		}
	}
}
