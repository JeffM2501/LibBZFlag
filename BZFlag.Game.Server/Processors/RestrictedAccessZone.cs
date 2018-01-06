using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BZFlag.Networking.Messages;

using BZFlag.Game.Host.Players;
using BZFlag.Networking;
using BZFlag.Networking.Messages.BZFS.World;
using BZFlag.Networking.Messages.BZFS.Player;
using BZFlag.Networking.Messages.BZFS.Control;
using BZFlag.Networking.Messages.BZFS.Info;
using BZFlag.Services;
using BZFlag.Game.Host.World;
using BZFlag.Data.Game;
using System.Security.Cryptography;

namespace BZFlag.Game.Host.Processors
{
    internal class RestrictedAccessZone : PlayerProcessor
    {
        protected MsgGameSettings SettingsCache = null;
        protected MsgWantWHash HashCache = null;
        protected MsgCacheURL URLCache = null;

        public delegate bool IDBanCallback(ServerPlayer player, ref string reason);

        public IDBanCallback CheckIDBan = null;

        public event EventHandler<ServerPlayer> PlayerRejected;
        public event EventHandler<ServerPlayer> PlayerBanned;
        public event EventHandler<ServerPlayer> PlayerAccepted;

        public event EventHandler<Server.BooleanResultPlayerEventArgs> CheckPlayerAcceptance;

        public RestrictedAccessZone(ServerConfig cfg) : base(cfg)
        {
            MessageProcessor = SecurityJailMessageFacotry.Factory;

            RegisterCommonHandlers();

            MessageDispatch.Add(new MsgWantWHash(), HandleWantWorldHash);
            MessageDispatch.Add(new MsgEnter(), HandleEnter);
            MessageDispatch.Add(new MsgNegotiateFlags(), HandleNegotiateFlags);
            MessageDispatch.Add(new MsgWantSettings(), HandleWantSettings);

            MessageDispatch.Add(new MsgGetWorld(), HandleGetWorld);
        }

        public override void Setup()
        {
            SettingsCache = new MsgGameSettings();

            byte[] worldBuffer = World.GetWorldData();
            SettingsCache.WorldSize = worldBuffer.Length;
            SettingsCache.GameType = Config.GameData.GameType;
            SettingsCache.GameOptions = Config.GameData.GameOptions;
            SettingsCache.MaxPlayers = Config.GameData.MaxPlayers;
            SettingsCache.MaxShots = Config.GameData.MaxShots;
            SettingsCache.MaxFlags = FlagManager.MaxFlagID;
            SettingsCache.LinearAcceleration = Config.GameData.LinearAcceleration;
            SettingsCache.AngularAcceleration = Config.GameData.AngularAcceleration;

            HashCache = new MsgWantWHash();
            HashCache.IsRandomMap = World.IsRandom;
            HashCache.WorldHash = BZFlag.Data.Utils.Cryptography.MD5Hash(worldBuffer);

            URLCache = new MsgCacheURL();
            URLCache.URL = Config.GameData.MapURL;
        }

        protected override void PlayerAdded(ServerPlayer player)
        {
            player.NeedStartupInfo = true;

            // send them the player ID, so they can give us data
            player.SendDirectMessage(true, new byte[] { (byte)player.PlayerID });

            base.PlayerAdded(player);
        }

        private void HandleEnter(ServerPlayer player, NetworkMessage msg)
        {
            Logger.Log4("Processing enter for " + player.PlayerID.ToString());

            MsgEnter enter = msg as MsgEnter;
            if (enter == null)
                return;

            if (enter.PlayerType == Data.Players.PlayerTypes.ComputerPlayer)
            {
                if (player.HasValidEnter)   // keep the one valid connection
                {
                    Logger.Log1("Reject Solo bot on  " + player.PlayerID + " connection");
                    player.SendMessage(new MsgReject(MsgReject.RejectionCodes.RejectBadType, Resources.NoRobotsMessage));
                }
                else
                    SendReject(player, MsgReject.RejectionCodes.RejectBadType, Resources.NoRobotsMessage);
                return;
            }

            if (enter.Callsign == string.Empty || enter.Callsign.Length < 3)
            {
                SendReject(player, MsgReject.RejectionCodes.RejectBadCallsign, Resources.BadCallsignMessage);
                return;
            }

            player.DesiredTeam = enter.PlayerTeam;
            player.Callsign = enter.Callsign;
            player.Motto = enter.Motto;
            player.Token = enter.Token;

            Server.BooleanResultPlayerEventArgs args = new Server.BooleanResultPlayerEventArgs(player);
            CheckPlayerAcceptance?.Invoke(this, args);

            if (!args.Result)
            {
                SendReject(player, MsgReject.RejectionCodes.RejectUnknown, Resources.APIRejectMessage);
                return;
            }

            player.HasValidEnter = true;
            if (player.Token == string.Empty && !Config.ProtectRegisteredNames)
            {
                player.AuthStatus = ServerPlayer.AuthStatuses.NoneProvided;
                if (!Config.AllowAnonUsers)
                    SendReject(player, MsgReject.RejectionCodes.RejectBadCallsign, Resources.NoUnregMessage);
                else
                    SendAccept(player);
            }
            else
            {
                Logger.Log3("Starting token verification for " + player.PlayerID.ToString() + ":" + enter.Callsign);

                player.AuthStatus = ServerPlayer.AuthStatuses.InProgress;

                ClientTokenCheck checker = new ClientTokenCheck();
                checker.Tag = player;
                checker.RequestCompleted += this.Checker_RequestCompleted;
                checker.RequestErrored += this.Checker_RequestErrored;

                checker.CheckToken(player.Callsign, player.Token, player.ConnectionData.GetIPAsString(), Config.SecurityGroups);
            }
        }

        private void Checker_RequestErrored(object sender, EventArgs e)
        {
            Logger.Log4("Token Checkup failed");

            ClientTokenCheck checker = sender as ClientTokenCheck;
            if (checker == null || checker.Tag as ServerPlayer == null)
                return;

            ServerPlayer player = checker.Tag as ServerPlayer;

            Logger.Log3("Token verification failed for " + player.PlayerID.ToString() + ":" + player.Callsign);

            player.AuthStatus = ServerPlayer.AuthStatuses.Failed;
            if (!Config.AllowAnonUsers)
                SendReject(player, MsgReject.RejectionCodes.RejectBadCallsign, Resources.BadAuthMessage);
            else
                SendAccept(player);
        }

        private void Checker_RequestCompleted(object sender, EventArgs e)
        {
            ClientTokenCheck checker = sender as ClientTokenCheck;
            if (checker == null || checker.Tag as ServerPlayer == null)
                return;

            ServerPlayer player = checker.Tag as ServerPlayer;

            Logger.Log3("Token verification returned for " + player.PlayerID.ToString() + ":" + player.Callsign + (checker.OK ? " OK:" : " BAD:") + (checker.NameRegistered ? "REGISTERED" : "UNKNOWN"));

            if (checker.OK)
            {
                player.AuthStatus = ServerPlayer.AuthStatuses.Valid;
                player.BZID = checker.BZID;
                player.GroupMemberships = checker.Groups;


                bool ban = false;
                string reason = string.Empty;

                if (CheckIDBan != null)
                    ban = CheckIDBan(player, ref reason);

                if (ban)
                {
                    PlayerBanned?.Invoke(this, player);
                    SendReject(player, MsgReject.RejectionCodes.RejectIDBanned, reason);
                }
                   
                else
                    SendAccept(player);
            }
            else if (checker.NameRegistered && Config.ProtectRegisteredNames)
                SendReject(player, MsgReject.RejectionCodes.RejectBadCallsign, Resources.NameTakenMessagae);
            else
            {
                if (!Config.AllowAnonUsers)
                    SendReject(player, MsgReject.RejectionCodes.RejectBadCallsign, Resources.NoUnregMessage);
                else
                    SendAccept(player);
            }
        }

        private void SendAccept(ServerPlayer player)
        {
            Logger.Log1("Accept Player " + player.PlayerID + ":" + player.Callsign);

            MsgAccept accept = new MsgAccept();
            accept.PlayerID = player.PlayerID;

            player.SendMessage(accept);

            PlayerAccepted?.Invoke(this, player);
            Promote(player);
        }

        private void SendReject(ServerPlayer player, MsgReject.RejectionCodes code, string reason)
        {
            player.Accepted = false;
            player.RejectionReason = code.ToString() + " :" + reason;

            Logger.Log1("Reject Player " + player.PlayerID + " " + player.RejectionReason);
            player.SendMessage(new MsgReject(code, reason));

            PlayerRejected?.Invoke(this, player);

            RemovePlayer(player);
            player.Disconnect();
        }

        private void HandleWantWorldHash(ServerPlayer player, NetworkMessage msg)
        {
            Logger.Log4("Getting world hash for " + player.PlayerID.ToString());

            MsgWantWHash hash = msg as MsgWantWHash;
            if (hash == null)
                return;

            if (URLCache != null && URLCache.URL != string.Empty)
                player.SendMessage(URLCache);

            player.SendMessage(HashCache);
        }

        private void HandleNegotiateFlags(ServerPlayer player, NetworkMessage msg)
        {
            Logger.Log4("Negotiating flags for " + player.PlayerID.ToString());

            player.ClientFlagList = msg as MsgNegotiateFlags;
            if (player.ClientFlagList == null)
                return;

            player.SendMessage(Flags.GetFlagNegotiation(player.ClientFlagList));
        }

        private void HandleWantSettings(ServerPlayer player, NetworkMessage msg)
        {
            Logger.Log4("Getting settings for " + player.PlayerID.ToString());

            MsgWantSettings ws = msg as MsgWantSettings;
            if (ws == null || player.SentSettings)
                return;

            player.SentSettings = true;
            player.SendMessage(SettingsCache);
        }
   
        private void HandleGetWorld(ServerPlayer player, NetworkMessage msg)
        {
            MsgGetWorld getW = msg as MsgGetWorld;
            if (getW == null)
                return;

            Logger.Log4("Getting world chunk for " + player.PlayerID.ToString() + " at offset " + getW.Offset);

            UInt32 len = (UInt32)World.GetWorldData().Length;
            UInt32 start = getW.Offset;
            if (start >= len)
                start = len-1;

            UInt32 end = start + 1024;
            if (end > len)
                end = len;

            UInt32 realLen = end - start;
            getW.Data = new byte[realLen];
            getW.Offset = len-end;
            Array.Copy(World.GetWorldData(), start, getW.Data, 0, getW.Data.Length);

            player.SendMessage(getW);
        }
    }
}
