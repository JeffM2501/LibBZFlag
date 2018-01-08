using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BZFlag.Game.Host.Players;
using BZFlag.Game.Host;
using BZFlag.Game.Host.API;

namespace LatancyManagement
{
    public class Module : PlugIn
    {
        public override string Name => "LatencyManagement";
        public override string Description => "Lag stats and management";

        public override void Startup(Server serverInstance)
        {
            State.Players.PlayerLagUpdated += Players_PlayerLagUpdated;
        }

        public class LatencyInfo
        {
            public static readonly string Name = "LatancyManagement.LatencyInfo";

            public int Warnings = 0;
            public double LastPacketLoss = double.MinValue;
        }

        public static double WarnLag = -1;
        public static double WarningPacketLoss = -1;

        public static int MaxWarnings = 5;

        private void Players_PlayerLagUpdated(object sender, ServerPlayer e)
        {
            LatencyInfo info = e.GetTag<LatencyInfo>(LatencyInfo.Name);

            bool didWarn = false;

            if (WarnLag > 0)
            {
                if (e.Lag.InstantLagTime > WarnLag && e.Lag.AverageLag > WarnLag)
                {
                    info.Warnings++;
                    didWarn = false;
                }
            }

            if(WarningPacketLoss >  0)
            {
                if (e.Lag.TotalPacketLoss > WarningPacketLoss)
                {
                    if (e.Lag.TotalPacketLoss > info.LastPacketLoss)
                    {
                        info.Warnings++;
                        didWarn = false;
                    }
                    info.LastPacketLoss = e.Lag.TotalPacketLoss;
                }
            }

            if (info.Warnings >= MaxWarnings)
            {
                State.Chat.SendChatToUser(null, e, Resources.KickMessage, false);
                e.FlushTCP();
                e.Disconnect();
            }
            else if (didWarn)
            {
                State.Chat.SendChatToUser(null, e, Resources.WarnMessage, false);
            }
        }
    }
}
