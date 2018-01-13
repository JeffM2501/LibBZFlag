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
using BZFlag.Networking;
using BZFlag.LinearMath;

namespace BZFlag.Game
{
    public partial class Client
    {
        InboundMessageCallbackProcessor Handler = new InboundMessageCallbackProcessor();

        public class UnknownMessageEventArgs : EventArgs
        {
            public string CodeAbriv = string.Empty;
            public int CodeID = 0;
        }
        public event EventHandler<UnknownMessageEventArgs> ReceivedUnknownMessage = null;

        public event EventHandler ServerShutdown = null;

        public event EventHandler TimedGameStarted = null;
        public event EventHandler TimedGameTimeUpdated = null;
        public event EventHandler TimedGameEnded = null;

        protected bool UDPRequestSent = false;
        protected bool UDPSendEnabled = false;
        private bool UDPOutOk = false;
        private bool UDPInOk = false;

        public void SendMessage(NetworkMessage msg)
        {
            NetClient.SendMessage(true, msg);
        }

        public void SendMessage(bool viaTCP, NetworkMessage msg)
        {
            NetClient.SendMessage(viaTCP, msg);
        }

        protected virtual void NetClient_HostMessageReceived(object sender, Networking.Common.Peer.MessageReceivedEventArgs e)
        {
            PreDispatchChecks(e.Message);

            if (!Handler.DispatchMessage(e.Message))
            {
                if (ReceivedUnknownMessage != null)
                {
                    UnknownMessageEventArgs args = new UnknownMessageEventArgs();
                    args.CodeAbriv = e.Message.CodeAbreviation;
                    args.CodeID = e.Message.Code;
                    ReceivedUnknownMessage.Invoke(this, args);
                }
            }

            PostDispatchChecks();
        }

        protected void PreDispatchChecks(NetworkMessage msg)
        {
            if (!InitalSetVarsFinished && InitalSetVarsStarted && msg.Code != MsgSetVars.CodeValue)
            {
                InitalSetVarsFinished = true;
                BZDatabase.FinishLoading();
            }
        }

        private void PostDispatchChecks()
        {

        }

        protected virtual void SendTCPMessage(NetworkMessage msg)
        {
            NetClient.SendMessage(true, msg);
        }

        protected virtual void SendUDPMessage(NetworkMessage msg)
        {
            NetClient.SendMessage(!UDPSendEnabled, msg);
        }

        protected void SendEnter()
        {
            var enter = new MsgEnter();
            enter.Callsign = Params.Callsign;
            enter.Motto = Params.Motto;
            enter.Token = Params.Token;
            enter.PlayerTeam = Params.DesiredTeam;
            enter.PlayerType = Data.Players.PlayerTypes.TankPlayer;

            if (Params.VersionOveride != string.Empty)
                enter.Version = Params.VersionOveride;

            SendTCPMessage(enter);
        }

        protected virtual void RegisterMessageHandlers()
        {
            // basic connections
            Handler.Add(new MsgAccept(), HandleAcceptMessage);
            Handler.Add(new MsgReject(), HandleRejectMessage);

            Handler.Add(new MsgGameTime(), HandleGameTime);
            Handler.Add(new MsgUDPLinkRequest(), HandleUDPLinkRequest);
            Handler.Add(new MsgUDPLinkEstablished(), HandleUDPLinkEstablished);
            Handler.Add(new MsgLagPing(), HandleLagPing);

            Handler.Add(new MsgSuperKill(), HandleSuperKill);

            // server data
            Handler.Add(new MsgGameSettings(), HandleGameSettings);
            Handler.Add(new MsgNegotiateFlags(), HandleNegotiateFlags);

            // world data
            Handler.Add(new MsgWantWHash(), HandleWorldHash);
            Handler.Add(new MsgCacheURL(), HandleWorldCacheURL);
            Handler.Add(new MsgGetWorld(), HandleGetWorld);
            Handler.Add(new MsgTeleport(), HandleTeleported);

            // game info
            Handler.Add(new MsgTimeUpdate(), HandleTimeUpdate);
            Handler.Add(new MsgScoreOver(), HandleScoreOver);

            // bzdb
            Handler.Add(new MsgSetVars(), HandleSetVarsMessage);

            // teams
            Handler.Add(new MsgTeamUpdate(), HandleTeamUpdate);

            // flags
            Handler.Add(new MsgFlagUpdate(), HandleFlagUpdate);
            Handler.Add(new MsgDropFlag(), HandleDropFlag);
            Handler.Add(new MsgGrabFlag(), HandleGrabFlag);
            Handler.Add(new MsgTransferFlag(), HandleTransferFlag);
            Handler.Add(new MsgNearFlag(), HandleNearFlag);
            Handler.Add(new MsgCaptureFlag(), HandleCaptureFlag);

            // players
            Handler.Add(new MsgAddPlayer(), PlayerList.HandleAddPlayer);
            Handler.Add(new MsgRemovePlayer(), PlayerList.HandleRemovePlayer);
            Handler.Add(new MsgPlayerInfo(), PlayerList.HandlePlayerInfo);
            Handler.Add(new MsgScore(), PlayerList.HandleScoreUpdate);
            Handler.Add(new MsgAlive(), PlayerList.HandleAlive);
            Handler.Add(new MsgKilled(), PlayerList.HandleKilled);
            Handler.Add(new MsgPlayerUpdate(), PlayerList.HandlePlayerUpdate);
            Handler.Add(new MsgPlayerUpdateSmall(), PlayerList.HandlePlayerUpdate);
            Handler.Add(new MsgHandicap(), PlayerList.HandleHandicap);
            Handler.Add(new MsgPause(), PlayerList.HandlePause);
            Handler.Add(new MsgAutoPilot(), PlayerList.HandleAutoPilot);
            Handler.Add(new MsgNewRabbit(), PlayerList.HandleNewRabbit);
            Handler.Add(new MsgAdminInfo(), PlayerList.HandleAdminInfo);

            // chat
            Handler.Add(new MsgMessage(), Chat.HandleChatMessage);

            // shots
            Handler.Add(new MsgShotBegin(), ShotMan.HandleShotBegin);
            Handler.Add(new MsgShotEnd(), ShotMan.HandleShotEnd);
            Handler.Add(new MsgGMUpdate(), ShotMan.HandleGMUpdate);
        }

        private void HandleAcceptMessage(NetworkMessage msg)
        {
            MsgAccept accept = msg as MsgAccept;

            PlayerList.LocalPlayerID = accept.PlayerID;

            NetClientAccepted();
            UDPRequestSent = true;
            // start UDP Link
            NetClient.ConnectToUDP();
            NetClient.SendMessage(false, new MsgUDPLinkRequest(PlayerList.LocalPlayerID));
        }

        private void HandleRejectMessage(NetworkMessage msg)
        {
            MsgReject reject = msg as MsgReject;
            NetClient.Shutdown();
            NetClientRejected(reject.ReasonCode, reject.ReasonMessage);
        }

        private void HandleSuperKill(NetworkMessage msg)
        {
            NetClient.Shutdown();

            if (ServerShutdown != null)
                ServerShutdown.Invoke(this, EventArgs.Empty);
        }

        private void HandleGameTime(NetworkMessage msg)
        {
            MsgGameTime gt = msg as MsgGameTime;
            Clock.AddTimeUpdate(gt.NetTime);
        }


        public event EventHandler UDPLinkEstablished = null;

        private void HandleUDPLinkRequest(NetworkMessage msg)
        {
            MsgUDPLinkRequest udp = msg as MsgUDPLinkRequest;

            if (udp.FromUDP)
            {
                if (UDPRequestSent)
                {
                    UDPInOk = true;
                    NetClient.SendMessage(false, new MsgUDPLinkEstablished());

                    if (UDPOutOk)
                    {
                        if (UDPLinkEstablished != null)
                            UDPLinkEstablished.Invoke(this, EventArgs.Empty);
                        UDPSendEnabled = true;
                    }
                }
            }
        }

        private void HandleUDPLinkEstablished(NetworkMessage msg)
        {
            MsgUDPLinkEstablished udp = msg as MsgUDPLinkEstablished;

            if (!udp.FromUDP)
            {
                if (UDPRequestSent)
                {
                    UDPOutOk = true;
                    NetClient.SendMessage(false, new MsgUDPLinkEstablished());

                    if (UDPInOk)
                    {
                        if (UDPLinkEstablished != null)
                            UDPLinkEstablished.Invoke(this, EventArgs.Empty);
                        UDPSendEnabled = true;
                    }
                }
            }
        }

        private void HandleLagPing(NetworkMessage msg)
        {
            MsgLagPing ping = msg as MsgLagPing;

            NetClient.SendMessage(ping.FromUDP, ping);
        }

        private void HandleTimeUpdate(NetworkMessage msg)
        {
            MsgTimeUpdate tu = msg as MsgTimeUpdate;

            TimeLeftInGame = tu.TimeLeft;

            if (!InTimedGame)
            {
                InTimedGame = true;
                if (TimedGameStarted != null)
                    TimedGameStarted.Invoke(this, EventArgs.Empty);
            }
            else
            {
                if (TimedGameTimeUpdated != null)
                    TimedGameTimeUpdated.Invoke(this, EventArgs.Empty);
            }
        }

        private void HandleScoreOver(NetworkMessage msg)
        {
            MsgScoreOver so = msg as MsgScoreOver;
            InTimedGame = false;
            TimeLeftInGame = -1;

            if (TimedGameEnded != null)
                TimedGameEnded.Invoke(this, EventArgs.Empty);
        }

        private void HandleGameSettings(NetworkMessage msg)
        {
            MsgGameSettings st = msg as MsgGameSettings;
            if (st == null)
                return;

            GameSettings = st;

            SendMessage(new MsgWantWHash());
        }

        private void HandleNegotiateFlags(NetworkMessage msg)
        {
            MsgNegotiateFlags nf = msg as MsgNegotiateFlags;

            if (nf != null)
                SendMessage(new MsgWantSettings());
        }

    }
}
