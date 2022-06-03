using Assets.Scripts.Data_Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data_Models
{
    public class UnitClassModel
    {
        #region properties
        public UnitClassTemplates.UnitClasses ClassId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Symbol { get; set; }
        public string Portrait { get; set; }
        #region stats
        public int MaxHP { get; set; }//maximum health points
        public int MaxMP { get; set; }//maximum munition points
        public float MoveSpeed { get; set; }//base move speed
        public float TurnSpeed { get; set; }//base turn speed
        #endregion
        #region weapons
        //weapons
        public WeaponModel PrimaryWeapon { get; set; }
        public WeaponModel SecondaryWeapon { get; set; }
        //special abilities
        public UnitAbilityModel TargetedAbility { get; set; }
        public UnitAbilityModel ActivatedAbility { get; set; }
        #endregion
        #region abilities
        //passive abilities
        public bool IsAmbidextrous { get; set; }//whether the unit can fire primary and secondary weapons simultaneously, conditions permitting
        public float AutoRepairStrength { get; set; }//how much to heal per second, so long as unit can spend ammo
        public float HealthPickupEfficiency { get; set; } = 1;//effectiveness multiplier for heath pickups
        public float AmmoPickupEfficiency { get; set; } = 1;//effectiveness multiplier for ammo pickups
        public float SpeedBoostPower { get; set; }//speed boost while unit has ammo.  set negative to burden unit with ammo
        public float FuelConsumption { get; set; }//how much ammo is consumed while moving, per distance unit moved
        public bool HasJumpBoost { get; set; }//whether this unit can jump low obstacles
        public bool IncompatibleAmmo { get; set; }//if true, can't gain ammo from weapons or abilities
        public float AmmoRegenRate { get; set; }//rate at which this unit gains or loses ammo, regardless of other factors.
        #endregion
        #region AI helpers
        public float AttackerWeight { get; set; }//how good this class is at taking objectives and removing defenders from the point
        public float DefenderWeight { get; set; }//how good this class is at holding objectives against attackers
        public float SupportWeight { get; set; }//how well this class acts as a force-multiplier for other units
        #endregion
        #endregion
    }
}
