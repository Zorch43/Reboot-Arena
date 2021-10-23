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
                Description = "Genralist frontliner, adept at both attacking and holding points.",
                MaxHP = 400,
                MaxMP = 100,
                MoveSpeed = 1,
                TurnSpeed = 12,
                PrimaryWeapon = WeaponTemplates.CreateAssaultRifle(),
                SecondaryWeapon = WeaponTemplates.CreateShotGun(),
                SpecialAbility = UnitAbilityTemplates.CreateFragGrenade()
            };
            return template;
        }
    }
}
