using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Teams;
using BZFlag.Networking.Messages.BZFS.Control;
using BZFlag.Networking.Messages.BZFS.World;

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
		public BZFlag.Networking.Client NetClient = new BZFlag.Networking.Client();

		public BZFlag.Map.WorldMap Map = null;

		public int PlayerID { get; protected set; }

		protected ClientParams Params = null;

		public Client(ClientParams _params)
		{
			Params = _params;
			RegisterMessageHandlers();

			NetClient.TCPConnected += NetClient_TCPConnected;
			NetClient.HostMessageReceived += NetClient_HostMessageReceived;
			NetClient.HostIsNotBZFS += NetClient_HostIsNotBZFS;

			NetClient.Startup(Params.Host, Params.Port);
		}

		public void Shutdown()
		{

		}

		public event EventHandler HostIsNotBZFlag = null;
		private void NetClient_HostIsNotBZFS(object sender, EventArgs e)
		{
			if(HostIsNotBZFlag != null)
				HostIsNotBZFlag.Invoke(this, e);
		}

		private void NetClient_TCPConnected(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		protected virtual void NetClientAccepted()
		{
			NetClient.SendMessage(new MsgWantWHash());
		}

		protected virtual void NetClientRejected(MsgReject.RejectionCodes code, string reason)
		{

		}
	}
}
