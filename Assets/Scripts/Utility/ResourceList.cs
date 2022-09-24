using Assets.Scripts.Data_Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    public static class ResourceList
    {
        #region constants
        #region folder paths
        const string ICONS = "Sprites/Icons/";
        const string CURSORS = "Sprites/Cursors/";
        const string MARKERS = "Sprites/Markers/";
        const string PORTRAITS = "Sprites/Portraits/";
        const string SYMBOLS = "Sprites/Symbols/";

        const string UNITS = "Units/";
        const string DRONES = "Drones/";
        const string HOLOGRAMS = DRONES + "Holograms/";
        const string PICKUPS = "Pickups/";
        const string EFFECTS = "Effects/";
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

        public const string ICON_RELOAD = ICONS + "ReloadIcon";
        public const string EFFECT_RELOAD = EFFECTS + "ReloadAbilityEffect";

        public const string ICON_BUILD_TURRET = ICONS + "BuildTurretIcon";
        public const string CURSOR_BUILD_TURRET = CURSORS + "BuildTurretCursor";
        public const string MARKER_BUILD_TURRET = MARKERS + "BuildTurretMarker";
        public const string DRONE_TURRET = DRONES + "DroneTurret";
        public const string HOLOGRAM_TURRET = HOLOGRAMS + "TurretHologram";

        public const string ICON_THROW_NANOPACK = ICONS + "ThrowNanoPackIcon";

        public const string UNIT_TROOPER = UNITS + "UnitTrooper";
        public const string PORTRAIT_TROOPER = PORTRAITS + "PortraitTrooper";
        public const string SYMBOL_TROOPER = SYMBOLS + "SymbolTrooper";
        public const string EFFECT_KILLSTREAK = EFFECTS + "KillStreakConditionEffect";

        public const string UNIT_FABRICATOR = UNITS + "UnitFabricator";
        public const string PORTRAIT_FABRICATOR = PORTRAITS + "PortraitFabricator";
        public const string SYMBOL_FABRICATOR = SYMBOLS + "SymbolFabricator";

        public const string UNIT_RANGER = UNITS + "UnitRanger";
        public const string PORTRAIT_RANGER = PORTRAITS + "PortraitRanger";
        public const string SYMBOL_RANGER = SYMBOLS + "SymbolRanger";

        public const string PICKUP_NANOPACK = PICKUPS + "HealthPack";
        public const string PICKUP_AMMOPACK = PICKUPS + "AmmoPack";

        
        #endregion
        #endregion
        #region public methods
        public static UnitController GetUnitTemplate(UnitClassTemplates.UnitClasses unitClass)
        {
            string path = "";
            switch (unitClass)
            {
                case UnitClassTemplates.UnitClasses.Trooper:
                    path = UNIT_TROOPER;
                    break;
                case UnitClassTemplates.UnitClasses.Fabricator:
                    path = UNIT_FABRICATOR;
                    break;
                case UnitClassTemplates.UnitClasses.Ranger:
                    path = UNIT_RANGER;
                    break;
                default:
                    Debug.LogError("Invalid class type: " + unitClass.ToString());
                    return null;
            }
            var unitTemplate = Resources.Load<UnitController>(path);
            return unitTemplate;
        }
        public static PickupController GetPickupTemplate(PickupController.PickupType pickupType)
        {
            string path = "";
            switch (pickupType)
            {
                case PickupController.PickupType.NanoPack:
                    path = PICKUP_NANOPACK;
                    break;
                case PickupController.PickupType.AmmoPack:
                    path = PICKUP_AMMOPACK;
                    break;
                default:
                    Debug.LogError("Unsupported pickup type: " + pickupType.ToString());
                    return null;
            }
            var pickupTemplate = Resources.Load<PickupController>(path);
            return pickupTemplate;
        }
        #endregion
        #region private methods

        #endregion
    }
}
