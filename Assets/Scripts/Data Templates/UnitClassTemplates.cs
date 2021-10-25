using Assets.Scripts.Data_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data_Templates
{
    public static class UnitClassTemplates
    {
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
    }
}
