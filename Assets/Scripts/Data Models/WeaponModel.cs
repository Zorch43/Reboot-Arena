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
        public const float WEAPON_RANGE_MELEE = 0;
        public const float WEAPON_RANGE_SHORT = 0.64f;//2
        public const float WEAPON_RANGE_MEDIUM_SHORT = 1.6f;//5
        public const float WEAPON_RANGE_MEDIUM = 3.2f;//10
        public const float WEAPON_RANGE_MEDIUM_LONG = 4.8f;//15
        public const float WEAPON_RANGE_LONG = 6.4f;//20
        public const float WEAPON_RANGE_VERY_LONG = 9.6f;//30
        public const float WEAPON_RANGE_EXTREME = 16;//50
        public enum AttackArea
        {
            Single,//strike a single target
            Line,//strike all targets on a line 
            Cone,//strike all targets within cone area
            Blast//strike target and all targets withing radius
        }
        #endregion
        #region template properties
        public string Name { get; set; }//name of weapon
        public float Range { get; set; }//maximum range of weapon
        public float ProjectileSpeed { get; set; }//speed of weapon projectiles
        public AttackArea WeaponAOE { get; set; }//type of weapon area-of-effect
        public float WeaponAOESize { get; set; }//size of weapon aoe - affects blast radius, line and cone width
        public bool ArcingAttack { get; set; }//whether the attacks arcs over obstacles
        public float DamageFalloff { get; set; }//determines damage falloff at max range.  can be used to ramp up damage at long range as well
        public float Cooldown { get; set; }//time between attacks
        public float Damage { get; set; }//damage per attack
        public float AmmoCost { get; set; }//ammo cost, if any, to fire this weapon
                                           //TODO: what effects to apply on hit
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
        #endregion
    }
}
