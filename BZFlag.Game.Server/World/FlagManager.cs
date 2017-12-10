
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BZFlag.Networking.Messages.BZFS.Info;
using BZFlag.Data.Flags;

namespace BZFlag.Game.Host.World
{
    public class FlagManager
    {
        public MsgNegotiateFlags GetFlagNegotiation(MsgNegotiateFlags inFlags)
        {
            MsgNegotiateFlags outFlags = new MsgNegotiateFlags();

            foreach (var flag in FlagTypeList.Flags)
            {
                if (flag.FlagAbbv != string.Empty && !inFlags.Contains(flag.FlagAbbv))
                    outFlags.FlagAbrevs.Add(flag.FlagAbbv);
            }

            return outFlags;
        }
    }
}
