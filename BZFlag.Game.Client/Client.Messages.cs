using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using BZFlag.Networking.Messages;
using BZFlag.Networking.Messages.BZFS;
using BZFlag.Networking.Messages.BZFS.UDP;
using BZFlag.Networking.Messages.BZFS.World;
using BZFlag.Networking.Messages.BZFS.BZDB;
using BZFlag.Networking.Messages.BZFS.Player;
using BZFlag.Networking.Messages.BZFS.Info;
using BZFlag.Networking.Messages.BZFS.Flags;
using BZFlag.Networking.Messages.BZFS.Shots;
using BZFlag.Networking.Messages.BZFS.Control;

namespace BZFlag.Game
{
    public partial class Client
    {
		public delegate void MessageHandler(NetworkMessage msg);
		public static Dictionary<int, MessageHandler> Handlers = new Dictionary<int, MessageHandler>();

		protected bool UDPRequestSent = false;
		protected bool UDPSendEnabled = false;
		private  bool UDPOutOk = false;
		private  bool UDPInOk = false;

		private void NetClient_HostMessageReceived(object sender, Networking.Client.HostMessageReceivedEventArgs e)
		{
			if(Handlers.ContainsKey(e.Message.Code))
				Handlers[e.Message.Code](e.Message);
			else if(e.Message as UnknownMessage != null)
			{
				// unknown message
			}
		}

		protected void SendTCPMessage(NetworkMessage msg)
		{
			NetClient.SendMessage(true, msg);
		}

		protected void SendUDPMessage(NetworkMessage msg)
		{
			NetClient.SendMessage(!UDPSendEnabled, msg);
		}

		protected void SendEnter()
		{
			var enter = new MsgEnter();
			enter.Callsign = Params.Callsign;
			enter.Motto = Params.Motto;
			enter.Token = Params.Token;
			SendTCPMessage(enter);
		}

		public void RegisterMessageHandlers()
		{
			Handlers.Add(new MsgAccept().Code, HandleAcceptMessage);
			Handlers.Add(new MsgReject().Code, HandleRejectMessage);
			Handlers.Add(new MsgWantWHash().Code, HandleWorldHash);
			Handlers.Add(new MsgCacheURL().Code, HandleWorldCacheURL);
			Handlers.Add(new MsgGetWorld().Code, HandleGetWorld);
		}

		private void HandleAcceptMessage(NetworkMessage msg)
		{
			MsgAccept accept = msg as MsgAccept;

			PlayerID = accept.PlayerID;

			NetClientAccepted();
			UDPRequestSent = true;
			// start UDP Link
			NetClient.ConnectToUDP();
			NetClient.SendMessage(false, new MsgUDPLinkRequest(PlayerID));
		}

		private void HandleRejectMessage(NetworkMessage msg)
		{
			MsgReject reject = msg as MsgReject;
			NetClient.Shutdown();
			NetClientRejected(reject.ReasonCode, reject.ReasonMessage);
		}

	}
}
