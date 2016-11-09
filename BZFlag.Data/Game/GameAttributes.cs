using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Data.Game
{
    public enum GameTypes
    {
        Unknown = -1,
        TeamFFA = 0,       // normal teamed FFA
        ClassicCTF = 1,    // your normal CTF
        OpenFFA = 2,       // teamless FFA
        RabbitChase = 3,    // hunt the rabbit mode
    };

    [Flags]
    public enum GameOptionFlags
    {
        NoStyle = 0,
        SuperFlagGameStyle = 0x0002, // superflags allowed
        JumpingGameStyle = 0x0008, // jumping allowed
        InertiaGameStyle = 0x0010, // momentum for all
        RicochetGameStyle = 0x0020, // all shots ricochet
        ShakableGameStyle = 0x0040, // can drop bad flags
        AntidoteGameStyle = 0x0080, // anti-bad flags
        HandicapGameStyle = 0x0100, // handicap players based on score (eek! was TimeSyncGameStyle)
        NoTeamKillsGameStyle = 0x0400
        // add here before reusing old ones above
    };
}
