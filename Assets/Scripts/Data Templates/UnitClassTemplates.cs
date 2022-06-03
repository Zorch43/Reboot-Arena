using Assets.Scripts.Data_Models;
using Assets.Scripts.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Data_Templates
{
    public static class UnitClassTemplates
    {
        public enum UnitClasses
        {
            Trooper,
            Fabricator,
            Ranger,
            TurretDrone
        }
        public static UnitClassModel GetClassByName(UnitClasses unitClass)
        {
            switch (unitClass)
            {
                case UnitClasses.Trooper:
                    return GetTrooperClass();
                case UnitClasses.Fabricator:
                    return GetFabricatorClass();
                case UnitClasses.Ranger:
                    return GetRangerClass();
                case UnitClasses.TurretDrone:
                    return GetTurretDroneClass();
            }
            Debug.LogError("Invalid class requested!");
            return null;
        }
        public static UnitClassModel GetTrooperClass()
        {
            var template = new UnitClassModel()
            {
                ClassId = UnitClasses.Trooper,
                Name = "Trooper",
                Description = "Generalist frontliner, adept at both attacking and holding points.",
                Portrait = ResourceList.PORTRAIT_TROOPER,
                Symbol = ResourceList.SYMBOL_TROOPER,
                MaxHP = 500,
                MaxMP = 200,
                MoveSpeed = 3,
                TurnSpeed = 12,
                PrimaryWeapon = WeaponTemplates.CreateMachineGun(),
                SecondaryWeapon = WeaponTemplates.CreateAssaultRifle(),
                TargetedAbility = UnitAbilityTemplates.CreateFragGrenade(),
                ActivatedAbility = UnitAbilityTemplates.CreateNanoPack(),
                AttackerWeight = 2,
                DefenderWeight = 1
            };
            return template;
        }
        public static UnitClassModel GetFabricatorClass()
        {
            var template = new UnitClassModel()
            {
                ClassId = UnitClasses.Fabricator,
                Name = "Fabricator",
                Description = "Defensive backliner that can build walls and turrets, and refill ammo",
                Portrait = ResourceList.PORTRAIT_FABRICATOR,
                Symbol = ResourceList.SYMBOL_FABRICATOR,
                MaxHP = 300,
                MaxMP = 400,
                MoveSpeed = 3,
                TurnSpeed = 6,
                PrimaryWeapon = WeaponTemplates.CreateMunitionsPrinter(),
                SecondaryWeapon = WeaponTemplates.CreatePlasmaGun(),
                TargetedAbility = UnitAbilityTemplates.CreateTurretDrone(),
                ActivatedAbility = UnitAbilityTemplates.CreateNanoPack(),
                IsAmbidextrous = true,
                IncompatibleAmmo = true,
                AmmoRegenRate = 10,
                DefenderWeight = 1,
                SupportWeight = 2
            };
            return template;
        }
        public static UnitClassModel GetTurretDroneClass()
        {
            var template = new UnitClassModel()
            {
                ClassId = UnitClasses.TurretDrone,
                Name="Turret Drone",
                Description = "Sentry turret that fires plasma at enemy units.  Uses ammo to repair and upgrade itself.",
                MaxHP = 600,
                MaxMP = 800,
                PrimaryWeapon = WeaponTemplates.CreatePlasmaTurret(),
                SecondaryWeapon = WeaponTemplates.CreateWeakPlasmaTurret(),
                AutoRepairStrength = 40,
                DefenderWeight = 1
            };
            return template;
        }
        public static UnitClassModel GetRangerClass()
        {
            var template = new UnitClassModel()
            {
                ClassId = UnitClasses.Ranger,
                Name = "Ranger",
                Description = "Lightweight, turbo-charged attacker that can dispense Nano-packs.",
                Portrait = ResourceList.PORTRAIT_RANGER,
                Symbol = ResourceList.SYMBOL_RANGER,
                MaxHP = 200,
                MaxMP = 200,
                MoveSpeed = 3,//non-boosted speed
                TurnSpeed = 9,
                HasJumpBoost = true,
                SpeedBoostPower = 5,//boosted speed bonus
                FuelConsumption = 10,//fuel cost per second while moving
                HealthPickupEfficiency = 2,
                AmmoPickupEfficiency = 2,
                PrimaryWeapon = WeaponTemplates.CreateLaserPistols(),
                TargetedAbility = UnitAbilityTemplates.CreateFragGrenade(),
                ActivatedAbility = UnitAbilityTemplates.CreateNanoPack(),
                AttackerWeight = 2,
                SupportWeight = 1
            };
            return template;
        }
    }
}
