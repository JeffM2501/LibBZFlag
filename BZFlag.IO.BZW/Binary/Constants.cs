using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZFlag.IO.BZW.Binary
{
    public static class Constants
    {
        // world database codes

        public static readonly UInt16 WorldCodeHeader = 0x6865;        // 'he'
        public static readonly UInt16 WorldCodeBase = 0x6261;          // 'ba'
        public static readonly UInt16 WorldCodeBox = 0x6278;           // 'bx'
        public static readonly UInt16 WorldCodeEnd = 0x6564;           // 'ed'
        public static readonly UInt16 WorldCodeLink = 0x6c6e;          // 'ln'
        public static readonly UInt16 WorldCodePyramid = 0x7079;       // 'py'
        public static readonly UInt16 WorldCodeMesh = 0x6D65;          // 'me'
        public static readonly UInt16 WorldCodeArc = 0x6172;           // 'ar'
        public static readonly UInt16 WorldCodeCone = 0x636e;          // 'cn'
        public static readonly UInt16 WorldCodeSphere = 0x7370;        // 'sp'
        public static readonly UInt16 WorldCodeTetra = 0x7468;     // 'th'
        public static readonly UInt16 WorldCodeTeleporter = 0x7465;        // 'te'
        public static readonly UInt16 WorldCodeWall = 0x776c;          // 'wl'
        public static readonly UInt16 WorldCodeWeapon = 0x7765;        // 'we'
        public static readonly UInt16 WorldCodeZone = 0x7A6e;          // 'zn'
        public static readonly UInt16 WorldCodeGroup = 0x6772;     // 'gr'
        public static readonly UInt16 WorldCodeGroupDefStart = 0x6473; // 'ds'
        public static readonly UInt16 WorldCodeGroupDefEnd = 0x6465;        // 'de'


        // world database sizes
        public static readonly UInt16 WorldSettingsSize = 30;
        public static readonly UInt16 WorldCodeHeaderSize = 10;
        public static readonly UInt16 WorldCodeBaseSize = 31;
        public static readonly UInt16 WorldCodeWallSize = 24;
        public static readonly UInt16 WorldCodeBoxSize = 29;
        public static readonly UInt16 WorldCodeEndSize = 0;
        public static readonly UInt16 WorldCodePyramidSize = 29;
        public static readonly UInt16 WorldCodeMeshSize = 0xA5;  // dummy value, sizes are variable
        public static readonly UInt16 WorldCodeArcSize = 85;
        public static readonly UInt16 WorldCodeConeSize = 65;
        public static readonly UInt16 WorldCodeSphereSize = 53;
        public static readonly UInt16 WorldCodeTetraSize = 66;
        public static readonly UInt16 WorldCodeTeleporterSize = 34;
        public static readonly UInt16 WorldCodeLinkSize = 4;
        public static readonly UInt16 WorldCodeWeaponSize = 24;  // basic size, not including lists
        public static readonly UInt16 WorldCodeZoneSize = 34;    // basic size, not including lists


        public static readonly byte DRIVE_THRU = (1 << 0);
        public static readonly byte SHOOT_THRU = (1 << 1);
        public static readonly byte FLIP_Z = (1 << 2);
        public static readonly byte RICOCHET = (1 << 3);
    }
}
