using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

using BZFlag.Networking;
using BZFlag.Networking.Messages;

using BZFlag.Networking.Messages.BZFS;
using BZFlag.Networking.Messages.BZFS.UDP;
using BZFlag.Networking.Messages.BZFS.World;
using BZFlag.Networking.Messages.BZFS.BZDB;
using BZFlag.Networking.Messages.BZFS.Player;
using BZFlag.Networking.Messages.BZFS.Info;
using BZFlag.Networking.Messages.BZFS.Flags;
using BZFlag.Networking.Messages.BZFS.Shots;

using BZFlag.Data.Teams;
using BZFlag.Data.Types;
using BZFlag.Authentication;


namespace ConnectionTester
{
	class Program
	{
		public static Client client = null;

		public static int PlayerID = -1;

		public delegate void MessageHandler(NetworkMessage msg);
		public static Dictionary<int, MessageHandler> Handlers = new Dictionary<int, MessageHandler>();

		public static StreamWriter sw = new StreamWriter("log.txt");

		private static byte[] WorldData = new byte[0];

		private static string Callsign = "Billy D. Bugger";

		private static bool GetWorld = false;

		static void Main(string[] args)
		{
			Link.RequestCompleted += Link_RequestCompleted;
			Link.RequestErrored += Link_RequestErrored;

			if(args.Length > 0)
				Callsign = args[0];

			GetList(Callsign, args.Length > 1 ? args[1] : string.Empty);

			RegisterHandlers();

			client = new Client();
			client.HostMessageReceived += Client_HostMessageReceived;
			client.TCPConnected += Client_TCPConnected;

            var server = Link.FindServerWithMostPlayers();
            if (server == null || false)
                server = new ServiceLink.ListServerData("bzflag.allejo.io", 5147);

			client.Startup(server.Host,server.Port);

			while(true)
			{
				client.Update();
				Thread.Sleep(100);
			}
		}

		static bool GotList = false;
		private static ServiceLink Link = new ServiceLink();

		private static void GetList(string user, string pass)
		{
			Link.GetList(user, pass);

			while(GotList == false)
				Thread.Sleep(10);
		}

		private static void Link_RequestErrored(object sender, EventArgs e)
		{
			WriteLine("List Request Error " + Link.LastError);
			GotList = true;
		}

		private static void Link_RequestCompleted(object sender, EventArgs e)
		{
			WriteLine("List Request Completed " + Link.LastToken);
			WriteLine("List Server Count " + Link.ServerList.Count.ToString());
			GotList = true;
		}

		public static void WriteLine(string text)
		{
			Console.WriteLine(text);
			sw.WriteLine(text);
			sw.Flush();
		}

		public static void WriteEmptyLine()
		{
			Console.WriteLine();
			sw.WriteLine();
			sw.Flush();
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
            Handlers.Add(new MsgUDPLinkEstablished().Code, HandleUDPLinkEstablished);
            Handlers.Add(new MsgLagPing().Code, HandleLagPing);
			Handlers.Add(new MsgFlagUpdate().Code, HandleFlagUpdate);
			Handlers.Add(new MsgAddPlayer().Code, HandleAddPlayer);
			Handlers.Add(new MsgPlayerInfo().Code, HandlePlayerInfo);
			Handlers.Add(new MsgGetWorld().Code, HandleGetWorld);
            Handlers.Add(new MsgQueryPlayers().Code, HandleQueryPlayers);
            Handlers.Add(new MsgPlayerUpdate().Code, HandlePlayerUpdate);
            Handlers.Add(new MsgPlayerUpdateSmall().Code, HandlePlayerUpdate);
            Handlers.Add(new MsgScore().Code, HandleScoreUpdate);
            Handlers.Add(new MsgAlive().Code, HandleAlive);
            Handlers.Add(new MsgRemovePlayer().Code, HandleRemovePlayer);
			Handlers.Add(new MsgShotBegin().Code, HandleShotBegin);
			Handlers.Add(new MsgShotEnd().Code, HandleShotEnd);
			Handlers.Add(new MsgDropFlag().Code, HandleDropFlag);
			Handlers.Add(new MsgGrabFlag().Code, HandleGrabFlag);
			Handlers.Add(new MsgTransferFlag().Code, HandleTransferFlag);
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
					WriteEmptyLine();
				}
			}
			else
				WriteLine("Unknown Message " + e.Message.Code.ToString());

			WriteEmptyLine();
		}

        protected static bool UDPSendEnabled = false;

        private static bool UDPRequestSent = false;
        private static bool UDPOutOk = false;
        private static bool UDPInOk = false;

        protected static void SendTCPMessage(NetworkMessage msg)
        {
            client.SendMessage(true, msg);
        }

        protected static void SendUDPMessage(NetworkMessage msg)
        {
            client.SendMessage(!UDPSendEnabled, msg);
        }

        protected static void SendEnter()
		{
			var enter = new MsgEnter();
			enter.Callsign = Callsign;
			enter.Motto = "Testing 1...2..3.";
			enter.Token = Link.LastToken;
            SendTCPMessage(enter);
		}

		private static void HandleAddPlayer(NetworkMessage msg)
		{
			MsgAddPlayer ap = msg as MsgAddPlayer;
			WriteLine("Player Added " + ap.Callsign + "(" + ap.PlayerID.ToString() + ")");
		}

		private static void HandleRemovePlayer(NetworkMessage msg)
		{
			MsgRemovePlayer rp = msg as MsgRemovePlayer;
			WriteLine("Player Removed " + rp.PlayerID.ToString());
		}

		private static void HandlePlayerInfo(NetworkMessage msg)
		{
			MsgPlayerInfo info = msg as MsgPlayerInfo;
			WriteLine("Players Were Updated ");
			foreach(var p in info.PlayerUpdates)
				WriteLine(String.Format("\tID: {0} Attributes: {1} ", p.PlayerID, p.Attributes.ToString()));
		}

        private static void HandleScoreUpdate(NetworkMessage msg)
        {
            MsgScore sc = msg as MsgScore;
            WriteLine("Player Score was updated " + sc.PlayerID.ToString() + String.Format(" = {0}/{1}/{2}", sc.Wins, sc.Losses, sc.TeamKills));
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

			if (GetWorld)
			{
				WriteLine("Requesting World");
				client.SendMessage(new MsgGetWorld(0));
			}
			else
				SendEnter();
		}

		private static void HandleGetWorld(NetworkMessage msg)
		{
			MsgGetWorld wldChunk = msg as MsgGetWorld;

			WriteLine("World Data Received, " + (wldChunk.Offset / 1024.0).ToString() + "Kb is left");
					
			int offset = WorldData.Length;
			Array.Resize(ref WorldData, WorldData.Length + wldChunk.Data.Length);
			Array.Copy(wldChunk.Data, 0, WorldData, offset, wldChunk.Data.Length);

			if(wldChunk.Offset > 0)
				client.SendMessage(new MsgGetWorld((UInt32)WorldData.Length));
			else
			{
				WriteLine("World Data Received, size " + (WorldData.Length / 1024.0).ToString() + "Kb");
				SendEnter();
			}	
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

            UDPRequestSent = true;
            // start UDP Link
            client.ConnectToUDP();
			client.SendMessage(false, new MsgUDPLinkRequest(PlayerID));
			WriteLine("Sent UDP Link request via UDP");
		}

		private static void HandleUDPLinkRequest(NetworkMessage msg)
		{
			MsgUDPLinkRequest udp = msg as MsgUDPLinkRequest;

			if (udp.FromUDP)
			{
                WriteLine("UDP Link request via UDP");
                if (UDPRequestSent)
                {
                    UDPInOk = true;

                    if (UDPOutOk)
                    {
                        client.SendMessage(false, new MsgUDPLinkEstablished());
                        WriteLine("UDP handshake complete");
                        UDPSendEnabled = true;
                    }
                }
			}
			else
			{
                WriteLine("UDP Link request via TCP ... SHOULD NOT HAPPEN TO CLIENT!");
            }
		}

        private static void HandleUDPLinkEstablished(NetworkMessage msg)
        {
            MsgUDPLinkEstablished udp = msg as MsgUDPLinkEstablished;

            if (udp.FromUDP)
            {
                WriteLine("UDP Link established via UDP ... SHOULD NOT HAPPEN TO CLIENT!");
            }
            else
            {
                WriteLine("UDP Link established via TCP");
                if (UDPRequestSent)
                {
                    UDPOutOk = true;

                    if (UDPInOk)
                    {
                        client.SendMessage(false, new MsgUDPLinkEstablished());
                        WriteLine("UDP handshake complete");
                        UDPSendEnabled = true;
                    }
                }
            }
        }

        private static void HandleLagPing(NetworkMessage msg)
        {
            MsgLagPing ping = msg as MsgLagPing;

            client.SendMessage(ping.FromUDP, ping);
        }
		private static void HandleFlagUpdate(NetworkMessage msg)
		{
			MsgFlagUpdate update = msg as MsgFlagUpdate;

			WriteLine("Flag update");
			foreach(var u in update.FlagUpdates)
			{
				WriteLine(String.Format("\tID:{0} ({1}) {2}", u.FlagID, u.Abreviation, u.Status));
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
			WriteLine("MsgQueryGame " + g.CodeAbreviation.ToString());
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

        private static void HandleQueryPlayers(NetworkMessage msg)
        {
            MsgQueryPlayers qp = msg as MsgQueryPlayers;

            WriteLine("MsgQueryPlayers "+ String.Format("Teams {0}  Players {1}", qp.NumTeams, qp.NumPlayers));
        }

        private static void HandleAlive(NetworkMessage msg)
        {
            MsgAlive alive = msg as MsgAlive;
            WriteLine("MsgAlive " + alive.PlayerID.ToString());
            WriteLine(String.Format("\tPosition = X{0} Y{1} Z{2} Rotation = {3}", alive.Position.X, alive.Position.Y, alive.Position.Z, alive.Azimuth));
        }

        private static void HandlePlayerUpdate(NetworkMessage msg)
        {
            MsgPlayerUpdateBase upd = msg as MsgPlayerUpdateBase;
            WriteLine("MsgPlayerUpdate " + upd.CodeAbreviation.ToString() + String.Format(" From {0} {1}",upd.PlayerID,upd.Status));
            WriteLine(String.Format("\tPosition = X{0} Y{1} Z{2}", upd.Position.X, upd.Position.Y, upd.Position.Z));
			WriteLine(String.Format("\tVelocity = X{0} Y{1} Z{2}", upd.Velocity.X, upd.Velocity.Y, upd.Velocity.Z));
		}

		private static void HandleShotBegin(NetworkMessage msg)
		{
			MsgShotBegin sb = msg as MsgShotBegin;
			WriteLine("MsgShotBegin " + sb.PlayerID.ToString() + " With flag [" + sb.Flag + "]");
			WriteLine("\tShotID " + sb.ShotID.ToString());
			WriteLine(String.Format("\tPosition = X{0} Y{1} Z{2}", sb.Position.X, sb.Position.Y, sb.Position.Z));
			WriteLine(String.Format("\tVelocity = X{0} Y{1} Z{2}", sb.Velocity.X, sb.Velocity.Y, sb.Velocity.Z));
		}

		private static void HandleShotEnd(NetworkMessage msg)
		{
			MsgShotEnd se = msg as MsgShotEnd;
			WriteLine("MsgShotEnd " + se.PlayerID.ToString());
			WriteLine("\tShotID " + se.ShotID.ToString());
			WriteLine("\tExploded " + se.Exploded.ToString());
		}

		private static void HandleDropFlag(NetworkMessage msg)
		{
			MsgDropFlag df = msg as MsgDropFlag;
			WriteLine("MsgDropFlag " + df.PlayerID.ToString());
			WriteLine("\tFlagID " + df.FlagID.ToString());
		}

		private static void HandleGrabFlag(NetworkMessage msg)
		{
			MsgGrabFlag gf = msg as MsgGrabFlag;
			WriteLine("MsgGrabFlag " + gf.PlayerID.ToString());
			WriteLine("\tFlagID " + gf.FlagData.FlagID.ToString());
		}
		private static void HandleTransferFlag(NetworkMessage msg)
		{
			MsgTransferFlag tf = msg as MsgTransferFlag;
			WriteLine("MsgTransferFlag From" + tf.FromID.ToString() + " To " + tf.ToID.ToString());
			WriteLine("\tFlagID " + tf.FlagID.ToString());
		}
	}
}
