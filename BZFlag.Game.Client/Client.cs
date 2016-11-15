using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Teams;
using BZFlag.Networking.Messages.BZFS.Control;
using BZFlag.Networking.Messages.BZFS.World;
using BZFlag.Game.Chat;
using BZFlag.Game.Shots;
using BZFlag.Game.Players;
using BZFlag.Data.Flags;

namespace BZFlag.Game
{
	public class ClientParams
	{
		public string Callsign = string.Empty;
		public string Motto = string.Empty;
		public string Token = string.Empty;

		public string Host = string.Empty;
		public int Port = 5-1;

		public TeamColors DesiredTeam = TeamColors.AutomaticTeam;

		public string CacheFolder = string.Empty;
	}

    public partial class Client
    {
		public BZFlag.Networking.ClientConnection NetClient = new BZFlag.Networking.ClientConnection();

		public BZFlag.Map.WorldMap Map = null;
		public ChatSystem Chat = new ChatSystem();

		public FlagTypeList FlagTypes = new FlagTypeList();
		public PlayerManager PlayerList = new PlayerManager();
		public ShotManager ShotMan = null;

		public GameTime Clock = new GameTime();

        public bool InTimedGame { get; protected set; }
        public double TimeLeftInGame { get; protected set; }

        protected ClientParams Params = null;

		public Client(ClientParams _params)
		{
            InTimedGame = false;
            TimeLeftInGame = -1;

            PlayerList.Clock = Clock;
			PlayerList.FlagTypes = FlagTypes;

			ShotMan = new ShotManager(PlayerList); ;

			Params = _params;
			RegisterMessageHandlers();

			NetClient.TCPConnected += NetClient_TCPConnected;
			NetClient.HostMessageReceived += NetClient_HostMessageReceived;
			NetClient.HostIsNotBZFS += NetClient_HostIsNotBZFS;

			NetClient.Startup(Params.Host, Params.Port);
		}

		public void Shutdown()
		{
			NetClient.Shutdown();
		}

		public void Update()
		{
			Clock.SetStepTime();
			NetClient.Update();
		}

		public event EventHandler HostIsNotBZFlag = null;
		private void NetClient_HostIsNotBZFS(object sender, EventArgs e)
		{
			if(HostIsNotBZFlag != null)
				HostIsNotBZFlag.Invoke(this, e);
		}

		public event EventHandler TCPConnected = null;

		private void NetClient_TCPConnected(object sender, EventArgs e)
		{
			//SendMessage(new MsgNegotiateFlags(BZFlag.Networking.Flags.FlagCache.FlagList.Keys));
			// SendMessage(new MsgQueryGame());
			NetClient.SendMessage(new MsgWantWHash());

			if(TCPConnected != null)
				TCPConnected.Invoke(this, e);
		}

		public event EventHandler ClientAccepted = null;
		protected virtual void NetClientAccepted()
		{
			//NetClient.SendMessage(new MsgWantWHash());
			if(ClientAccepted != null)
				ClientAccepted.Invoke(this, EventArgs.Empty);
		}

		public class ClientRejectionEventArgs : EventArgs
		{
			public MsgReject.RejectionCodes Code;
			public string Reason = string.Empty;

			public ClientRejectionEventArgs(MsgReject.RejectionCodes code, string reason)
			{
				Code = code;
				Reason = reason;
			}
		}
		public event EventHandler<ClientRejectionEventArgs> ClientRejected = null;

		protected virtual void NetClientRejected(MsgReject.RejectionCodes code, string reason)
		{
			NetClient.Shutdown();
			if(ClientRejected != null)
				ClientRejected.Invoke(this, new ClientRejectionEventArgs(code,reason));
		}
		
	}
}
