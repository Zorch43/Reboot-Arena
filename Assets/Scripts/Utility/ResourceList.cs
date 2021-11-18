using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Utility
{
    public static class ResourceList
    {
        #region constants
        #region folder paths
        const string ICONS = "Sprites/Icons/";
        const string CURSORS = "Sprites/Cursors/";
        const string MARKERS = "Sprites/Markers/";
        const string DRONES = "Drones/";
        #endregion
        #region resources
        public const string CURSOR_ABILITY_ATTACK = CURSORS + "SpecialAttackCursor";
        public const string MARKER_ABILITY_ATTACK = MARKERS + "SpecialAttackMarker";

        public const string ICON_DEFAULT_ABILITY = ICONS + "DefaultAbilityIcon";
        public const string CURSOR_DEFAULT_ABILITY = CURSORS + "DefaultAbilityCursor";
        public const string MARKER_DEFAULT_ABILITY = MARKERS + "DefaultAbilityMarker";

        public const string ICON_FRAG_GRENADE = ICONS + "FragGrenadeIcon";
        public const string CURSOR_FRAG_GRENADE = CURSOR_ABILITY_ATTACK;
        public const string MARKER_FRAG_GRENADE = MARKER_ABILITY_ATTACK;

        public const string ICON_BUILD_TURRET = ICONS + "BuildTurretIcon";
        public const string CURSOR_BUILD_TURRET = CURSORS + "BuildTurretCursor";
        public const string MARKER_BUILD_TURRET = MARKERS + "BuildTurretMarker";
        public const string DRONE_TURRET = DRONES + "DroneTurret";
        #endregion
        #endregion
        #region public methods

        #endregion
        #region private methods

        #endregion
    }
}
