using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BZFlag.Data.Types;

using BZFlag.LinearMath;

namespace BZFlag.Data.Players
{
    public enum PlayerTypes
    {
        TankPlayer = 0,
        ComputerPlayer = 1,
        Unknown = Byte.MaxValue,
    };

    public enum ShotTypes
    {
        NormalShot = 0,
        SpecialShot = 1,
        Unknown = Byte.MaxValue,
    };

    [Flags]
    public enum PlayerStatuses
    {           // bit masks
        DeadStatus = 0,     // not alive, not paused, etc.
        Alive = (1 << 0),   // player is alive
        Paused = (1 << 1),  // player is paused
        Exploding = (1 << 2),   // currently blowing up
        Teleporting = (1 << 3), // teleported recently
        FlagActive = (1 << 4),  // flag special powers active
        CrossingWall = (1 << 5),    // tank crossing building wall
        Falling = (1 << 6), // tank accel'd by gravity
        OnDriver = (1 << 7),    // tank is on a physics driver
        UserInputs = (1 << 8),  // user speed and angvel are sent
        JumpJets = (1 << 9),    // tank has jump jets on
        PlaySound = (1 << 10),	// play one or more sounds
    };

    [Flags]
    public enum PlayerStatusSounds
    {
        NoSounds = 0,
        JumpSound = (1 << 0),
        WingsSound = (1 << 1),
        BounceSound = (1 << 2),
    };

    public enum BlowedUpReasons
    {
        GotKilledMsg = 0,
        GotShot = 1,
        GotRunOver = 2,
        GotCaptured = 3,
        GenocideEffect = 4,
        SelfDestruct = 5,
        WaterDeath = 6,
        LastReason = 7,
        DeathTouch = 8,
        Unknown = 255,
    };

    [Flags]
    public enum PlayerAttributes
    {
        Unknown = 0,
        IsRegistered = 0x02,
        IsVerified = 0x04,
        IsAdmin = 0x08,
    };

}
