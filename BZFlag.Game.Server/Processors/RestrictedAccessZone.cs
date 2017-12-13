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
using BZFlag.Game.Security;
using BZFlag.Game.Host.World;
using BZFlag.Data.Game;
using System.Security.Cryptography;

namespace BZFlag.Game.Host.Processors
{
    public class RestrictedAccessZone : PlayerProcessor
    {
        public BanList Bans = new BanList();
        public FlagManager Flags = new FlagManager();
        public GameWorld World = new GameWorld();

        protected MsgGameSettings SettingsCache = null;
        protected MsgWantWHash HashCache = null;
        protected MsgCacheURL URLCache = null;

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
            SettingsCache.GameType = GameTypes.OpenFFA;
            SettingsCache.GameOptions = GameOptionFlags.NoStyle;
            SettingsCache.MaxPlayers = 200;
            SettingsCache.MaxShots = 1;
            SettingsCache.MaxFlags = 50;
            SettingsCache.LinearAcceleration = 0;
            SettingsCache.AngularAcceleration = 0;

            HashCache = new MsgWantWHash();
            HashCache.IsRandomMap = true;
            HashCache.WorldHash = BZFlag.Data.Utils.Cryptography.MD5Hash(worldBuffer);

            URLCache = new MsgCacheURL();
            URLCache.URL = Config.MapURL;

        }

        private void HandleEnter(ServerPlayer player, NetworkMessage msg)
        {
            Logger.Log4("Processing enter for " + player.PlayerID.ToString());

            MsgEnter enter = msg as MsgEnter;
            if (enter == null)
                return;

            if (enter.Callsign == string.Empty || enter.Callsign.Length < 3)
            {
                SendReject(player, MsgReject.RejectionCodes.RejectBadCallsign, "Invalid callsign");
                return;
            }

            player.Callsign = enter.Callsign;
            player.Motto = enter.Motto;
            player.Token = enter.Token;

            if (player.Token == string.Empty && !Config.ProtectRegisteredNames)
            {
                player.AuthStatus = ServerPlayer.AuthStatuses.NoneProvided;
                if (!Config.AllowAnonUsers)
                    SendReject(player, MsgReject.RejectionCodes.RejectBadCallsign, "Registered Users Only");
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
                SendReject(player, MsgReject.RejectionCodes.RejectBadCallsign, "Authentication Failed");
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

                var ban = Bans.FindIDBan(checker.BZID);

                if (ban != null)
                    SendReject(player, MsgReject.RejectionCodes.RejectIDBanned, ban.Reason);
                else
                    SendAccept(player);
            }
            else if (checker.NameRegistered && Config.ProtectRegisteredNames)
                SendReject(player, MsgReject.RejectionCodes.RejectBadCallsign, "Callsign is registered");
            else
            {
                if (!Config.AllowAnonUsers)
                    SendReject(player, MsgReject.RejectionCodes.RejectBadCallsign, "Callsign must be registered");
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

            Promote(player);
        }

        private void SendReject(ServerPlayer player, MsgReject.RejectionCodes code, string reason)
        {
            Logger.Log1("Reject Player " + player.PlayerID + " " + code.ToString() + " :" + reason);
            player.SendMessage(new MsgReject(code, reason));
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
            if (ws == null)
                return;

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
