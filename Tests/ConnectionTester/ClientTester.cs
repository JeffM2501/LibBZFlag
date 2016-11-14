using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;

using BZFlag.Game;
using BZFlag.Authentication;
using BZFlag.Data.Teams;
using BZFlag.Game.Players;

namespace ConnectionTester
{
	public class ClientTester
	{
		ClientParams StartupParams = new ClientParams();

		Client GameClient = null;

		ServiceLink Link = new ServiceLink();
		bool GotList = false;

		public StreamWriter LogStream = new StreamWriter("log.txt");
		public void WriteLine(string text)
		{
			Console.WriteLine(text);
			LogStream.WriteLine(text);
			LogStream.Flush();
		}

		public void WriteEmptyLine()
		{
			Console.WriteLine();
			LogStream.WriteLine();
			LogStream.Flush();
		}

		public void Write(string text)
		{
			Console.Write(text);
			LogStream.Write(text);
		}

		public ClientTester(string[] args)
		{
			Link.RequestCompleted += Link_RequestCompleted;
			Link.RequestErrored += Link_RequestErrored;

			StartupParams.DesiredTeam = TeamColors.ObserverTeam;

			if(args.Length > 0)
				StartupParams.Callsign = args[0];
			else
				StartupParams.Callsign = "Billy D. Bugger";

			GetList(args.Length > 1 ? args[1] : string.Empty);

			var server = Link.FindServerWithMostPlayers();
			if(server == null || false)
				server = new ServiceLink.ListServerData("bzflag.allejo.io", 5170);

			StartupParams.Host = server.Host;
			StartupParams.Port = server.Port;
			StartupParams.Motto = "Testing 1...2..3.";
		}

		private void GetList(string pass)
		{
			Link.GetList(StartupParams.Callsign, pass);

			while(GotList == false)
				Thread.Sleep(10);
		}

		private void Link_RequestErrored(object sender, EventArgs e)
		{
			WriteLine("List Request Error " + Link.LastError);
			GotList = true;
		}

		private void Link_RequestCompleted(object sender, EventArgs e)
		{
			WriteLine("List Request Completed " + Link.LastToken);
			WriteLine("List Server Count " + Link.ServerList.Count.ToString());
			StartupParams.Token = Link.LastToken;
			GotList = true;
		}

		public void Run()
		{
			GameClient = new Client(StartupParams);
			GameClient.HostIsNotBZFlag += GameClient_HostIsNotBZFlag;

			GameClient.WorldDownloadProgress += GameClient_WorldDownloadProgress;	

			GameClient.TCPConnected += GameClient_TCPConnected;
			GameClient.ClientAccepted += GameClient_ClientAccepted;
			GameClient.ClientRejected += GameClient_ClientRejected;

			GameClient.SelfAdded += GameClient_SelfAdded;
			GameClient.PlayerAdded += GameClient_PlayerAdded;
			GameClient.PlayerRemoved += GameClient_PlayerRemoved;
			GameClient.PlayerStateUpdated += GameClient_PlayerStateUpdated;
			GameClient.PlayerInfoUpdated += GameClient_PlayerInfoUpdated;

			GameClient.ReceivedUnknownMessage += GameClient_ReceivedUnknownMessage;

			GameClient.Chat.ChatMessageReceived += Chat_ChatMessageReceived;

			GameClient.FlagCreated += GameClient_FlagCreated;
			GameClient.FlagUpdated += GameClient_FlagUpdated;
			GameClient.FlagGrabbed += GameClient_FlagGrabbed;
			GameClient.FlagDropped += GameClient_FlagDropped;
			GameClient.FlagTransfered += GameClient_FlagTransfered;

			while(true)
			{
				GameClient.Update();
				Thread.Sleep(50);
			}
		}

		private void GameClient_ReceivedUnknownMessage(object sender, Client.UnknownMessageEventArgs e)
		{
			WriteLine("Unknown message " + e.CodeAbriv + " (" + e.CodeID.ToString("X2") + ")");
		}

		private void Chat_ChatMessageReceived(object sender, BZFlag.Game.Chat.ChatSystem.ChatMessageEventArgs e)
		{
			WriteLine("Chat Recieved " + e.Message.From.ToString() + " " + e.Message.Message);
		}

		private void GameClient_WorldDownloadProgress(object sender, Client.WorldDownloadProgressEventArgs e)
		{
			WriteLine("World Download Progress " + (e.Paramater * 100.0f).ToString() + "%");
		}

		private void GameClient_TCPConnected(object sender, EventArgs e)
		{
			WriteLine("TCP Connected to " + StartupParams.Host + ":" + StartupParams.Port.ToString());
		}

		private void GameClient_ClientRejected(object sender, Client.ClientRejectionEventArgs e)
		{
			WriteLine("Client Rejected " + e.Code.ToString() + " " + e.Reason);
		}

		private void GameClient_ClientAccepted(object sender, EventArgs e)
		{
			WriteLine("Client Accepted " + GameClient.PlayerID.ToString());
		}

		private void GameClient_PlayerRemoved(object sender, Player player)
		{
			WriteLine("Player Removed " + player.Callsign);
		}

		private void GameClient_PlayerAdded(object sender, Player player)
		{
			WriteLine("Player Added " + player.Callsign);
		}

		private void GameClient_PlayerInfoUpdated(object sender, Player player)
		{
			WriteLine("Player Info Updated " + player.Callsign);
		}

		private void GameClient_PlayerStateUpdated(object sender, Player player)
		{
			WriteLine("Player Status Updated " + player.Callsign + " Last Pos " + string.Format("X{0} Y{1} Z{2}", player.LastUpdate.Position.X, player.LastUpdate.Position.Y, player.LastUpdate.Position.Z));
		}

		private void GameClient_SelfAdded(object sender, Player player)
		{
			WriteLine("Self Added " + player.Callsign);
		}

		private void GameClient_FlagTransfered(object sender, BZFlag.Game.Flags.FlagInstance e)
		{
			WriteLine("Flag Instance " + e.ID.ToString() + " Transfered to " + e.Owner.Callsign);
		}

		private void GameClient_FlagDropped(object sender, BZFlag.Game.Flags.FlagInstance e)
		{
			WriteLine("Flag Instance " + e.ID.ToString() + " dropped");
		}

		private void GameClient_FlagGrabbed(object sender, BZFlag.Game.Flags.FlagInstance e)
		{
			WriteLine("Flag Instance " + e.ID.ToString() + " Grabbed by " + e.Owner.Callsign);
		}

		private void GameClient_FlagUpdated(object sender, BZFlag.Game.Flags.FlagInstance e)
		{
			WriteLine("Flag Instance " + e.ID.ToString() + " Updated");
		}

		private void GameClient_FlagCreated(object sender, BZFlag.Game.Flags.FlagInstance e)
		{
			WriteLine("Flag Instance " + e.ID.ToString() + " Created");
		}

		private void GameClient_HostIsNotBZFlag(object sender, EventArgs e)
		{
			WriteLine("Host is not BZFS");
		}
	}
}
