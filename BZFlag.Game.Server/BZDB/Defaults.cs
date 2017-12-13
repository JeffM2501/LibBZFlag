using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.Game.Host.BZDB
{
    public class Defaults
    {
        public enum StateDBPermission
        {
            ReadWrite,
            Locked,
            ReadOnly
        };

        private static BZFlag.Data.BZDB.Database StateDatabase = null;

        public static void Add(string key, string value, bool persist, StateDBPermission perms)
        {
            StateDatabase.SetValue(key, value);
        }

        public static void Setup(BZFlag.Data.BZDB.Database db)
        {
            StateDatabase = db;

            Add("_agilityAdVel", "2.25", false, StateDBPermission.Locked);
            Add("_agilityTimeWindow", "1.0", false, StateDBPermission.Locked);
            Add("_agilityVelDelta", "0.3", false, StateDBPermission.Locked);
            Add("_ambientLight", "none", false, StateDBPermission.Locked);
            Add("_angleTolerance", "0.05", false, StateDBPermission.Locked);
            Add("_angularAd", "1.5", false, StateDBPermission.Locked);
            Add("_avenueSize", "2.0*_boxBase", false, StateDBPermission.Locked);
            Add("_baseSize", "60.0", false, StateDBPermission.Locked);
            Add("_boxBase", "30.0", false, StateDBPermission.Locked);
            Add("_boxHeight", "6.0*_muzzleHeight", false, StateDBPermission.Locked);
            Add("_burrowAngularAd", "0.55", false, StateDBPermission.Locked);
            Add("_burrowDepth", "-1.32", false, StateDBPermission.Locked);
            Add("_burrowSpeedAd", "0.80", false, StateDBPermission.Locked);
            Add("_coldetDepth", "6", false, StateDBPermission.Locked);
            Add("_coldetElements", "4", false, StateDBPermission.Locked);
            Add("_countdownResumeDelay", "5", false, StateDBPermission.Locked);
            Add("_cullDepth", "6", false, StateDBPermission.Locked);
            Add("_cullDist", "fog", false, StateDBPermission.Locked);
            Add("_cullElements", "16", false, StateDBPermission.Locked);
            Add("_cullOccluders", "0", false, StateDBPermission.Locked);
            Add("_disableBots", "0", false, StateDBPermission.Locked);
            Add("_disableSpeedChecks", "0", false, StateDBPermission.Locked);
            Add("_disableHeightChecks", "0", false, StateDBPermission.Locked);
            Add("_drawCelestial", "1", false, StateDBPermission.Locked);
            Add("_drawClouds", "1", false, StateDBPermission.Locked);
            Add("_drawGround", "1", false, StateDBPermission.Locked);
            Add("_drawGroundLights", "1", false, StateDBPermission.Locked);
            Add("_drawMountains", "1", false, StateDBPermission.Locked);
            Add("_drawSky", "1", false, StateDBPermission.Locked);
            Add("_enableDistanceCheck", "0", false, StateDBPermission.Locked);
            Add("_endShotDetection", "5", false, StateDBPermission.Locked);
            Add("_explodeTime", "5.0", false, StateDBPermission.Locked);
            Add("_flagAltitude", "11.0", false, StateDBPermission.Locked);
            Add("_flagEffectTime", "0.64", false, StateDBPermission.Locked);
            Add("_flagHeight", "10.0", false, StateDBPermission.Locked);
            Add("_flagPoleWidth", "0.025", false, StateDBPermission.Locked);
            Add("_flagPoleSize", "0.8", false, StateDBPermission.Locked);
            Add("_flagRadius", "2.5", false, StateDBPermission.Locked);
            Add("_friction", "0", false, StateDBPermission.Locked);
            Add("_forbidMarkers", "0", false, StateDBPermission.Locked);
            Add("_fogMode", "none", false, StateDBPermission.Locked);
            Add("_fogDensity", "0.001", false, StateDBPermission.Locked);
            Add("_fogStart", "0.5*_worldSize", false, StateDBPermission.Locked);
            Add("_fogEnd", "_worldSize", false, StateDBPermission.Locked);
            Add("_fogColor", "0.25 0.25 0.25", false, StateDBPermission.Locked);
            Add("_fogNoSky", "0", false, StateDBPermission.Locked);
            Add("_forbidIdentify", "0", false, StateDBPermission.Locked);
            Add("_forbidHunting", "0", false, StateDBPermission.Locked);
            Add("_gmActivationTime", "0.5", false, StateDBPermission.Locked);
            Add("_gmAdLife", "0.95", false, StateDBPermission.Locked);
            Add("_gmSize", "1.5", false, StateDBPermission.Locked);
            Add("_gmTurnAngle", "0.628319", false, StateDBPermission.Locked);
            Add("_gravity", "-9.8", false, StateDBPermission.Locked);
            Add("_handicapScoreDiff", "50.0", false, StateDBPermission.Locked);
            Add("_handicapVelAd", "2.0", false, StateDBPermission.Locked);
            Add("_handicapAngAd", "1.5", false, StateDBPermission.Locked);
            Add("_handicapShotAd", "1.75", false, StateDBPermission.Locked);
            Add("_hideFlagsOnRadar", "0", false, StateDBPermission.Locked);
            Add("_hideTeamFlagsOnRadar", "0", false, StateDBPermission.Locked);
            Add("_HTTPIndexResourceDir", "", false, StateDBPermission.Locked);
            Add("_identifyRange", "50.0", false, StateDBPermission.Locked);
            Add("_jumpVelocity", "19.0", false, StateDBPermission.Locked);
            Add("_laserAdLife", "0.1", false, StateDBPermission.Locked);
            Add("_laserAdRate", "0.5", false, StateDBPermission.Locked);
            Add("_laserAdVel", "1000.0", false, StateDBPermission.Locked);
            Add("_latitude", "37.5", false, StateDBPermission.Locked);
            Add("_lockOnAngle", "0.15", false, StateDBPermission.Locked);
            Add("_longitude", "122", false, StateDBPermission.Locked);
            Add("_lRAdRate", "0.5", false, StateDBPermission.Locked);
            Add("_maxBumpHeight", "0.33", false, StateDBPermission.Locked);
            Add("_maxFlagGrabs", "4.0", false, StateDBPermission.Locked);
            Add("_maxLOD", "32767.0", false, StateDBPermission.Locked);
            Add("_maxPlayerAddDelay", "30", false, StateDBPermission.Locked);
            Add("_mirror", "none", false, StateDBPermission.Locked);
            Add("_momentumAngAcc", "1.0", false, StateDBPermission.Locked);
            Add("_momentumLinAcc", "1.0", false, StateDBPermission.Locked);
            Add("_momentumFriction", "0", false, StateDBPermission.Locked);
            Add("_mGunAdLife", "1.0 / _mGunAdRate", false, StateDBPermission.Locked);
            Add("_mGunAdRate", "10.0", false, StateDBPermission.Locked);
            Add("_mGunAdVel", "1.5", false, StateDBPermission.Locked);
            Add("_muzzleFront", "_tankRadius + 0.1", false, StateDBPermission.Locked);
            Add("_muzzleHeight", "1.57", false, StateDBPermission.Locked);
            Add("_noClimb", "1", false, StateDBPermission.Locked);
            Add("_noShadows", "0", false, StateDBPermission.Locked);
            Add("_noSmallPackets", "0", false, StateDBPermission.Locked);
            Add("_notRespondingTime", "5.0", false, StateDBPermission.Locked);
            Add("_obeseFactor", "2.5", false, StateDBPermission.Locked);
            Add("_pauseDropTime", "15.0", false, StateDBPermission.Locked);
            Add("_positionTolerance", "0.09", false, StateDBPermission.Locked);
            Add("_pyrBase", "4.0*_tankHeight", false, StateDBPermission.Locked);
            Add("_pyrHeight", "5.0*_tankHeight", false, StateDBPermission.Locked);
            Add("_rainBaseColor", "none", false, StateDBPermission.Locked);
            Add("_radarLimit", "_worldSize", false, StateDBPermission.Locked);
            Add("_rainRoofs", "1", false, StateDBPermission.Locked);
            Add("_rainDensity", "none", false, StateDBPermission.Locked);
            Add("_rainEndZ", "none", false, StateDBPermission.Locked);
            Add("_rainMaxPuddleTime", "none", false, StateDBPermission.Locked);
            Add("_rainPuddleSpeed", "none", false, StateDBPermission.Locked);
            Add("_rainPuddleColor", "none", false, StateDBPermission.Locked);
            Add("_rainPuddleTexture", "none", false, StateDBPermission.Locked);
            Add("_rainSpread", "none", false, StateDBPermission.Locked);
            Add("_rainSpeed", "none", false, StateDBPermission.Locked);
            Add("_rainSpeedMod", "none", false, StateDBPermission.Locked);
            Add("_rainSpins", "none", false, StateDBPermission.Locked);
            Add("_rainStartZ", "none", false, StateDBPermission.Locked);
            Add("_rainTexture", "none", false, StateDBPermission.Locked);
            Add("_rainTopColor", "none", false, StateDBPermission.Locked);
            Add("_rainType", "none", false, StateDBPermission.Locked);
            Add("_rejoinTime", "_explodeTime", false, StateDBPermission.Locked);
            Add("_reloadTime", "_shotRange / _shotSpeed", false, StateDBPermission.Locked);
            Add("_rFireAdLife", "1.0 / _rFireAdRate", false, StateDBPermission.Locked);
            Add("_rFireAdRate", "2.0", false, StateDBPermission.Locked);
            Add("_rFireAdVel", "1.5", false, StateDBPermission.Locked);
            Add("_shieldFlight", "2.7", false, StateDBPermission.Locked);
            Add("_shockAdLife", "0.2", false, StateDBPermission.Locked);
            Add("_shockInRadius", "_tankLength", false, StateDBPermission.Locked);
            Add("_shockOutRadius", "60.0", false, StateDBPermission.Locked);
            Add("_shotTailLength", "4.0", false, StateDBPermission.Locked);
            Add("_shotRadius", "0.5", false, StateDBPermission.Locked);
            Add("_shotRange", "350.0", false, StateDBPermission.Locked);
            Add("_shotSpeed", "100.0", false, StateDBPermission.Locked);
            Add("_shotsKeepVerticalVelocity", "0", false, StateDBPermission.Locked);
            Add("_skyColor", "white", false, StateDBPermission.Locked);
            Add("_spawnMaxCompTime", "0.01", false, StateDBPermission.Locked);
            Add("_spawnSafeRadMod", "20", false, StateDBPermission.Locked);
            Add("_spawnSafeSRMod", "3", false, StateDBPermission.Locked);
            Add("_spawnSafeSWMod", "1.5", false, StateDBPermission.Locked);
            Add("_speedChecksLogOnly", "0", false, StateDBPermission.Locked);
            Add("_srRadiusMult", "2.0", false, StateDBPermission.Locked);
            Add("_squishFactor", "1.0", false, StateDBPermission.Locked);
            Add("_squishTime", "1.0", false, StateDBPermission.Locked);
            Add("_syncTime", "-1.0", false, StateDBPermission.Locked);
            Add("_syncLocation", "0", false, StateDBPermission.Locked);
            Add("_tankExplosionSize", "3.5 * _tankLength", false, StateDBPermission.Locked);
            Add("_tankAngVel", "0.785398", false, StateDBPermission.Locked);
            Add("_tankHeight", "2.05", false, StateDBPermission.Locked);
            Add("_tankLength", "6.0", false, StateDBPermission.Locked);
            Add("_tankRadius", "0.72 * _tankLength", false, StateDBPermission.Locked);
            Add("_tankSpeed", "25.0", false, StateDBPermission.Locked);
            Add("_tankWidth", "2.8", false, StateDBPermission.Locked);
            Add("_targetingAngle", "0.3", false, StateDBPermission.Locked);
            Add("_targetingDistance", "300.00", false, StateDBPermission.Locked);
            Add("_teleportBreadth", "4.48", false, StateDBPermission.Locked);
            Add("_teleportHeight", "10.08", false, StateDBPermission.Locked);
            Add("_teleportTime", "1.0", false, StateDBPermission.Locked);
            Add("_teleportWidth", "1.12", false, StateDBPermission.Locked);
            Add("_thiefAdLife", "0.05", false, StateDBPermission.Locked);
            Add("_thiefAdRate", "12.0", false, StateDBPermission.Locked);
            Add("_thiefAdShotVel", "8.0", false, StateDBPermission.Locked);
            Add("_thiefTinyFactor", "0.5", false, StateDBPermission.Locked);
            Add("_thiefVelAd", "1.67", false, StateDBPermission.Locked);
            Add("_thiefDropTime", "_reloadTime * 0.5", false, StateDBPermission.Locked);
            Add("_tinyFactor", "0.4", false, StateDBPermission.Locked);
            Add("_trackFade", "3.0", false, StateDBPermission.Locked);
            Add("_updateThrottleRate", "30.0", false, StateDBPermission.Locked);
            Add("_useLineRain", "none", false, StateDBPermission.Locked);
            Add("_useRainPuddles", "none", false, StateDBPermission.Locked);
            Add("_useRainBillboards", "none", false, StateDBPermission.Locked);
            Add("_velocityAd", "1.5", false, StateDBPermission.Locked);
            Add("_wallHeight", "3.0*_tankHeight", false, StateDBPermission.Locked);
            Add("_weapons", "1", false, StateDBPermission.Locked);
            Add("_wideAngleAng", "1.745329", false, StateDBPermission.Locked);
            Add("_wingsGravity", "_gravity", false, StateDBPermission.Locked);
            Add("_wingsJumpCount", "1", false, StateDBPermission.Locked);
            Add("_wingsJumpVelocity", "_jumpVelocity", false, StateDBPermission.Locked);
            Add("_wingsSlideTime", "0.0", false, StateDBPermission.Locked);
            Add("_worldSize", "800.0", false, StateDBPermission.Locked);
        }
    }
}
