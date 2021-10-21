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
                Cooldown = .5f,
                Damage = 10,
                DamageFalloff = 0.25f,
                MaxRange = WeaponModel.WEAPON_RANGE_MEDIUM,
                CanAutoAttack = true,
                FireWhileMoving = true,
                ProjectileSpeed = 6.4f
            };
            return weapon;
        }
        //test explosions
        public static WeaponModel CreateGrenadeLauncher()
        {
            var weapon = new WeaponModel()
            {
                Name = "Grenade Launcher",
                Cooldown = 2,
                Damage = 60,
                MaxRange = WeaponModel.WEAPON_RANGE_MEDIUM,
                Explodes = true,
                ExplosionSize = 0.64f,
                ArcingAttack = true,
                ProjectileSpeed = 3.2f
            };
            return weapon;
        }
        public static WeaponModel CreateFragGrenade()
        {
            var weapon = new WeaponModel()
            {
                Name = "Frag Grenade",
                Cooldown = 2,
                Damage = 100,
                MaxRange = WeaponModel.WEAPON_RANGE_MEDIUM,
                Explodes = true,
                ExplosionSize = 0.64f,
                ArcingAttack = true,
                ProjectileSpeed = 3.2f,
                ProjectileStartSize = .08f,
                ProjectileEndSize = .08f,
                AmmoCost = 100
            };
            return weapon;
        }
        //test piercing walls
        public static WeaponModel CreateRailgun()
        {
            var weapon = new WeaponModel()
            {
                Name = "Railgun",
                Cooldown = 4,
                Damage = 100,
                MaxRange = WeaponModel.WEAPON_RANGE_LONG,
                ProjectileSpeed = 0,
                PiercesUnits = true,
                PiercesWalls = true,
                ProjectileStartSize = .04f,
                ProjectileEndSize = .04f,
                AmmoCost = 25
            };
            return weapon;
        }
        //test changing size of projectiles
        public static WeaponModel CreateFlameThrower()
        {
            var weapon = new WeaponModel()
            {
                Name = "Flame Thrower",
                Cooldown = 0.1f,
                Damage = 1.5f,
                MaxRange = WeaponModel.WEAPON_RANGE_MEDIUM_SHORT,
                ProjectileSpeed = 1.6f,
                ProjectileStartSize = .08f,
                ProjectileEndSize = .32f,
                PiercesUnits = true,
                AmmoCost = 5,
                FireWhileMoving = true,
                CanAutoAttack = true
            };
            return weapon;
        }
        //test projectile bursts
        public static WeaponModel CreateShotGun()
        {
            var weapon = new WeaponModel()
            {
                Name = "Shotgun",
                Cooldown = 1.5f,
                Damage = 45,
                MaxRange = WeaponModel.WEAPON_RANGE_MEDIUM,
                ProjectileSpeed = 6.4f,
                ProjectileBurstSize = 5,
                ProjectileBurstSpread = 30,
                CanAutoAttack = true,
                FireWhileMoving = true
            };
            return weapon;
        }
        //test projectile inaccuracy
        public static WeaponModel CreateMinigun()
        {
            var weapon = new WeaponModel()
            {
                Name = "Minigun",
                Cooldown = .1f,
                Damage = 4,
                MaxRange = WeaponModel.WEAPON_RANGE_MEDIUM,
                ProjectileSpeed = 6.4f,
                InAccuracy = 10,
                CanAutoAttack = true
            };
            return weapon;
        }
        //melee weapon
        public static WeaponModel CreateBuzzSaw()
        {
            var weapon = new WeaponModel()
            {
                Name = "Buzz Saw",
                Cooldown = .1f,
                Damage = 5,
                MaxRange = WeaponModel.WEAPON_RANGE_SHORT,
                ProjectileStartSize = 0.16f,
                ProjectileEndSize = 0.02f,
                CanAutoAttack = true,
                FireWhileMoving = true
            };
            return weapon;
        }
    }
}
