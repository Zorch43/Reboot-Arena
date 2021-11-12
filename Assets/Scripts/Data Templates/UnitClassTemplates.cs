using Assets.Scripts.Data_Models;
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
            Fabricator
        }
        public static UnitClassModel GetClassByName(UnitClasses unitClass)
        {
            switch (unitClass)
            {
                case UnitClasses.Trooper:
                    return GetTrooperClass();
                case UnitClasses.Fabricator:
                    return GetFabricatorClass();
            }
            Debug.LogError("Invalid class requested!");
            return null;
        }
        public static UnitClassModel GetTrooperClass()
        {
            var template = new UnitClassModel()
            {
                Name = "Trooper",
                Description = "Generalist frontliner, adept at both attacking and holding points.",
                MaxHP = 500,
                MaxMP = 200,
                MoveSpeed = 3,
                TurnSpeed = 12,
                PrimaryWeapon = WeaponTemplates.CreateMachineGun(),
                SecondaryWeapon = WeaponTemplates.CreateAssaultRifle(),
                SpecialAbility = UnitAbilityTemplates.CreateFragGrenade()
            };
            return template;
        }
        public static UnitClassModel GetFabricatorClass()
        {
            var template = new UnitClassModel()
            {
                Name = "Fabricator",
                Description = "Defensive backliner that can build walls and turrets, and refill ammo",
                MaxHP = 300,
                MaxMP = 800,
                MoveSpeed = 3,
                TurnSpeed = 1,
                PrimaryWeapon = WeaponTemplates.CreateMunitionsPrinter(),
                SecondaryWeapon = WeaponTemplates.CreatePlasmaGun(),
                SpecialAbility = UnitAbilityTemplates.CreateFragGrenade(),//TODO: replace with build
                IsAmbidextrous = true
            };
            return template;
        }
    }
}
