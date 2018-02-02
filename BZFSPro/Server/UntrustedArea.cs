using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using BZFlag.Game.Host;
using BZFlag.Game.Host.Players;
using BZFlag.Game.Host.Processors;
using BZFSPro.Net;

namespace BZFSPro.Server
{
    internal class UntrustedArea
    {
        internal RestrictedAccessZone SecurityArea = null;
        internal StagingZone StagingArea = null;

        public event EventHandler<ServerPlayer> ReleasePlayer;

        protected Thread ProcessingThread = null;

        public void Setup(GameState state)
        {
            SecurityArea = new RestrictedAccessZone(state.ConfigData);
            SecurityArea.PromotePlayer += SecurityArea_PromotePlayer;
            SecurityArea.Set(state);

            StagingArea = new StagingZone(state.ConfigData);
            StagingArea.PromotePlayer += this.StagingArea_PromotePlayer;
            StagingArea.Set(state);
        }

        public void AddConnection(ServerPlayer player)
        {
            SecurityArea.AddPendingConnection(player);

            lock(this)
            {
                if (ProcessingThread == null)
                    return;

                ProcessingThread = new Thread(new ThreadStart(ProcessUpdates));
                ProcessingThread.Start();
            }
        }

        private void SecurityArea_PromotePlayer(object sender, ServerPlayer e)
        {
            // they the basic security tests and bans and have the map and flags
            StagingArea.AddPendingConnection(e); // get them up to speed with data
        }

        private void StagingArea_PromotePlayer(object sender, ServerPlayer e)
        {
            // they are ready to join the game, they have BZDB
            ReleasePlayer?.Invoke(this, e);
        }

        protected void ProcessUpdates()
        {
            bool done = false;
            while (!done)
            {
                if (!SecurityArea.ProcessUpdate() && !StagingArea.ProcessUpdate())
                {
                    lock (this)
                        ProcessingThread = null;
                    return;
                }
                Thread.Sleep(50);
            }
        }
    }
}
