
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using BZFlag.Networking.Messages.BZFS.Info;
using BZFlag.Networking.Messages.BZFS.World;
using BZFlag.Networking.Messages.BZFS.Player;
using BZFlag.IO.BZW.Binary;
using BZFlag.Networking.Messages;
using BZFlag.Game.Players;
using BZFlag.Map.Elements.Shapes;
using BZFlag.Map;
using BZFlag.Data.BZDB;
using BZFlag.LinearMath;

namespace BZFlag.Game
{
    public partial class Client
    {
        private WorldUnpacker Unpacker = null;

        private MsgGameSettings GameSettings = new MsgGameSettings();

        private string WorldURL = string.Empty;

        public class WorldDownloadProgressEventArgs : EventArgs
        {
            public float Paramater = 0;

            public WorldDownloadProgressEventArgs(float p)
            {
                Paramater = p;
            }
        }
        public event EventHandler<WorldDownloadProgressEventArgs> WorldDownloadProgress = null;

        protected string WorldHash = string.Empty;
        protected BZWCache WorldCache = null;

        protected void SetMap(WorldMap map)
        {
            Map = map;
            ShotMan.Map = map;
        }

        private void HandleWorldHash(NetworkMessage msg)
        {
            MsgWantWHash hash = msg as MsgWantWHash;
            WorldHash = hash.WorldHash;

            bool getWorld = true;

            if (Params.CacheFolder != null)
            {
                WorldCache = new BZWCache(Params.CacheFolder);
                if (WorldCache.CheckCacheForHash(hash.WorldHash))
                {
                    SetMap(WorldCache.ReadMapFromCache(hash.WorldHash));
                    if (Map != null)
                        getWorld = false;
                }
            }

            if (getWorld && WorldURL != string.Empty)
            {
                if (WorldDownloadProgress != null)
                    WorldDownloadProgress.Invoke(this, new WorldDownloadProgressEventArgs(0));

                // try to download it from the interwebs
                WebClient worldWWW = new WebClient();
                worldWWW.DownloadDataCompleted += worldWWW_DownloadDataCompleted;
                worldWWW.DownloadProgressChanged += worldWWW_DownloadProgressChanged;
                worldWWW.DownloadDataAsync(new Uri(WorldURL));
            }

            if (getWorld)
                SendGetWorld();
            else
                SendEnter();
        }

        protected void SendGetWorld()
        {
            if (WorldDownloadProgress != null)
                WorldDownloadProgress.Invoke(this, new WorldDownloadProgressEventArgs(0));
            Unpacker = new WorldUnpacker();
            NetClient.SendMessage(new MsgGetWorld(0));
        }

        private void worldWWW_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (WorldDownloadProgress != null)
                WorldDownloadProgress.Invoke(this, new WorldDownloadProgressEventArgs(e.ProgressPercentage / 100.0f));
        }

        private void worldWWW_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            if (WorldDownloadProgress != null)
                WorldDownloadProgress.Invoke(this, new WorldDownloadProgressEventArgs(1.0f));

            if (e.Cancelled || e.Error != null)
                SendGetWorld();
            else
            {
                WorldUnpacker unpacker = new WorldUnpacker(e.Result);
                SetMap(unpacker.Unpack());
                if (Map == null)
                    SendGetWorld();
                else
                {
                    WorldCache.SaveMapToCache(WorldHash, unpacker.GetBuffer());
                    SendEnter();
                }
            }
        }

        private void HandleGetWorld(NetworkMessage msg)
        {
            MsgGetWorld wldChunk = msg as MsgGetWorld;

            if (Unpacker == null)
                Unpacker = new WorldUnpacker();

            Unpacker.AddData(wldChunk.Data);

            if (wldChunk.Offset > 0)
            {
                if (WorldDownloadProgress != null)
                    WorldDownloadProgress.Invoke(this, new WorldDownloadProgressEventArgs((float)Unpacker.Size() / (float)(((UInt32)wldChunk.Offset + Unpacker.Size()))));
                NetClient.SendMessage(new MsgGetWorld((UInt32)Unpacker.Size()));
            }
            else
            {
                if (WorldDownloadProgress != null)
                    WorldDownloadProgress.Invoke(this, new WorldDownloadProgressEventArgs(1));

                SetMap(Unpacker.Unpack());
                if (WorldCache != null)
                    WorldCache.SaveMapToCache(WorldHash, Unpacker.GetBuffer());
                SendEnter();
            }
        }

        private void HandleWorldCacheURL(NetworkMessage msg)
        {
            if (WorldURL != string.Empty)
                return;

            MsgCacheURL url = msg as MsgCacheURL;
            WorldURL = url.URL;

            NetClient.SendMessage(new MsgWantWHash()); // fastmap servers send the world cache before the hash, so send the hash request again so they know we wants it
        }

        public class TeleportEventArgs : EventArgs
        {
            public Player PortingPlayer = null;
            public Teleporter From = null;
            public Teleporter To = null;
        }
        public event EventHandler<TeleportEventArgs> PlayerTeleported = null;

        public void HandleTeleported(NetworkMessage msg)
        {
            MsgTeleport tp = msg as MsgTeleport;

            TeleportEventArgs args = new Game.Client.TeleportEventArgs();

            args.PortingPlayer = PlayerList.GetPlayerByID(tp.PlayerID);
            if (args.PortingPlayer == null)
                return;

            args.From = Map.GetTeleporterByID(tp.FromTPID);
            args.To = Map.GetTeleporterByID(tp.ToTPID);

            args.PortingPlayer.SetTeleport(Clock.StepTime, args.From, args.To);

            if (PlayerTeleported != null)
                PlayerTeleported.Invoke(this, args);

        }

        protected void HandleGravityChanged(object sender, EventArgs e)
        {
            if (Map != null)
                Map.Constants.Gravity = BZDatabase.GetValueF(BZDBVarNames.Gravity);
        }
    }
}
