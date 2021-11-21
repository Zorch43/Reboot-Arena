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
        #region Trooper
        public static WeaponModel CreateAssaultRifle()
        {
            //accurate, needs no ammo, can fire on the move
            var weapon = new WeaponModel()
            {
                Name = "Assault Rifle (Semi-Auto)",
                Cooldown = .5f,
                HealthDamage = 10,
                DamageFalloff = 0.25f,
                MaxRange = WeaponModel.WEAPON_RANGE_MEDIUM,
                CanAutoAttack = true,
                FireWhileMoving = true,
                ProjectileSpeed = 20f,
                ProjectileStartSize = .05f,
                ProjectileEndSize = .05f
            };
            return weapon;
        }
        public static WeaponModel CreateMachineGun()
        {
            //inaccurate, long-range, rapid fire.  can't fire while moving, uses ammo
            var weapon = new WeaponModel()
            {
                Name = "Assault Rifle (Full Auto)",
                Cooldown = .2f,
                HealthDamage = 10,
                MaxRange = WeaponModel.WEAPON_RANGE_MEDIUM_LONG,
                ProjectileSpeed = 20f,
                InAccuracy = 10,
                CanAutoAttack = true,
                AmmoCost = 2,
                ProjectileStartSize = .05f,
                ProjectileEndSize = .05f
            };
            return weapon;
        }
        public static WeaponModel CreateFragGrenade()
        {
            //lobbed explosive.  uses a lot of ammo
            var weapon = new WeaponModel()
            {
                Name = "Frag Grenade",
                Cooldown = 1,
                HealthDamage = 50,
                MaxRange = WeaponModel.WEAPON_RANGE_MEDIUM_SHORT,
                Explodes = true,
                ExplosionSize = 2f,
                ArcingAttack = true,
                ProjectileSpeed = 3.2f,
                ProjectileStartSize = .1f,
                ProjectileEndSize = .1f,
                AmmoCost = 100
            };
            return weapon;
        }
        #endregion
        #region Fabricator
        //fabricator weapon
        public static WeaponModel CreatePlasmaGun()
        {
            var weapon = new WeaponModel()
            {
                Name = "Plasma Gun",
                Cooldown = 1f,
                HealthDamage = 25,
                MaxRange = WeaponModel.WEAPON_RANGE_MEDIUM,
                CanAutoAttack = true,
                FireWhileMoving = true,
                ProjectileSpeed = 7f,
                ProjectileStartSize = 0.15f,
                ProjectileEndSize = 0.15f,
                FiringArc = 360,
                TraversalSpeed = 6
            };
            return weapon;
        }
        public static WeaponModel CreateMunitionsPrinter()
        {
            var weapon = new WeaponModel()
            {
                Name = "Munitions Printer",
                Cooldown = 0.1f,
                AmmoDamage = -10,
                AmmoCost = 5,
                CanAutoAttack = true,
                MaxRange = WeaponModel.WEAPON_RANGE_MELEE,
                ProjectileStartSize = 0.15f,
                PiercesWalls = true
            };
            return weapon;
        }
        public static WeaponModel CreateBuildTools()
        {
            var weapon = new WeaponModel()
            {
                Name = "Build Tools",
                Cooldown = 2,
                MaxRange = WeaponModel.WEAPON_RANGE_MELEE,
                AmmoCost = 100,
                ProjectileStartSize = 0.25f
            };
            return weapon;
        }
        //turret weapon
        public static WeaponModel CreatePlasmaTurret()
        {
            var weapon = CreatePlasmaGun();

            weapon.Name = "Plasma Turret";
            weapon.AmmoCost = 5;

            return weapon;
        }
        public static WeaponModel CreateWeakPlasmaTurret()
        {
            var weapon = CreatePlasmaGun();

            weapon.Name = "Plasma Turret (Weak)";
            weapon.HealthDamage = 10;
            weapon.ProjectileStartSize = 0.1f;
            weapon.ProjectileEndSize = 0.1f;

            return weapon;
        }
        #endregion
        #region toybox
        //test explosions
        public static WeaponModel CreateGrenadeLauncher()
        {
            var weapon = new WeaponModel()
            {
                Name = "Grenade Launcher",
                Cooldown = 2,
                HealthDamage = 60,
                MaxRange = WeaponModel.WEAPON_RANGE_MEDIUM,
                Explodes = true,
                ExplosionSize = 0.64f,
                ArcingAttack = true,
                ProjectileSpeed = 3.2f
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
                HealthDamage = 100,
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
                HealthDamage = 1.5f,
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
                HealthDamage = 45,
                MaxRange = WeaponModel.WEAPON_RANGE_MEDIUM,
                ProjectileSpeed = 6.4f,
                ProjectileBurstSize = 5,
                ProjectileBurstSpread = 30,
                CanAutoAttack = true,
                FireWhileMoving = true
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
                HealthDamage = 5,
                MaxRange = WeaponModel.WEAPON_RANGE_SHORT,
                ProjectileStartSize = 0.16f,
                ProjectileEndSize = 0.02f,
                CanAutoAttack = true,
                FireWhileMoving = true
            };
            return weapon;
        }
        #endregion

    }
}
