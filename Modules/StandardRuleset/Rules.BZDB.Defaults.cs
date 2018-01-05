using BZFlag.Game.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZFS.StandardRuleset
{
    public partial class Rules
    {

        public enum StateDBPermission
        {
            ReadWrite,
            Locked,
            ReadOnly
        };

        private static BZFlag.Data.BZDB.Database StateDatabase = null;

        public static void SetInitalBZDB(string key, string value, bool persist, StateDBPermission perms, bool isStatic = false)
        {
            StateDatabase.InitValues(key, value, isStatic);
        }

        protected virtual void LoadBZDBDefaults(Server host, BZFlag.Data.BZDB.Database db)
        {
            StateDatabase = db;

            SetInitalBZDB("_agilityAdVel", "2.25", false, StateDBPermission.Locked);
            SetInitalBZDB("_agilityTimeWindow", "1.0", false, StateDBPermission.Locked);
            SetInitalBZDB("_agilityVelDelta", "0.3", false, StateDBPermission.Locked);
            SetInitalBZDB("_ambientLight", "none", false, StateDBPermission.Locked);
            SetInitalBZDB("_angleTolerance", "0.05", false, StateDBPermission.Locked);
            SetInitalBZDB("_angularAd", "1.5", false, StateDBPermission.Locked);
            SetInitalBZDB("_avenueSize", "2.0*_boxBase", false, StateDBPermission.Locked);
            SetInitalBZDB("_baseSize", "60.0", false, StateDBPermission.Locked);
            SetInitalBZDB("_boxBase", "30.0", false, StateDBPermission.Locked);
            SetInitalBZDB("_boxHeight", "6.0*_muzzleHeight", false, StateDBPermission.Locked);
            SetInitalBZDB("_burrowAngularAd", "0.55", false, StateDBPermission.Locked);
            SetInitalBZDB("_burrowDepth", "-1.32", false, StateDBPermission.Locked);
            SetInitalBZDB("_burrowSpeedAd", "0.80", false, StateDBPermission.Locked);
            SetInitalBZDB("_coldetDepth", "6", false, StateDBPermission.Locked);
            SetInitalBZDB("_coldetElements", "4", false, StateDBPermission.Locked);
            SetInitalBZDB("_countdownResumeDelay", "5", false, StateDBPermission.Locked);
            SetInitalBZDB("_cullDepth", "6", false, StateDBPermission.Locked);
            SetInitalBZDB("_cullDist", "fog", false, StateDBPermission.Locked);
            SetInitalBZDB("_cullElements", "16", false, StateDBPermission.Locked);
            SetInitalBZDB("_cullOccluders", "0", false, StateDBPermission.Locked);
            SetInitalBZDB("_disableBots", "0", false, StateDBPermission.Locked);
            SetInitalBZDB("_disableSpeedChecks", "0", false, StateDBPermission.Locked);
            SetInitalBZDB("_disableHeightChecks", "0", false, StateDBPermission.Locked);
            SetInitalBZDB("_drawCelestial", "1", false, StateDBPermission.Locked);
            SetInitalBZDB("_drawClouds", "1", false, StateDBPermission.Locked);
            SetInitalBZDB("_drawGround", "1", false, StateDBPermission.Locked);
            SetInitalBZDB("_drawGroundLights", "1", false, StateDBPermission.Locked);
            SetInitalBZDB("_drawMountains", "1", false, StateDBPermission.Locked);
            SetInitalBZDB("_drawSky", "1", false, StateDBPermission.Locked);
            SetInitalBZDB("_enableDistanceCheck", "0", false, StateDBPermission.Locked);
            SetInitalBZDB("_endShotDetection", "5", false, StateDBPermission.Locked);
            SetInitalBZDB("_explodeTime", "5.0", false, StateDBPermission.Locked);
            SetInitalBZDB("_flagAltitude", "11.0", false, StateDBPermission.Locked);
            SetInitalBZDB("_flagEffectTime", "0.64", false, StateDBPermission.Locked);
            SetInitalBZDB("_flagHeight", "10.0", false, StateDBPermission.Locked, true);
            SetInitalBZDB("_flagPoleWidth", "0.025", false, StateDBPermission.Locked, true);
            SetInitalBZDB("_flagPoleSize", "0.8", false, StateDBPermission.Locked, true);
            SetInitalBZDB("_flagRadius", "2.5", false, StateDBPermission.Locked,true);
            SetInitalBZDB("_friction", "0", false, StateDBPermission.Locked);
            SetInitalBZDB("_forbidMarkers", "0", false, StateDBPermission.Locked);
            SetInitalBZDB("_fogMode", "none", false, StateDBPermission.Locked);
            SetInitalBZDB("_fogDensity", "0.001", false, StateDBPermission.Locked);
            SetInitalBZDB("_fogStart", "0.5*_worldSize", false, StateDBPermission.Locked);
            SetInitalBZDB("_fogEnd", "_worldSize", false, StateDBPermission.Locked);
            SetInitalBZDB("_fogColor", "0.25 0.25 0.25", false, StateDBPermission.Locked);
            SetInitalBZDB("_fogNoSky", "0", false, StateDBPermission.Locked);
            SetInitalBZDB("_forbidIdentify", "0", false, StateDBPermission.Locked);
            SetInitalBZDB("_forbidHunting", "0", false, StateDBPermission.Locked);
            SetInitalBZDB("_gmActivationTime", "0.5", false, StateDBPermission.Locked);
            SetInitalBZDB("_gmAdLife", "0.95", false, StateDBPermission.Locked);
            SetInitalBZDB("_gmSize", "1.5", false, StateDBPermission.Locked);
            SetInitalBZDB("_gmTurnAngle", "0.628319", false, StateDBPermission.Locked);
            SetInitalBZDB("_gravity", "-9.8", false, StateDBPermission.Locked);
            SetInitalBZDB("_handicapScoreDiff", "50.0", false, StateDBPermission.Locked);
            SetInitalBZDB("_handicapVelAd", "2.0", false, StateDBPermission.Locked);
            SetInitalBZDB("_handicapAngAd", "1.5", false, StateDBPermission.Locked);
            SetInitalBZDB("_handicapShotAd", "1.75", false, StateDBPermission.Locked);
            SetInitalBZDB("_hideFlagsOnRadar", "0", false, StateDBPermission.Locked);
            SetInitalBZDB("_hideTeamFlagsOnRadar", "0", false, StateDBPermission.Locked);
            SetInitalBZDB("_HTTPIndexResourceDir", "", false, StateDBPermission.Locked);
            SetInitalBZDB("_identifyRange", "50.0", false, StateDBPermission.Locked);
            SetInitalBZDB("_jumpVelocity", "19.0", false, StateDBPermission.Locked);
            SetInitalBZDB("_laserAdLife", "0.1", false, StateDBPermission.Locked);
            SetInitalBZDB("_laserAdRate", "0.5", false, StateDBPermission.Locked);
            SetInitalBZDB("_laserAdVel", "1000.0", false, StateDBPermission.Locked);
            SetInitalBZDB("_latitude", "37.5", false, StateDBPermission.Locked);
            SetInitalBZDB("_lockOnAngle", "0.15", false, StateDBPermission.Locked);
            SetInitalBZDB("_longitude", "122", false, StateDBPermission.Locked);
            SetInitalBZDB("_lRAdRate", "0.5", false, StateDBPermission.Locked);
            SetInitalBZDB("_maxBumpHeight", "0.33", false, StateDBPermission.Locked);
            SetInitalBZDB("_maxFlagGrabs", "4.0", false, StateDBPermission.Locked);
            SetInitalBZDB("_maxLOD", "32767.0", false, StateDBPermission.Locked);
            SetInitalBZDB("_maxPlayerAddDelay", "30", false, StateDBPermission.Locked);
            SetInitalBZDB("_mirror", "none", false, StateDBPermission.Locked);
            SetInitalBZDB("_momentumAngAcc", "1.0", false, StateDBPermission.Locked);
            SetInitalBZDB("_momentumLinAcc", "1.0", false, StateDBPermission.Locked);
            SetInitalBZDB("_momentumFriction", "0", false, StateDBPermission.Locked);
            SetInitalBZDB("_mGunAdLife", "1.0 / _mGunAdRate", false, StateDBPermission.Locked);
            SetInitalBZDB("_mGunAdRate", "10.0", false, StateDBPermission.Locked);
            SetInitalBZDB("_mGunAdVel", "1.5", false, StateDBPermission.Locked);
            SetInitalBZDB("_muzzleFront", "_tankRadius + 0.1", false, StateDBPermission.Locked);
            SetInitalBZDB("_muzzleHeight", "1.57", false, StateDBPermission.Locked);
            SetInitalBZDB("_noClimb", "1", false, StateDBPermission.Locked);
            SetInitalBZDB("_noShadows", "0", false, StateDBPermission.Locked);
            SetInitalBZDB("_noSmallPackets", "0", false, StateDBPermission.Locked);
            SetInitalBZDB("_notRespondingTime", "5.0", false, StateDBPermission.Locked);
            SetInitalBZDB("_obeseFactor", "2.5", false, StateDBPermission.Locked);
            SetInitalBZDB("_pauseDropTime", "15.0", false, StateDBPermission.Locked);
            SetInitalBZDB("_positionTolerance", "0.09", false, StateDBPermission.Locked);
            SetInitalBZDB("_pyrBase", "4.0*_tankHeight", false, StateDBPermission.Locked);
            SetInitalBZDB("_pyrHeight", "5.0*_tankHeight", false, StateDBPermission.Locked);
            SetInitalBZDB("_rainBaseColor", "none", false, StateDBPermission.Locked);
            SetInitalBZDB("_radarLimit", "_worldSize", false, StateDBPermission.Locked);
            SetInitalBZDB("_rainRoofs", "1", false, StateDBPermission.Locked);
            SetInitalBZDB("_rainDensity", "none", false, StateDBPermission.Locked);
            SetInitalBZDB("_rainEndZ", "none", false, StateDBPermission.Locked);
            SetInitalBZDB("_rainMaxPuddleTime", "none", false, StateDBPermission.Locked);
            SetInitalBZDB("_rainPuddleSpeed", "none", false, StateDBPermission.Locked);
            SetInitalBZDB("_rainPuddleColor", "none", false, StateDBPermission.Locked);
            SetInitalBZDB("_rainPuddleTexture", "none", false, StateDBPermission.Locked);
            SetInitalBZDB("_rainSpread", "none", false, StateDBPermission.Locked);
            SetInitalBZDB("_rainSpeed", "none", false, StateDBPermission.Locked);
            SetInitalBZDB("_rainSpeedMod", "none", false, StateDBPermission.Locked);
            SetInitalBZDB("_rainSpins", "none", false, StateDBPermission.Locked);
            SetInitalBZDB("_rainStartZ", "none", false, StateDBPermission.Locked);
            SetInitalBZDB("_rainTexture", "none", false, StateDBPermission.Locked);
            SetInitalBZDB("_rainTopColor", "none", false, StateDBPermission.Locked);
            SetInitalBZDB("_rainType", "none", false, StateDBPermission.Locked);
            SetInitalBZDB("_rejoinTime", "_explodeTime", false, StateDBPermission.Locked);
            SetInitalBZDB("_reloadTime", "_shotRange / _shotSpeed", false, StateDBPermission.Locked);
            SetInitalBZDB("_rFireAdLife", "1.0 / _rFireAdRate", false, StateDBPermission.Locked);
            SetInitalBZDB("_rFireAdRate", "2.0", false, StateDBPermission.Locked);
            SetInitalBZDB("_rFireAdVel", "1.5", false, StateDBPermission.Locked);
            SetInitalBZDB("_shieldFlight", "2.7", false, StateDBPermission.Locked);
            SetInitalBZDB("_shockAdLife", "0.2", false, StateDBPermission.Locked);
            SetInitalBZDB("_shockInRadius", "_tankLength", false, StateDBPermission.Locked);
            SetInitalBZDB("_shockOutRadius", "60.0", false, StateDBPermission.Locked);
            SetInitalBZDB("_shotTailLength", "4.0", false, StateDBPermission.Locked);
            SetInitalBZDB("_shotRadius", "0.5", false, StateDBPermission.Locked);
            SetInitalBZDB("_shotRange", "350.0", false, StateDBPermission.Locked);
            SetInitalBZDB("_shotSpeed", "100.0", false, StateDBPermission.Locked);
            SetInitalBZDB("_shotsKeepVerticalVelocity", "0", false, StateDBPermission.Locked);
            SetInitalBZDB("_skyColor", "white", false, StateDBPermission.Locked);
            SetInitalBZDB("_spawnMaxCompTime", "0.01", false, StateDBPermission.Locked);
            SetInitalBZDB("_spawnSafeRadMod", "20", false, StateDBPermission.Locked);
            SetInitalBZDB("_spawnSafeSRMod", "3", false, StateDBPermission.Locked);
            SetInitalBZDB("_spawnSafeSWMod", "1.5", false, StateDBPermission.Locked);
            SetInitalBZDB("_speedChecksLogOnly", "0", false, StateDBPermission.Locked);
            SetInitalBZDB("_srRadiusMult", "2.0", false, StateDBPermission.Locked);
            SetInitalBZDB("_squishFactor", "1.0", false, StateDBPermission.Locked);
            SetInitalBZDB("_squishTime", "1.0", false, StateDBPermission.Locked);
            SetInitalBZDB("_syncTime", "-1.0", false, StateDBPermission.Locked);
            SetInitalBZDB("_syncLocation", "0", false, StateDBPermission.Locked);
            SetInitalBZDB("_tankExplosionSize", "3.5 * _tankLength", false, StateDBPermission.Locked);
            SetInitalBZDB("_tankAngVel", "0.785398", false, StateDBPermission.Locked);
            SetInitalBZDB("_tankHeight", "2.05", false, StateDBPermission.Locked);
            SetInitalBZDB("_tankLength", "6.0", false, StateDBPermission.Locked);
            SetInitalBZDB("_tankRadius", "0.72 * _tankLength", false, StateDBPermission.Locked, true);
            SetInitalBZDB("_tankSpeed", "25.0", false, StateDBPermission.Locked);
            SetInitalBZDB("_tankWidth", "2.8", false, StateDBPermission.Locked);
            SetInitalBZDB("_targetingAngle", "0.3", false, StateDBPermission.Locked);
            SetInitalBZDB("_targetingDistance", "300.00", false, StateDBPermission.Locked);
            SetInitalBZDB("_teleportBreadth", "4.48", false, StateDBPermission.Locked);
            SetInitalBZDB("_teleportHeight", "10.08", false, StateDBPermission.Locked);
            SetInitalBZDB("_teleportTime", "1.0", false, StateDBPermission.Locked);
            SetInitalBZDB("_teleportWidth", "1.12", false, StateDBPermission.Locked);
            SetInitalBZDB("_thiefAdLife", "0.05", false, StateDBPermission.Locked);
            SetInitalBZDB("_thiefAdRate", "12.0", false, StateDBPermission.Locked);
            SetInitalBZDB("_thiefAdShotVel", "8.0", false, StateDBPermission.Locked);
            SetInitalBZDB("_thiefTinyFactor", "0.5", false, StateDBPermission.Locked);
            SetInitalBZDB("_thiefVelAd", "1.67", false, StateDBPermission.Locked);
            SetInitalBZDB("_thiefDropTime", "_reloadTime * 0.5", false, StateDBPermission.Locked);
            SetInitalBZDB("_tinyFactor", "0.4", false, StateDBPermission.Locked);
            SetInitalBZDB("_trackFade", "3.0", false, StateDBPermission.Locked);
            SetInitalBZDB("_updateThrottleRate", "30.0", false, StateDBPermission.Locked);
            SetInitalBZDB("_useLineRain", "none", false, StateDBPermission.Locked);
            SetInitalBZDB("_useRainPuddles", "none", false, StateDBPermission.Locked);
            SetInitalBZDB("_useRainBillboards", "none", false, StateDBPermission.Locked);
            SetInitalBZDB("_velocityAd", "1.5", false, StateDBPermission.Locked);
            SetInitalBZDB("_wallHeight", "3.0*_tankHeight", false, StateDBPermission.Locked);
            SetInitalBZDB("_weapons", "1", false, StateDBPermission.Locked);
            SetInitalBZDB("_wideAngleAng", "1.745329", false, StateDBPermission.Locked);
            SetInitalBZDB("_wingsGravity", "_gravity", false, StateDBPermission.Locked);
            SetInitalBZDB("_wingsJumpCount", "1", false, StateDBPermission.Locked);
            SetInitalBZDB("_wingsJumpVelocity", "_jumpVelocity", false, StateDBPermission.Locked);
            SetInitalBZDB("_wingsSlideTime", "0.0", false, StateDBPermission.Locked);
            SetInitalBZDB("_worldSize", "800.0", false, StateDBPermission.Locked);
        }
    }
}
