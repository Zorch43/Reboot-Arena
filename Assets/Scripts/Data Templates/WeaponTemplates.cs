using Assets.Scripts.Data_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data_Templates
{
    public static class WeaponTemplates
    {
        public static WeaponModel CreateAssaultRifle()
        {
            var weapon = new WeaponModel()
            {
                Name = "Assault Rifle",
                Cooldown = 1f,
                Damage = 40,
                DamageFalloff = 0.25f,
                Range = WeaponModel.WEAPON_RANGE_MEDIUM,
                CanAutoAttack = true,
                FireWhileMoving = true
            };
            return weapon;
        }
        public static WeaponModel CreateGrenadeLauncher()
        {
            var weapon = new WeaponModel()
            {
                Name = "Grenade Launcher",
                Cooldown = 2,
                Damage = 80,
                Range = WeaponModel.WEAPON_RANGE_MEDIUM_SHORT,
                WeaponAOE = WeaponModel.AttackArea.Blast,
                WeaponAOESize = 0.64f,
                ArcingAttack = true,
                ProjectileSpeed = 3.2f
            };
            return weapon;
        }
    }
}
