using Assets.Scripts.Data_Models;
using Assets.Scripts.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data_Templates
{
    public static class UnitAbilityTemplates
    {
        public static UnitAbilityModel CreateFragGrenade()
        {
            var ability = new UnitAbilityModel()
            {
                Name = "Grenade",
                Description = "Throw an explosive grenade at the target location",
                Icon = ResourceList.ICON_FRAG_GRENADE,
                Cursor = ResourceList.CURSOR_FRAG_GRENADE,
                Marker = ResourceList.MARKER_FRAG_GRENADE,
                IsTargetedAbility = true,
                IsWeaponAbility = true,
                GroupActivationRule = UnitAbilityModel.GroupActivationType.Single,
                ConsiderMostAmmoInGroup = true,
                ConsiderLeastHealthInGroup = true,
                ConsiderLeastMoveDistanceToTarget = true,
                AbilityWeapon = WeaponTemplates.CreateFragGrenade(),
                AmmoCostInstant = 100
            };
            return ability;
        }
        public static UnitAbilityModel CreateTurretDrone()
        {
            var ability = new UnitAbilityModel()
            {
                Name = "Turret",
                Description = "Build a structuer that clocks enemy fire and can be upgraded to equip a plasma turret",
                Icon = ResourceList.ICON_BUILD_TURRET,
                Cursor = ResourceList.CURSOR_BUILD_TURRET,
                Marker = ResourceList.MARKER_BUILD_TURRET,
                IsBuildAbility = true,
                IsWeaponAbility = true,//uses weapon range
                DroneTemplate = ResourceList.DRONE_TURRET,
                DroneHologram = ResourceList.HOLOGRAM_TURRET,
                IsTargetedAbility = true,
                GroupActivationRule = UnitAbilityModel.GroupActivationType.Single,
                ConsiderMostAmmoInGroup = true,
                ConsiderLeastTotalDistanceToTarget = true,
                ConsiderMostHealthInGroup = true,
                AbilityWeapon = WeaponTemplates.CreateBuildTools(),
                AmmoCostInstant = 100
            };
            return ability;
        }
        public static UnitAbilityModel CreateNanoPack()
        {
            var ability = new UnitAbilityModel()
            {
                Name = "NanoPack",
                Description = "Throw a NanoPack to a random location within a short range",
                Icon = ResourceList.ICON_THROW_NANOPACK,
                GroupActivationRule = UnitAbilityModel.GroupActivationType.Single,
                LootDrop = new PickupController.PickupType[] {PickupController.PickupType.NanoPack},
                IsNonInterrupting = true,
                ConsiderMostAmmoInGroup = true,
                ConsiderLeastHealthInGroup = true,
                AmmoCostInstant = 50
            };
            return ability;
        }
    }
}
