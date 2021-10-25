using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data_Models
{
    public class WeaponModel
    {
        #region constants
        public const float WEAPON_RANGE_MELEE = 1;
        public const float WEAPON_RANE_VERY_SHORT = 2.5f;
        public const float WEAPON_RANGE_SHORT = 5f;
        public const float WEAPON_RANGE_MEDIUM_SHORT = 7.5f;
        public const float WEAPON_RANGE_MEDIUM = 10;
        public const float WEAPON_RANGE_MEDIUM_LONG = 12.5f;
        public const float WEAPON_RANGE_LONG = 15;
        public const float WEAPON_RANGE_VERY_LONG = 20;
        public const float WEAPON_RANGE_EXTREME = 25;
        #endregion
        #region template properties
        public string Name { get; set; }//name of weapon
        public float MaxRange { get; set; }//maximum range of weapon
        public float MinRange { get; set; }//minimum range of the weapon
        public float ProjectileSpeed { get; set; }//speed of weapon projectiles
        //public AttackArea WeaponAOE { get; set; }//type of weapon area-of-effect
        public bool Explodes { get; set; }//whether the projectile produces an explosion on impact
        public float ExplosionSize { get; set; }//size of impact explosion, if generated
        public bool ArcingAttack { get; set; }//whether the attacks arcs over obstacles - projectile speed is ignored
        public float DamageFalloff { get; set; }//determines damage falloff at max range.  can be used to ramp up damage at long range as well
        public float Cooldown { get; set; }//time between attacks
        public float Damage { get; set; }//damage per attack
        public float ProjectileStartSize { get; set; } = .02f;//initial size of the projectile
        public float ProjectileEndSize { get; set; } = .02f;//size of the projectile by the end of range
        public int ProjectileBurstSize { get; set; } = 1;//number of projectiles produced by a single firing.  damge is divided evenly between projectiles
        public float ProjectileBurstSpread { get; set; }//angleof cone that burst fits into
        public float InAccuracy { get; set; }//maximum angle of trajectory deviation allowed for any given projectile
        public bool PiercesUnits { get; set; }//whether weapon projectiles pass through units
        public bool PiercesWalls { get; set; }//whether weapon projectiles pass through environmental obstacles
        public float AmmoCost { get; set; }//ammo cost, if any, to fire this weapon
        //TODO: what effects to apply on hit
        public bool FireWhileMoving { get; set; }//whether this weapon can be fired while moving
        public bool CanAutoAttack { get; set; }//whether this weapon can be used on autoattack targets
        #endregion
        #region dynamic properties

        //dynamic properties
        public float RemainingCooldown { get; set; }
        #endregion
        #region public methods
        public void DoCooldown(float time)
        {
            if (RemainingCooldown > 0)
            {
                RemainingCooldown -= time;
            }
            else if (RemainingCooldown < 0)
            {
                RemainingCooldown = 0;
            }
        }
        public void StartCooldown()
        {
            RemainingCooldown += Cooldown;
        }
        public bool IsCooledDown()
        {
            return RemainingCooldown <= 0;
        }
        public bool NeedsLineOfSight()
        {
            return !(ArcingAttack || PiercesWalls);
        }
        #endregion
    }
}
