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
                MaxHP = 400,
                MaxMP = 100,
                MoveSpeed = 1,
                TurnSpeed = 12,
                SecondaryWeapon = WeaponTemplates.CreateAssaultRifle(),
                PrimaryWeapon = WeaponTemplates.CreateFlameThrower()
            };
            return template;
        }
    }
}
