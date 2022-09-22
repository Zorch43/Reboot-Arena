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
                AmmoCostInstant = 100,
                EventNameKey = EventList.EventNames.OnInputKeyAbilityGrenade,
                EventNameUI = EventList.EventNames.OnInputUIAbilityGrenade,
                EventNameSet = EventList.EventNames.OnInputAbilityGrenadeSet
            };
            return ability;
        }
        public static UnitAbilityModel CreateReload()
        {
            var ability = new UnitAbilityModel()
            {
                Name = "Reload",
                Description = "Stop to restore your ammo.  You can't do anything else while reloading.",
                Icon = ResourceList.ICON_RELOAD,
                IsTargetedAbility = false
                
                //TODO: shortcuts?
            };
            //upon activation, apply reload condition
            ability.OnActivation += (sender, a) =>
            {
                var unit = sender as UnitController;
                unit.ApplyCondition(ConditionTemplates.CreateReloadCondition());
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
                AmmoCostInstant = 100,
                EventNameKey = EventList.EventNames.OnInputKeyAbilityTurret,
                EventNameUI = EventList.EventNames.OnInputUIAbilityTurret,
                EventNameSet = EventList.EventNames.OnInputAbilityTurretSet
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
                SelfHeal= 200,
                IsNonInterrupting = true,
                ConsiderMostAmmoInGroup = true,
                ConsiderLeastHealthInGroup = true,
                AmmoCostInstant = 50,
                EventNameKey = EventList.EventNames.OnInputKeyAbilityNanoPack,
                EventNameUI = EventList.EventNames.OnInputUIAbilityNanoPack,
                EventNameSet = EventList.EventNames.OnInputAbilityNanoPackSet
            };
            return ability;
        }

        
    }
}
